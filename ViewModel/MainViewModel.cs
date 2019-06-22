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
using System.Threading.Tasks;

namespace DiffGenerator2.ViewModel
{
    public class MainViewModel
    {
        private readonly ICommandFactory _commandFactory;
        private readonly ILifetimeService _lifetimeService;
        private readonly IExcelReader _excelReader;
        private readonly IEipReader _eipReader;
        public MainModel Model { get; }
        private ILogService _logService;

        public MainViewModel(ICommandFactory commandFactory, ILogService logService, ILifetimeService lifetimeService,
                              IExcelReader excelReader, IEipReader eipReader)
        {
            _commandFactory = commandFactory;
            _logService = logService;
            _lifetimeService = lifetimeService;
            _excelReader = excelReader;
            _eipReader = eipReader;
            Model = new MainModel();

            InitComponents();
            Build();
        }

        private void Build()
        {
            Model.SelectExcelFileCommand = _commandFactory.CreateCommand(() => SetExcelRelatedFields(FileSelect.Excel));
            Model.SelectEipFileCommand = _commandFactory.CreateCommand(() => SetSelectedFileName(FileSelect.Eip));
            Model.ExecuteCommand = _commandFactory.CreateCommand(() => GenerateDiff());
        }

        private void SetExcelRelatedFields(FileSelect excel)
        {
            SetSelectedFileName(excel);

            _logService.Information("Getting excel sheet names");
            var excelSheetNames = _excelReader.GetAvailableSheetNames(Model.ExcelFileName);
            
            Model.SheetItems.Clear();
            _logService.Information("Creating checkboxes");
            foreach (var sheetName in excelSheetNames)
            {
                Model.SheetItems.Add(new SheetCheckBoxItem
                {
                    Name = sheetName,
                    IsChecked = true
                });
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

        private void GenerateDiff()
        {
            try
            {
                var excelProductData = _excelReader.GetExcelProductData(Model.SheetItems.Where(item => item.IsChecked));
                
            }
            catch(Exception ex)
            {
                _logService.Error("Gerenate Diff error. ", ex);
                //show error to user.
                throw;//TODO: REMOVE THROW WHEN USR ERROR IS COMPELETED
            }
        }

        private void InitComponents()
        {
            Model.ExcelFileName = UIDefault.FileNotSelected;
            Model.EipFileName = UIDefault.FileNotSelected;
            Model.ExecuteEnabled = true;
        }
    }
}
