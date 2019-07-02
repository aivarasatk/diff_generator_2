using DiffGenerator2.DTOs;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace DiffGenerator2.Model
{
    public partial class MainModel
    {
        private string _excelFileName;
        private string _eipFileName;
        private bool _executeEnabled;
        private Visibility _isLoading;
        private ObservableCollection<SheetCheckBoxItem> _sheetItems;

        private string _error;
        private List<string> _errors = new List<string>();

        public ICommand SelectExcelFileCommand { get; set; }
        public ICommand SelectEipFileCommand { get; set; }
        public ICommand ExecuteCommand { get; set; }

        public MainModel()
        {
            _sheetItems = new ObservableCollection<SheetCheckBoxItem>();
        }

    }
}
