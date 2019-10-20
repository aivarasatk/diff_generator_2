﻿using Autofac;
using DiffGenerator2.Constants;
using DiffGenerator2.DTOs;
using DiffGenerator2.Enums;
using DiffGenerator2.Interfaces;
using DiffGenerator2.Model;
using DiffGenerator2.Views;
using MaterialDesignThemes.Wpf;
using System;
using System.Collections.Generic;
using System.Linq;
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
            Model.SelectExcelFileCommand = _commandFactory.CreateCommand(async () => await SetExcelRelatedFieldsAsync(FileSelect.Excel));
            Model.SelectEipFileCommand = _commandFactory.CreateCommand(async () => await SetEipRelatedFields(FileSelect.Eip));
            Model.ExecuteCommand = _commandFactory.CreateCommand(async () => await GenerateDiffAsync());
        }

        private async Task SetExcelRelatedFieldsAsync(FileSelect excel)
        {
            Model.IsLoading = Visibility.Visible;
            try
            {
                SetSelectedFileName(excel);

                Model.SheetItems.Clear();
                Model.MonthOnlySheets.Clear();

                if (FileSelected(Model.ExcelFileName))
                    await ShowAvailableSheetsAsync();
                else
                    Model.SheetSelectionVisibility = Visibility.Collapsed;

                Model.IsLoading = Visibility.Collapsed;
            }
            catch (Exception ex)
            {
                _logService.Error("Failed to finish displaying sheets to select. ", ex);
                Model.IsLoading = Visibility.Collapsed;
                await ShowMessageDialogAsync(DialogIcon.ErrorFilled, $"Klaida, kuriant \"Excel\" lapų pasirinkimą:\n\"{ex.Message}\"");
            }
        }

        private async Task ShowAvailableSheetsAsync()
        {
            _logService.Information("Getting excel sheet names");
            var excelSheetNames = await GetAvailableSheetNamesAsync();

            _logService.Information("Creating checkboxes");
            CreateCheckBoxesForSheets(excelSheetNames);
            Model.SheetSelectionVisibility = Visibility.Visible;
        }

        private async Task<IEnumerable<string>> GetAvailableSheetNamesAsync()
        {
            return await Task.Run(() =>
            {
                return _lifetimeService.ExecuteInLifetime<IEnumerable<string>, IExcelReader>(
                    reader => reader.GetAvailableSheetNames(Model.ExcelFileName)).ToList();
            });
        }

        private void CreateCheckBoxesForSheets(IEnumerable<string> excelSheetNames)
        {
            foreach (var sheetName in excelSheetNames)
            {
                Model.SheetItems.Add(new SheetCheckBoxItem
                {
                    Name = sheetName,
                    IsChecked = true
                });

                Model.MonthOnlySheets.Add(new SheetCheckBoxItem
                {
                    Name = sheetName,
                    IsChecked = false
                });
            }
        }

        private bool FileSelected(string fileName)
        {
            return !string.IsNullOrEmpty(fileName) && fileName != UIDefault.FileNotSelected;
        }

        private async Task SetEipRelatedFields(FileSelect eipMode)
        {
            try
            {
                SetSelectedFileName(eipMode);
            }
            catch(Exception ex)
            {
                _logService.Error($"Failed to select eip file", ex);

                await ShowMessageDialogAsync(DialogIcon.ErrorFilled, $"Klaida, pasirenkant .eip failą:\n\"{ex.Message}\"");
            }
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
                    throw new ArgumentException($"FileSelect enum does not contain {fileMode} value");

            }
        }

        private string GetSelectedFile(string filter)
        {
            return _lifetimeService.ExecuteInLifetime<string, IFileSelector>(selector => selector.GetSelectedFile(filter));
        }

        private async Task GenerateDiffAsync()
        {
            try
            {
                if (Model.SheetItems.All(sheet => !sheet.IsChecked))
                {
                    await ShowMessageDialogAsync(DialogIcon.InformationFilled, "Nėra pasirinktų „Excel“ lapų");
                    return;
                }

                Model.IsLoading = Visibility.Visible;

                _logService.Information("Started generating diff");
                var diffReport = await GetDiffReportAsync();
                await GenerateExcelReportAsync(diffReport);

                Model.IsLoading = Visibility.Collapsed;

                await ShowMessageDialogAsync(DialogIcon.InformationFilled, $"Baigta kurti nesutapimų ataskaita. Excel failas išsaugotas \"Reports\" kataloge.");

                _logService.Information("Finished generating diff");
                _logService.Information("");
            }
            catch(Exception ex)
            {
                Model.IsLoading = Visibility.Collapsed;
                _logService.Error("Generate Diff error. ", ex);
                await ShowMessageDialogAsync(DialogIcon.WarningFilled, $"Klaida generuojant nesutapimų ataskaitą:\n\"{ex.Message}\"");
            }
        }

        private Task<DiffReport> GetDiffReportAsync()
        {
            return Task.Run(() => {
                var excelProductData = _lifetimeService.ExecuteInLifetime<IEnumerable<ExcelBlockData>, IExcelReader>(
                    reader =>  reader.GetExcelProductData(Model.ExcelFileName, Model.SheetItems.Where(item => item.IsChecked)));

                var eipData = _lifetimeService.ExecuteInLifetime<IEnumerable<I07>, IEipReader>(
                    reader => {
                        var content = reader.GetEipContents(Model.EipFileName);
                        return reader.GetParsedEipContents(content);
                    });

                return _lifetimeService.ExecuteInLifetime<DiffReport, IDiffGenerator>(
                    reader => reader.GenerateDiffReport(eipData.ToList(), excelProductData.ToList()));
            });
        }

        private Task GenerateExcelReportAsync(DiffReport diffReport)
        {
            return Task.Run(() =>
            {
                _logService.Information("Start generating excel report");
                _lifetimeService.ExecuteInLifetime<IExcelReportGenerator>(generator => generator.GenerateReport(diffReport));
            });
        }

        private async Task ShowMessageDialogAsync(string icon, string message)
        {
            var view = new InformationDialogView
            {
                DataContext = new InformationDialogModel
                {
                    Icon = icon,
                    Message = message
                }
            };
            await DialogHost.Show(view, "RootDialog");
        }

        private void InitComponents()
        {
            Model.ExcelFileName = UIDefault.FileNotSelected;
            Model.EipFileName = UIDefault.FileNotSelected;
            Model.ExecuteEnabled = false;
            Model.SheetSelectionVisibility = Visibility.Collapsed;
            Model.IsLoading = Visibility.Collapsed;
        }
    }
}
