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
        public static string ExcelFileNamePropertyName = "ExcelFileName";

        public string ExcelFileName
        {
            get
            {
                return _excelFileName;
            }
            set
            {
                this.MutateVerbose(ref _excelFileName, value, RaisePropertyChanged());
            }
        }

        public static string EipFileNamePropertyName = "EipFileName";

        public string EipFileName
        {
            get
            {
                return _eipFileName;
            }
            set
            {
                this.MutateVerbose(ref _eipFileName, value, RaisePropertyChanged());
            }
        }

        public static string ExecuteEnabledPropertyName = "ExecuteEnabled";

        public bool ExecuteEnabled
        {
            get
            {
                return _executeEnabled;
            }
            set
            {
                this.MutateVerbose(ref _executeEnabled, value, RaisePropertyChanged());
            }
        }

        public static string SheetItemsPropertyName = "SheetItems";

        public ObservableCollection<SheetCheckBoxItem> SheetItems
        {
            get
            {
                return _sheetItems;
            }
            set
            {
                this.MutateVerbose(ref _sheetItems, value, RaisePropertyChanged());
            }
        }

        public static string IsLoadingPropertyName = "IsLoading";

        public Visibility IsLoading
        {
            get
            {
                return _isLoading;
            }
            set
            {
                this.MutateVerbose(ref _isLoading, value, RaisePropertyChanged());
            }
        }

        public static string SheetSelectionVisibilityPropertyName = "SheetSelectionVisibility";
        public Visibility SheetSelectionVisibility
        {
            get
            {
                return _sheetSelectionVisibility;
            }
            set
            {
                this.MutateVerbose(ref _sheetSelectionVisibility, value, RaisePropertyChanged());
            }
        }

        public static string ErrorPropertyName = "Error";

        public string Error {
            get { return _error; }

            set
            {
                this.MutateVerbose(ref _error, value, RaisePropertyChanged());
            }
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