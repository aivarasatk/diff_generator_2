﻿using DiffGenerator2.DTOs;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;

namespace DiffGenerator2.Model
{
    public partial class MainModel
    {
        private string _excelFileName;
        private string _eipFileName;
        private bool _executeEnabled;

        private int _orderNumberRangeStart;
        private int _orderNumberRangeEnd;

        private Visibility _isLoading;
        private Visibility _sheetSelectionVisibility;

        private ObservableCollection<SheetCheckBoxItem> _sheetItems;
        private ObservableCollection<SheetCheckBoxItem> _monthOnlySheets;

        private string _error;
        private List<string> _errors = new List<string>();

        public ICommand SelectExcelFileCommand { get; set; }
        public ICommand SelectEipFileCommand { get; set; }
        public ICommand ExecuteCommand { get; set; }

        public MainModel()
        {
            _sheetItems = new ObservableCollection<SheetCheckBoxItem>();
            _monthOnlySheets = new ObservableCollection<SheetCheckBoxItem>();
        }

    }
}
