using Prism.Commands;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Input;

namespace AutoLotManager.ViewModel
{
    public class MainWindowViewModel : ViewModelBase
    {
        #region PrivateMembers

        #endregion

        #region Constructors
        // TODO: setup dependency injection, logging, data service, etc.
        public MainWindowViewModel()
            : base()
        {
            WindowLoadedCommand = new DelegateCommand(WindowLoaded);
        }
        #endregion

        #region Public Properties and Backing Fields
        private string _windowTitle;

        public string WindowTitle
        {
            get 
            { 
                return _windowTitle; 
            }
            set 
            { 
                _windowTitle = value;
                OnPropertyChanged();
            }
        }

        #endregion

        #region ICommands
        public ICommand WindowLoadedCommand { get; }
        #endregion

        #region Command Methods/Callbacks

        #endregion

        #region Load Data
        // TODO: give this something to load
        public void WindowLoaded()
        {
            WindowTitle = "Window view model WindowLoaded() called!";
        }
        #endregion

        #region Window Events

        #endregion
    }
}
