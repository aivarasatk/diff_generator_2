using Autofac;
using DiffGenerator2.Common;
using DiffGenerator2.Enums;
using DiffGenerator2.Interfaces;
using DiffGenerator2.Model;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiffGenerator2.ViewModel
{
    public class MainViewModel
    {
        private readonly ICommandFactory _commandFactory;
        public MainModel Model { get; }
        private ILogService _logService;

        public MainViewModel(ICommandFactory commandFactory, ILogService logService)
        {
            _commandFactory = commandFactory;
            _logService = logService;
            Model = new MainModel();

            InitComponents();
            Build();
        }

        private void Build()
        {
            Model.SelectExcelFileCommand = _commandFactory.CreateCommand(() => SetSelectedFileName(FileSelect.Excel));
            Model.SelectEipFileCommand = _commandFactory.CreateCommand(() => SetSelectedFileName(FileSelect.Eip));
            Model.ExecuteCommand = _commandFactory.CreateCommand(() => GenerateDiff());
        }

        

        private void SetSelectedFileName(FileSelect fileMode)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            switch (fileMode)
            {
                case FileSelect.Excel:
                    openFileDialog.Filter = "Excel |*.xlsx";
                    if (openFileDialog.ShowDialog() == true)
                    {
                        Model.ExcelFileName = openFileDialog.FileName;
                    }
                    break;
                case FileSelect.Eip:
                    openFileDialog.Filter = "Eip |*.eip";
                    if (openFileDialog.ShowDialog() == true)
                    {
                        Model.EipFileName = openFileDialog.FileName;
                    }
                    break;
                default:
                    break;

            }
        }

        private void GenerateDiff()
        {
            throw new NotImplementedException();
        }

        private void InitComponents()
        {
            Model.ExcelFileName = "Failas nepasirinktas";
            Model.EipFileName = "Failas nepasirinktas";
            Model.ExecuteEnabled = false;
        }
    }
}
