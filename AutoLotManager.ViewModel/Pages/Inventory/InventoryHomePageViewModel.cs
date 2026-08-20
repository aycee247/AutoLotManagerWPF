using AutoLotManager.Core.Navigation;
using Prism.Commands;
using System;
using System.Windows.Input;

namespace AutoLotManager.ViewModel.Pages.Inventory
{
    /// <summary>
    /// View model for the inventory home page. It owns the commands behind the page's two
    /// tiles, both of which navigate elsewhere in the application.
    /// </summary>
    /// <remarks>
    /// This view model used to be empty while the page's tiles bound to
    /// <c>OpenAddEditInventoryPageCommand</c> and <c>OpenExportInventoryListCommand</c> —
    /// commands that existed on no view model at all. Clicking either tile did nothing.
    /// Navigation is requested through <see cref="IPageNavigator"/> rather than the Desktop
    /// project's navigation service, because this project targets netstandard2.0 and must
    /// stay free of WPF types.
    /// </remarks>
    public class InventoryHomePageViewModel : ViewModelBase
    {
        /// <summary>The page key for the add/edit inventory page.</summary>
        public const string AddEditInventoryPageKey = "AddEditInventory";

        /// <summary>The page key for the export inventory list page.</summary>
        public const string ExportInventoryListPageKey = "ExportInventoryList";

        private readonly IPageNavigator _pageNavigator;

        /// <summary>
        /// Creates the view model and wires its navigation commands.
        /// </summary>
        /// <param name="pageNavigator">
        /// Used to request navigation when a tile is clicked. Must not be null — a null
        /// navigator would restore the original defect in a harder-to-diagnose form.
        /// </param>
        /// <exception cref="ArgumentNullException"><paramref name="pageNavigator"/> is null.</exception>
        public InventoryHomePageViewModel(IPageNavigator pageNavigator)
        {
            if (pageNavigator == null)
            {
                throw new ArgumentNullException(nameof(pageNavigator));
            }

            _pageNavigator = pageNavigator;

            OpenAddEditInventoryPageCommand = new DelegateCommand(OpenAddEditInventoryPage);
            OpenExportInventoryListCommand = new DelegateCommand(OpenExportInventoryList);
        }

        #region ICommands
        /// <summary>
        /// Navigates to the add/edit inventory page. Bound to the page's first tile.
        /// </summary>
        public ICommand OpenAddEditInventoryPageCommand { get; }

        /// <summary>
        /// Navigates to the export inventory list page. Bound to the page's second tile.
        /// </summary>
        public ICommand OpenExportInventoryListCommand { get; }
        #endregion

        #region Command Methods/Callbacks
        private void OpenAddEditInventoryPage()
        {
            _pageNavigator.NavigateTo(AddEditInventoryPageKey);
        }

        private void OpenExportInventoryList()
        {
            _pageNavigator.NavigateTo(ExportInventoryListPageKey);
        }
        #endregion
    }
}
