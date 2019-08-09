using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace DiffGenerator2.Model
{
    public class InformationDialogModel : INotifyPropertyChanged
    {
        private string _icon;
        private string _message;

        public string Icon
        {
            get { return _icon; }
            set
            {
                this.MutateVerbose(ref _icon, value, RaisePropertyChanged());
            }
        }

        public string Message
        {
            get { return _message; }
            set
            {
                this.MutateVerbose(ref _message, value, RaisePropertyChanged());
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private Action<PropertyChangedEventArgs> RaisePropertyChanged()
        {
            return args => PropertyChanged?.Invoke(this, args);
        }
    }
}
