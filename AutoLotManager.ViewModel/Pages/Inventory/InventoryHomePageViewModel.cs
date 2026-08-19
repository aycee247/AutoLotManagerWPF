using AutoLotManager.Core;
using Bogus;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace AutoLotManager.ViewModel.Pages.Inventory
{
    /// <summary>
    /// View model for the inventory section's landing page. It is a
    /// placeholder: it exposes no inventory data and no commands, contributing
    /// only the change-notification support inherited from
    /// <see cref="ViewModelBase"/>.
    /// </summary>
    /// <remarks>
    /// The navigation service does set this as the inventory page's DataContext,
    /// so the gap is visible at runtime: the page's tiles bind to
    /// OpenAddEditInventoryPageCommand and OpenExportInventoryListCommand, and
    /// neither exists here, so clicking a tile does nothing.
    /// </remarks>
    public class InventoryHomePageViewModel : ViewModelBase
    {
        /// <summary>
        /// Creates the inventory home page view model. It performs no
        /// initialisation; unlike
        /// <see cref="ExportInventoryListPageViewModel"/> it loads no data.
        /// </summary>
        public InventoryHomePageViewModel()
        {
        }
    }
}
