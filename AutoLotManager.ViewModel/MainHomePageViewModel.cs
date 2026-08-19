using Prism.Commands;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Input;

namespace AutoLotManager.ViewModel
{
    /// <summary>
    /// View model for the application's home page. It is a placeholder: it
    /// exposes no data and no commands, and contributes nothing beyond the
    /// change-notification behaviour inherited from
    /// <see cref="ViewModelBase"/>.
    /// </summary>
    /// <remarks>
    /// Being empty is not harmless here. The home page's tiles bind to
    /// ProgressTileClickedCommand, which lives on
    /// <see cref="MainWindowViewModel"/>. The page picks that up by inheriting
    /// the main window's DataContext, so the tiles work until the user
    /// navigates to "Home" and the navigation service assigns this view model
    /// as the page's DataContext. From then on the binding cannot resolve and
    /// clicking a tile silently does nothing.
    /// </remarks>
    public class MainHomePageViewModel : ViewModelBase
    {
        #region PrivateMembers

        #endregion

        #region Constructors
        // TODO: setup dependency injection, logging, data service, etc.
        /// <summary>
        /// Creates the home page view model. It does no work today; command and
        /// data wiring is still to be added.
        /// </summary>
        public MainHomePageViewModel()
            : base()
        {
            // WindowLoadedCommand = new DelegateCommand(WindowLoaded);
            
        }
        #endregion

        #region Public Properties and Backing Fields

        #endregion

        #region ICommands
        // public ICommand WindowLoadedCommand { get; }
        #endregion

        #region Command Methods/Callbacks
        
        #endregion

        #region Load Data
        // TODO: give this something to load

        #endregion

        #region Window Events

        #endregion
    }
}
