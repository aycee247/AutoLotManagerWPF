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
        public MainWindow(MainWindowViewModel vm)
        {
            InitializeComponent();

            _vm = vm;
            DataContext = _vm;
            
            
        }

        private void Tile_Click(object sender, RoutedEventArgs e)
        {
            Debug.WriteLine("Tile clicked");
        }

        private void hmcLeftMenu_ItemClick(object sender, ItemClickEventArgs args)
        {
            var label = ((args.Source as HamburgerMenu).SelectedItem as HamburgerMenuGlyphItem).Label;
            switch (label)
            {
                case "Home":
                    frameContent.Content = new MainHomePage();
                    break;
                case "Inventory":
                    frameContent.Content = new InventoryHomePage(new InventoryHomePageViewModel());
                    break;
                case "Settings":
                    frameContent.Content = new SettingsHomePage();
                    break;
                case "Sales":
                    frameContent.Content = new SalesHomePage();
                    break;
                case "About":
                    frameContent.Content = new AboutPage();
                    break;
                default:
                    break;
            }
        }
    }
}
