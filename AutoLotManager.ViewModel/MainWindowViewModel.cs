using AutoLotManager.Core;
using Bogus;
using Prism.Commands;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Text;
using System.Windows.Input;

namespace AutoLotManager.ViewModel
{
    /// <summary>
    /// View model for the application's main window. It owns the window title,
    /// the busy indicator, the currently selected navigation menu item, a
    /// collection of vehicles, and the commands the window's chrome invokes.
    /// </summary>
    /// <remarks>
    /// Because pages hosted in the window's frame inherit this DataContext
    /// until navigation replaces it, several of these members are also the
    /// binding targets for page-level controls, not just the window's own.
    /// </remarks>
    public class MainWindowViewModel : ViewModelBase
    {
        #region PrivateMembers

        #endregion

        #region Constructors
        // TODO: setup dependency injection, logging, data service, etc.
        /// <summary>
        /// Creates the main window view model, wiring up its commands and
        /// populating <see cref="Cars"/>.
        /// </summary>
        /// <remarks>
        /// There is no real data source yet: the constructor synchronously
        /// generates 1000 fake <see cref="Car"/> records with Bogus and adds
        /// them to <see cref="Cars"/>. That work happens on whichever thread
        /// constructs the view model, typically the UI thread, so construction
        /// is measurably slower than a trivial view model's and the data is
        /// random placeholder content rather than real inventory.
        /// </remarks>
        public MainWindowViewModel()
            : base()
        {
            WindowLoadedCommand = new DelegateCommand(WindowLoaded);
            ProgressTileClickedCommand = new DelegateCommand(ProgressTileClicked);
            GithubIconClickedCommand = new DelegateCommand(GithubIconClicked);

            Cars = new ObservableCollection<Car>();
            var cars = new Faker<Car>()
                .RuleFor(c => c.Vin, a => a.Vehicle.Vin())
                .RuleFor(c => c.Make, a => a.Vehicle.Manufacturer())
                .RuleFor(c => c.Model, a => a.Vehicle.Model())
                .RuleFor(c => c.Year, a => a.Random.Number(1980, 2024))
                .RuleFor(c => c.Color, a => a.Commerce.Color());

            for (int i = 0; i < 1000; i++)
            {
                var car = cars.Generate();
                Cars.Add(car);
            };
        }
        #endregion

        #region Public Properties and Backing Fields
        private string _windowTitle;

        /// <summary>
        /// Text shown in the main window's title bar. Starts out null and is
        /// first set when <see cref="WindowLoaded"/> runs.
        /// </summary>
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

        /// <summary>
        /// Whether the window's busy/progress ring is visible. Defaults to
        /// false and is toggled by <see cref="ProgressTileClickedCommand"/>;
        /// nothing currently drives it from real background work.
        /// </summary>
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

        private object _selectedMenuItem;

        /// <summary>
        /// The navigation menu entry the user currently has selected. Typed as
        /// <see cref="object"/> because the menu items are supplied by the WPF
        /// hamburger menu this property is bound to, and this netstandard
        /// assembly does not reference that UI type.
        /// </summary>
        /// <remarks>
        /// Effectively write-only today: the menu's SelectedItem binding pushes
        /// values in, but no code reads the property back. The window's item
        /// click handler goes to the control's own SelectedItem instead.
        /// </remarks>
        public object SelectedMenuItem
        {
            get 
            { 
                return _selectedMenuItem; 
            }
            set 
            { 
                _selectedMenuItem = value;
                OnPropertyChanged();
            }
        }


        private ObservableCollection<Car> _cars;

        /// <summary>
        /// The vehicles held by the main window, populated in the constructor
        /// with 1000 randomly generated placeholder records rather than data
        /// from a real inventory source.
        /// </summary>
        /// <remarks>
        /// Nothing currently displays this collection: the only view that ever
        /// bound to it, a DataGrid in MainWindow.xaml, is commented out. The
        /// records are still generated on every startup, so the cost is paid
        /// even though the data is never shown.
        /// </remarks>
        public ObservableCollection<Car> Cars
        {
            get
            {
                return _cars;
            }
            set
            {
                _cars = value;
                OnPropertyChanged();
            }
        }
        #endregion

        #region ICommands
        /// <summary>
        /// Command bound to the window's Loaded event; invokes
        /// <see cref="WindowLoaded"/>. Assigned once during construction.
        /// </summary>
        public ICommand WindowLoadedCommand { get; }

        /// <summary>
        /// Command bound to the tiles on the home and tiles-demo pages. Each
        /// execution flips <see cref="DisplayProgressRing"/>, showing or hiding
        /// the progress ring; it does not start any actual work.
        /// </summary>
        /// <remarks>
        /// Those pages reach this command only while they are still inheriting
        /// the main window's DataContext. Once navigation assigns a page its own
        /// view model, the binding no longer resolves and the tiles do nothing.
        /// </remarks>
        public ICommand ProgressTileClickedCommand { get; }

        /// <summary>
        /// Command bound to the GitHub icon. Executing it leaves the
        /// application: it uses <see cref="Process"/> to open the author's
        /// GitHub profile page in the user's default browser. Any failure to
        /// launch the browser surfaces as an exception out of the command.
        /// </summary>
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
        /// <summary>
        /// Runs when the main window has loaded. It is a placeholder: the only
        /// thing it currently does is overwrite <see cref="WindowTitle"/> with a
        /// diagnostic message confirming the callback fired.
        /// </summary>
        public void WindowLoaded()
        {
            WindowTitle = "Window view model WindowLoaded() called!";
        }
        #endregion

        #region Window Events

        #endregion
    }
}
