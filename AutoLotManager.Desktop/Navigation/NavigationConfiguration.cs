using AutoLotManager.Desktop.Pages;
using AutoLotManager.ViewModel;
using AutoLotManager.ViewModel.Pages.Inventory;

namespace AutoLotManager.Desktop.Navigation
{
    /// <summary>
    /// Configuration class for registering navigation mappings.
    /// Each menu label is mapped explicitly to a Page type and an optional ViewModel type;
    /// types are not inferred from the key by naming convention.
    /// </summary>
    public static class NavigationConfiguration
    {
        /// <summary>
        /// Register all navigation pages with the navigation service
        /// </summary>
        public static void RegisterPages(INavigationService navigationService)
        {
            // Home page with view model
            navigationService.RegisterPage("Home", typeof(MainHomePage), typeof(MainHomePageViewModel));

            // Inventory page with view model
            navigationService.RegisterPage("Inventory", typeof(InventoryHomePage), typeof(InventoryHomePageViewModel));

            // Settings page - no view model for now
            navigationService.RegisterPage("Settings", typeof(SettingsHomePage));

            // Sales page - no view model for now
            navigationService.RegisterPage("Sales", typeof(SalesHomePage));

            // About page - no view model for now
            navigationService.RegisterPage("About", typeof(AboutPage));
        }
    }
}
