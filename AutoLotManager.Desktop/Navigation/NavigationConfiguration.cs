using AutoLotManager.Desktop.Pages;
using AutoLotManager.ViewModel;
using AutoLotManager.ViewModel.Pages.Inventory;

namespace AutoLotManager.Desktop.Navigation
{
    /// <summary>
    /// The application's navigation table: the one place where page keys are mapped to page types.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Each menu label is mapped explicitly to a <see cref="System.Windows.Controls.Page"/> type and
    /// an optional ViewModel type; types are not inferred from the key by naming convention. Nothing
    /// reflects over type names, so adding a page means adding a
    /// <see cref="INavigationService.RegisterPage"/> call here (and, if its view model has
    /// dependencies, a container registration in <c>Bootstrapper</c>). GitHub issue #38 tracks
    /// implementing the convention that older documentation in this repository claimed.
    /// </para>
    /// <para>
    /// The keys are meant to match the hamburger menu <c>Label</c> values in <c>MainWindow.xaml</c>,
    /// which is what <c>MainWindow</c> passes to <see cref="INavigationService.NavigateTo"/>. The
    /// match is not exact today — "Sales" is registered with no menu item to reach it, see the note
    /// on that registration. Lookup is case-insensitive, so a casing change in the XAML does not
    /// break navigation, but a spelling change does.
    /// </para>
    /// </remarks>
    public static class NavigationConfiguration
    {
        /// <summary>
        /// Registers every page mapping the application knows about with
        /// <paramref name="navigationService"/>.
        /// </summary>
        /// <param name="navigationService">
        /// The navigation service to populate. Called once during startup, from <c>Bootstrapper</c>.
        /// Calling it again on the same service is harmless: keys are replaced rather than
        /// duplicated, since the last registration for a key wins. Not null-checked — a null
        /// service faults on the first <c>RegisterPage</c> call.
        /// </param>
        public static void RegisterPages(INavigationService navigationService)
        {
            // Home page with view model
            navigationService.RegisterPage("Home", typeof(MainHomePage), typeof(MainHomePageViewModel));

            // Inventory page with view model
            navigationService.RegisterPage("Inventory", typeof(InventoryHomePage), typeof(InventoryHomePageViewModel));

            // Settings page - no view model for now
            navigationService.RegisterPage("Settings", typeof(SettingsHomePage));

            // Sales page - no view model for now.
            // Registered ahead of the UI: MainWindow.xaml has no menu item with Label="Sales",
            // so this mapping is currently unreachable. See issue #8.
            navigationService.RegisterPage("Sales", typeof(SalesHomePage));

            // About page - no view model for now
            navigationService.RegisterPage("About", typeof(AboutPage));
        }
    }
}
