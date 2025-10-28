using AutoLotManager.Desktop.Navigation;
using AutoLotManager.Desktop.Pages;
using AutoLotManager.ViewModel;
using AutoLotManager.ViewModel.Pages.Inventory;
using MahApps.Metro.Controls;
using System.Diagnostics;
using System.Web.UI.WebControls;
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
            catch (System.Exception ex)
            {
                Debug.WriteLine($"Navigation error: {ex.Message}");
                // Optionally show error to user
            }
        }
    }
}
