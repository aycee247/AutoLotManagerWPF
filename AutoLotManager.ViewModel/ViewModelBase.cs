using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;

namespace AutoLotManager.ViewModel
{
    public abstract class ViewModelBase : INotifyPropertyChanged
    {
        // TODO: implement common dependencies to be injected into all view models
        public ViewModelBase()
        {

        }
        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged([CallerMemberName]string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
