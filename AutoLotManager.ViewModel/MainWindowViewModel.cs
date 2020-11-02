using Prism.Commands;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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
            ProgressTileClickedCommand = new DelegateCommand(ProgressTileClicked);
            GithubIconClickedCommand = new DelegateCommand(GithubIconClicked);
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

        private bool _displayProgressRing = default;

        public bool DisplayProgressRing
        {
            get
            {
                return _displayProgressRing;
            }
            set
            {
                _displayProgressRing = value;
                OnPropertyChanged();
            }
        }
        #endregion

        #region ICommands
        public ICommand WindowLoadedCommand { get; }
        public ICommand ProgressTileClickedCommand { get; }
        public ICommand GithubIconClickedCommand { get; }
        #endregion

        #region Command Methods/Callbacks
        private void ProgressTileClicked()
        {
            DisplayProgressRing = !_displayProgressRing;
        }

        private void GithubIconClicked()
        {
            Process.Start("http://www.github.com/aycee247/");
        }
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
