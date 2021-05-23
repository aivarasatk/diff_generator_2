using DiffGenerator2.Constants;
using DiffGenerator2.DTOs;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace DiffGenerator2.Model
{
    public partial class MainModel : INotifyPropertyChanged, IDataErrorInfo
    {
        public string ExcelFileName
        {
            get => _excelFileName;
            set => this.MutateVerbose(ref _excelFileName, value, RaisePropertyChanged());
        }

        public string EipFileName
        {
            get => _eipFileName;
            set => this.MutateVerbose(ref _eipFileName, value, RaisePropertyChanged());
        }

        public int OrderNumberRangeStart
        {
            get => _orderNumberRangeStart;
            set => this.MutateVerbose(ref _orderNumberRangeStart, value, RaisePropertyChanged());
        }

        public int OrderNumberRangeEnd
        {
            get => _orderNumberRangeEnd;
            set => this.MutateVerbose(ref _orderNumberRangeEnd, value, RaisePropertyChanged());
        }

        public bool ExecuteEnabled
        {
            get => _executeEnabled;
            set => this.MutateVerbose(ref _executeEnabled, value, RaisePropertyChanged());
        }

        public ObservableCollection<SheetCheckBoxItem> SheetItems
        {
            get => _sheetItems;
            set => this.MutateVerbose(ref _sheetItems, value, RaisePropertyChanged());
        }

        public ObservableCollection<SheetCheckBoxItem> MonthOnlySheets
        {
            get => _monthOnlySheets;
            set => this.MutateVerbose(ref _monthOnlySheets, value, RaisePropertyChanged());
        }


        public Visibility IsLoading
        {
            get => _isLoading;
            set => this.MutateVerbose(ref _isLoading, value, RaisePropertyChanged());
        }

        public Visibility SheetSelectionVisibility
        {
            get => _sheetSelectionVisibility;
            set => this.MutateVerbose(ref _sheetSelectionVisibility, value, RaisePropertyChanged());
        }

        public string Error 
        {
            get => _error;
            set => this.MutateVerbose(ref _error, value, RaisePropertyChanged());
        }

        public string this[string columnName]
        {
            get
            {             
                var error = "";
                switch (columnName)
                {
                    case "ExcelFileName":
                    {
                        if (!FileSelected(ExcelFileName))
                        {
                            error = "Būtina pasirinkti Excelio failą";
                            _errors.Add(error);
                        }
                        else
                        {
                            _errors.Remove("Būtina pasirinkti Excelio failą");
                        }
                        break;
                    }
                    case "EipFileName":
                    {
                        if (!FileSelected(EipFileName))
                        {
                            error = "Būtina pasirinkti Eip failą";
                            _errors.Add(error);
                        }
                        else
                        {
                            _errors.Remove("Būtina pasirinkti Eip failą");
                        }
                        break;
                    }
                }
                Error = string.Join("\n", _errors);
                ExecuteEnabled = string.IsNullOrEmpty(Error);
                return error;
            }
        }
        private bool FileSelected(string fileName)
        {
            return !string.IsNullOrEmpty(fileName) && fileName != UIDefault.FileNotSelected;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private Action<PropertyChangedEventArgs> RaisePropertyChanged()
        {
            return args => PropertyChanged?.Invoke(this, args);
        }
    }
}