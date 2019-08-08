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
                if (_excelFileName == value)
                    return;

                _excelFileName = value;

                OnPropertyChanged(ExcelFileNamePropertyName);
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
                if (_eipFileName == value)
                    return;

                _eipFileName = value;

                OnPropertyChanged(EipFileNamePropertyName);
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
                if (_executeEnabled == value)
                    return;

                _executeEnabled = value;

                OnPropertyChanged(ExecuteEnabledPropertyName);
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
                if (_sheetItems == value)
                    return;

                _sheetItems = value;

                OnPropertyChanged(SheetItemsPropertyName);
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
                if (_isLoading == value)
                    return;

                _isLoading = value;

                OnPropertyChanged(IsLoadingPropertyName);
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
                if (_sheetSelectionVisibility == value)
                    return;

                _sheetSelectionVisibility = value;
                OnPropertyChanged(SheetSelectionVisibilityPropertyName);
            }
        }
        #region IDataErrorInfo Members
        public static string ErrorPropertyName = "Error";

        public string Error {
            get { return _error; }

            set
            {
                if (_error == value)
                    return;

                _error = value;
                OnPropertyChanged(ErrorPropertyName);
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
        #endregion

        #region INotificationChanged Members
        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged([CallerMemberName]string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion
    }
}