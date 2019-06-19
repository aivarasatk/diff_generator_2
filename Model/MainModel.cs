using DiffGenerator2.DTOs;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace DiffGenerator2.Model
{
    public partial class MainModel
    {
        private string _excelFileName;
        private string _eipFileName;
        private bool _executeEnabled;
        private ObservableCollection<SheetItem> _sheetItems;

        public ICommand SelectExcelFileCommand { get; set; }
        public ICommand SelectEipFileCommand { get; set; }
        public ICommand ExecuteCommand { get; set; }

        public MainModel()
        {
            _sheetItems = new ObservableCollection<SheetItem>();
        }

    }
}
