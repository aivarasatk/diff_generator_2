using DiffGenerator2.Constants;
using DiffGenerator2.DTOs;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Reactive.Subjects;
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

        public static string SheetItemsPropertyName = "SheetNamesPropertyName";

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

        #region IDataErrorInfo Members
        public string Error => throw new NotImplementedException();

        public string this[string columnName]
        {
            get
            {
                string result = null;
                if(columnName == "ExcelFileName")
                {
                    if (string.IsNullOrEmpty(ExcelFileName) || ExcelFileName == UIDefault.FileNotSelected)
                    {
                        result = "Būtina pasirinkti Excelio failą";
                    }
                }
                if(columnName == "EipFileName")
                {
                    if (string.IsNullOrEmpty(EipFileName) || ExcelFileName == UIDefault.FileNotSelected)
                    {
                        result = "Būtina pasirinkti Eip failą";
                    }
                }
                return result;
            }
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