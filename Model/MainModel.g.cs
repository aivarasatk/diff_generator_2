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
    public partial class MainModel:INotifyPropertyChanged
    {
        public static string ExcelFileNamePropertyName = "ExcelFileName";
        partial void OnExcelFileNameChanged();

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
                OnExcelFileNameChanged();
            }
        }

        public static string EipFileNamePropertyName = "EipFileName";
        partial void OnEipFileNameChanged();

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
                OnEipFileNameChanged();
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

        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged([CallerMemberName]string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }


    }
}
