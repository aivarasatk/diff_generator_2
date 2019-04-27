using DiffGenerator2.Constants;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reactive.Subjects;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
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
                //_eipFileNameSubject.OnNext(value);
            }
        }
        /*
        private ISubject<string> _eipFileNameSubject = new Subject<string>();

        public IObservable<string> EipFileNameObservable
        {
            get
            {
                return _eipFileNameSubject;
            }
        }
        */

        public static string ExecuteEnabledPropertyName = "ExecuteEnabled";
        partial void OnExecuteEnabledChanged();

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

                OnPropertyChanged();
                OnExecuteEnabledChanged();
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