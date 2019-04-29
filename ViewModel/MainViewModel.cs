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
        private readonly INamingSchemeReader _namingSchemeReader;
        private readonly ILifetimeService _lifetimeService;
        private readonly IExcelReader _excelReader;
        private readonly IEipReader _eipReader;
        public MainModel Model { get; }
        private ILogService _logService;

        public MainViewModel(ICommandFactory commandFactory, ILogService logService, ILifetimeService lifetimeService,
                             INamingSchemeReader namingSchemeReader, IExcelReader excelReader, IEipReader eipReader)
        {
            _commandFactory = commandFactory;
            _logService = logService;
            _lifetimeService = lifetimeService;
            _namingSchemeReader = namingSchemeReader;
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
                Model.SheetItems.Add(new SheetItem
                {
                    Name = sheetName,
                    IsChecked = true
                });
            }

        }

        private void SetSelectedFileName(FileSelect fileMode)
        {
            _logService.Information("Openning file select dialog with file mode filter: {Mode}", fileMode.ToString());
            OpenFileDialog openFileDialog = new OpenFileDialog();
            switch (fileMode)
            {
                case FileSelect.Excel:
                    openFileDialog.Filter = "Excel |*.xlsx";
                    if (openFileDialog.ShowDialog() == true)
                    {
                        Model.ExcelFileName = openFileDialog.FileName;
                    }
                    _logService.Information("File selected {FileName}", Model.ExcelFileName);
                    break;
                case FileSelect.Eip:
                    openFileDialog.Filter = "Eip |*.eip";
                    if (openFileDialog.ShowDialog() == true)
                    {
                        Model.EipFileName = openFileDialog.FileName;
                    }
                    _logService.Information("File selected {FileName}", Model.EipFileName);
                    break;
                default:
                    break;

            }
        }

        private void GenerateDiff()
        {
            try
            {
                var sheetNavigationDictionary = new Dictionary<string, SheetNavigation>();
                foreach(var sheetBox in Model.SheetItems)
                {
                    if (sheetBox.IsChecked)
                    {
                        _logService.Information($"Getting sheet navigation for {sheetBox.Name}");
                        var sheetNavigation = _excelReader.GetSheetNavigation(sheetBox.Name);
                        sheetNavigationDictionary.Add(sheetBox.Name, sheetNavigation);
                    }
                }
                var columnNamingScheme = _namingSchemeReader.GetColumnNamingScheme(ConfigurationManager.AppSettings["ColumnNamingSchemeFileName"]);
                
            }
            catch(Exception ex)
            {
                _logService.Error("Gerenate Diff crashed", ex);
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
