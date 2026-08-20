using AutoLotManager.Core.Navigation;
using AutoLotManager.Desktop.Navigation;
using AutoLotManager.ViewModel;
using MahApps.Metro.Controls;
using System;
using System.Diagnostics;
using System.Windows;

namespace AutoLotManager.Desktop
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : MetroWindow
    {
        private readonly MainWindowViewModel _vm;
        private readonly INavigationService _navigationService;
        private readonly IPageNavigator _pageNavigator;

        /// <summary>
        /// Creates the main window and subscribes to view-model-initiated navigation.
        /// </summary>
        public MainWindow(
            MainWindowViewModel vm,
            INavigationService navigationService,
            IPageNavigator pageNavigator)
        {
            InitializeComponent();

            _vm = vm;
            _navigationService = navigationService;
            _pageNavigator = pageNavigator;
            DataContext = _vm;

            // View models cannot reach INavigationService (it returns a WPF Page and this
            // project already references theirs), so they raise a request instead and the
            // shell performs the navigation. See AutoLotManager.Core.Navigation.
            _pageNavigator.NavigationRequested += OnNavigationRequested;
        }

        private void Tile_Click(object sender, RoutedEventArgs e)
        {
            Debug.WriteLine("Tile clicked");
        }

        private void hmcLeftMenu_ItemClick(object sender, ItemClickEventArgs args)
        {
            NavigateToMenuItem(args.ClickedItem);
        }

        /// <summary>
        /// Handles clicks in the menu's options section (About, and anything added there
        /// later).
        /// </summary>
        /// <remarks>
        /// Options items are a separate event on <see cref="HamburgerMenu"/>. Without this
        /// handler the About item never navigated: only ItemClick was wired, and it read the
        /// menu's SelectedItem, which an options click does not set. Both handlers now route
        /// through <see cref="NavigateToMenuItem"/> using the item that was actually clicked.
        /// </remarks>
        private void hmcLeftMenu_OptionsItemClick(object sender, ItemClickEventArgs args)
        {
            NavigateToMenuItem(args.ClickedItem);
        }

        private void NavigateToMenuItem(object clickedItem)
        {
            var menuItem = clickedItem as HamburgerMenuGlyphItem;
            if (menuItem == null)
            {
                return;
            }

            NavigateTo(menuItem.Label);
        }

        private void OnNavigationRequested(object sender, PageNavigationEventArgs e)
        {
            NavigateTo(e.PageKey);
        }

        /// <summary>
        /// Resolves and displays the page registered under <paramref name="pageKey"/>.
        /// </summary>
        private void NavigateTo(string pageKey)
        {
            try
            {
                frameContent.Content = _navigationService.NavigateTo(pageKey);
            }
            catch (InvalidOperationException ex)
            {
                // Page not registered. NavigationService throws this type deliberately.
                Debug.WriteLine($"Navigation error: {ex.Message}");
                // TODO (#92): log this properly and tell the user, rather than swallowing it.
            }
            // Covers ArgumentNullException (null key) and ArgumentException (empty/whitespace key).
            catch (ArgumentException ex)
            {
                Debug.WriteLine($"Navigation error: {ex.Message}");
            }
        }
    }
}
