using Prism.Commands;
using System.Windows.Input;

namespace AutoLotManager.ViewModel
{
    /// <summary>
    /// View model for the application's home page. It owns the commands the home page's
    /// tiles bind to.
    /// </summary>
    /// <remarks>
    /// This view model used to be empty, which was not harmless: the home page's tiles bind
    /// to <c>ProgressTileClickedCommand</c>, and that command only existed on
    /// <see cref="MainWindowViewModel"/>. The tiles appeared to work because the page
    /// inherited the main window's DataContext — until the navigation service assigned this
    /// view model as the page's DataContext, after which the binding could not resolve and
    /// clicking a tile silently did nothing. The command now lives here, on the view model
    /// the page actually gets.
    /// </remarks>
    public class MainHomePageViewModel : ViewModelBase
    {
        #region Constructors
        /// <summary>
        /// Creates the home page view model and wires its commands.
        /// </summary>
        public MainHomePageViewModel()
            : base()
        {
            ProgressTileClickedCommand = new DelegateCommand(ProgressTileClicked);
        }
        #endregion

        #region Public Properties and Backing Fields
        private bool _displayProgressRing;

        /// <summary>
        /// Whether the home page's progress ring is shown. Toggled by
        /// <see cref="ProgressTileClickedCommand"/>.
        /// </summary>
        /// <remarks>
        /// Deliberately the page's own state rather than the shell's. The equivalent property
        /// on <see cref="MainWindowViewModel"/> drives the ring in the main window chrome;
        /// this one drives the ring on the home page itself, so a tile click has a visible
        /// effect on the page that raised it without the page reaching into the shell.
        /// </remarks>
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
        /// <summary>
        /// Invoked by the home page's tiles. Toggles <see cref="DisplayProgressRing"/>.
        /// </summary>
        public ICommand ProgressTileClickedCommand { get; }
        #endregion

        #region Command Methods/Callbacks
        private void ProgressTileClicked()
        {
            DisplayProgressRing = !_displayProgressRing;
        }
        #endregion
    }
}
