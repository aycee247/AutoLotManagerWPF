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
        private MainWindowViewModel _vm;
        private INavigationService _navigationService;

        public MainWindow(MainWindowViewModel vm, INavigationService navigationService)
        {
            InitializeComponent();

            _vm = vm;
            _navigationService = navigationService;
            DataContext = _vm;
        }

        private void Tile_Click(object sender, RoutedEventArgs e)
        {
            Debug.WriteLine("Tile clicked");
        }

        private void hmcLeftMenu_ItemClick(object sender, ItemClickEventArgs args)
        {
            var menuItem = (args.Source as HamburgerMenu).SelectedItem as HamburgerMenuGlyphItem;
            if (menuItem == null)
                return;

            var label = menuItem.Label;
            
            try
            {
                // Use the navigation service to navigate to the page
                var page = _navigationService.NavigateTo(label);
                frameContent.Content = page;
            }
            catch (InvalidOperationException ex)
            {
                // Page not registered or navigation failed
                Debug.WriteLine($"Navigation error: {ex.Message}");
                // TODO: Consider adding user notification for production
            }
            // Covers ArgumentNullException (null key) and ArgumentException (empty/whitespace key).
            catch (ArgumentException ex)
            {
                Debug.WriteLine($"Navigation error: {ex.Message}");
            }
        }
    }
}
