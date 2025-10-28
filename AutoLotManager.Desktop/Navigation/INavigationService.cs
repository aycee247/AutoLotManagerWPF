using System.Windows.Controls;

namespace AutoLotManager.Desktop.Navigation
{
    /// <summary>
    /// Interface for navigation service that handles page navigation
    /// </summary>
    public interface INavigationService
    {
        /// <summary>
        /// Navigate to a page by its key/name
        /// </summary>
        /// <param name="pageKey">The key identifying the page (e.g., "Home", "Inventory")</param>
        /// <returns>The page instance to navigate to</returns>
        Page NavigateTo(string pageKey);

        /// <summary>
        /// Register a page type with a key
        /// </summary>
        /// <param name="pageKey">The key to identify the page</param>
        /// <param name="pageType">The type of the page</param>
        /// <param name="viewModelType">The type of the view model (optional)</param>
        void RegisterPage(string pageKey, System.Type pageType, System.Type viewModelType = null);
    }
}
