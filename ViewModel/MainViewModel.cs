using Autofac;
using DiffGenerator2.Common;
using DiffGenerator2.Constants;
using DiffGenerator2.DTOs;
using DiffGenerator2.Enums;
using DiffGenerator2.Interfaces;
using DiffGenerator2.Model;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace DiffGenerator2.ViewModel
{
    public class MainViewModel
    {
        private readonly ICommandFactory _commandFactory;
        private readonly ILifetimeService _lifetimeService;
        public MainModel Model { get; }
        

        private ILogService _logService;

        public MainViewModel(ICommandFactory commandFactory, ILogService logService, ILifetimeService lifetimeService)
        {
            _commandFactory = commandFactory ?? throw new ArgumentNullException(nameof(commandFactory));
            _logService = logService ?? throw new ArgumentNullException(nameof(logService));
            _lifetimeService = lifetimeService ?? throw new ArgumentNullException(nameof(lifetimeService));

            Model = new MainModel();

            _logService.Information("PROGRAM STARTING!");
            InitComponents();
            Build();
        }

        private void Build()
        {
            Model.SelectExcelFileCommand = _commandFactory.CreateCommand(async () => await SetExcelRelatedFields(FileSelect.Excel));
            Model.SelectEipFileCommand = _commandFactory.CreateCommand(() => SetSelectedFileName(FileSelect.Eip));
            Model.ExecuteCommand = _commandFactory.CreateCommand(async () => await GenerateDiff());
        }

        private async Task SetExcelRelatedFields(FileSelect excel)
        {
            Model.IsLoading = Visibility.Visible;
            try
            {
                SetSelectedFileName(excel);
                _logService.Information("Getting excel sheet names");
                var excelSheetNames = new List<string>();
                await Task.Run(() => {
                    excelSheetNames = _lifetimeService.ExecuteInLifetime<IEnumerable<string>, IExcelReader>(reader =>
                    {
                        return reader.GetAvailableSheetNames(Model.ExcelFileName);
                    }).ToList();
                });
            
                Model.SheetItems.Clear();
                _logService.Information("Creating checkboxes");

                Model.SheetSelectionVisibility = FileSelected(Model.ExcelFileName) ? Visibility.Visible : Visibility.Collapsed;
   
                foreach (var sheetName in excelSheetNames)
                {
                    Model.SheetItems.Add(new SheetCheckBoxItem
                    {
                        Name = sheetName,
                        IsChecked = true
                    });
                }
                Model.IsLoading = Visibility.Collapsed;
            }
            catch (Exception ex)
            {
                _logService.Error("Failed to finish displaying sheets to select. ", ex);
                Model.IsLoading = Visibility.Collapsed;
                MessageBox.Show($"Klaida, kuriant \"Excel\" lapų pasirinkimą:\n\"{ex.Message}\"", "Klaida", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private bool FileSelected(string fileName)
        {
            return !string.IsNullOrEmpty(fileName) && fileName != UIDefault.FileNotSelected;
        }

        private void SetSelectedFileName(FileSelect fileMode)
        {
            _logService.Information("Openning file select dialog with file mode filter: {Mode}", fileMode.ToString());
            switch (fileMode)
            {
                case FileSelect.Excel:
                    Model.ExcelFileName = GetSelectedFile("Excel |*.xlsx") ?? UIDefault.FileNotSelected;
                    _logService.Information("File selected {FileName}", Model.ExcelFileName);
                    break;
                case FileSelect.Eip:
                    Model.EipFileName = GetSelectedFile("Eip |*.eip") ?? UIDefault.FileNotSelected;
                    _logService.Information("File selected {FileName}", Model.EipFileName);
                    break;
                default:
                    break;

            }
        }

        private string GetSelectedFile(string filter)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Filter = filter
            };
            return openFileDialog.ShowDialog() == true ? openFileDialog.FileName : null;
        }

        private async Task GenerateDiff()
        {
            try
            {
                Model.IsLoading = Visibility.Visible;
                if (Model.SheetItems.All(sheet => !sheet.IsChecked))
                {
                    MessageBox.Show("Nėra pasirinktų „Excel“ lapų", "Klaida", MessageBoxButton.OK, MessageBoxImage.Information);
                    Model.IsLoading = Visibility.Collapsed;
                    return;
                }
                _logService.Information("Started generating diff");
                var diffReport = await GetDiffReport();
                await GenerateExcelReport(diffReport);

                Model.IsLoading = Visibility.Collapsed;
                MessageBox.Show($"Baiga kurti nesutapimų ataiskaitą", "Baigta", MessageBoxButton.OK, MessageBoxImage.Information);
                _logService.Information("Finished generating diff");
                _logService.Information("");
            }
            catch(Exception ex)
            {
                Model.IsLoading = Visibility.Collapsed;
                _logService.Error("Generate Diff error. ", ex);
                MessageBox.Show($"Klaida generuojant nesutapimų ataskaitą:\n\"{ex.Message}\"", "Klaida", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private Task<DiffReport> GetDiffReport()
        {
            return Task.Run(() => {
                var excelProductData = _lifetimeService.ExecuteInLifetime<IEnumerable<ExcelBlockData>, IExcelReader>(
                    reader =>  reader.GetExcelProductData(Model.ExcelFileName, Model.SheetItems.Where(item => item.IsChecked)));

                var eipData = _lifetimeService.ExecuteInLifetime<IEnumerable<I07>, IEipReader>(
                    reader => reader.GetEipContents(Model.EipFileName));

                return _lifetimeService.ExecuteInLifetime<DiffReport, IDiffGenerator>(
                    reader => reader.GenerateDiffReport(eipData.ToList(), excelProductData.ToList()));
            });
        }

        private Task GenerateExcelReport(DiffReport diffReport)
        {
            return Task.Run(() =>
            {
                _logService.Information("Start generating excel report");
                _lifetimeService.ExecuteInLifetime<IExcelReportGenerator>(generator => generator.GenerateReport(diffReport));
            });
        }

        private void InitComponents()
        {
            Model.ExcelFileName = UIDefault.FileNotSelected;
            Model.EipFileName = UIDefault.FileNotSelected;
            Model.ExecuteEnabled = true;
            Model.SheetSelectionVisibility = Visibility.Collapsed;
            Model.IsLoading = Visibility.Collapsed;
        }
    }
}
