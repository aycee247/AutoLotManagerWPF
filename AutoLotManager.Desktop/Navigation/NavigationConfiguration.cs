using AutoLotManager.Desktop.Pages;
using AutoLotManager.ViewModel;
using AutoLotManager.ViewModel.Pages.Inventory;

namespace AutoLotManager.Desktop.Navigation
{
    /// <summary>
    /// Configuration class for registering navigation mappings
    /// Follows convention: "PageName" maps to "PageNamePage" (View) and "PageNameViewModel" (ViewModel)
    /// </summary>
    public static class NavigationConfiguration
    {
        /// <summary>
        /// Register all navigation pages with the navigation service
        /// </summary>
        public static void RegisterPages(INavigationService navigationService)
        {
            // Home page - no view model needed for now
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
