using System.Windows.Controls;

namespace AutoLotManager.Desktop.Navigation
{
    /// <summary>
    /// Maps page keys to WPF <see cref="Page"/> types and creates the page instance for a key.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Registration is explicit: every key must be mapped by hand with
    /// <see cref="RegisterPage"/> (see <see cref="NavigationConfiguration"/>). Nothing is inferred
    /// from the key by naming convention — a key of "Inventory" does not resolve to an
    /// <c>InventoryHomePage</c> / <c>InventoryHomePageViewModel</c> pair on its own, despite the
    /// "convention-based" wording used elsewhere in this repository. GitHub issue #38 tracks
    /// adding a real convention.
    /// </para>
    /// <para>
    /// Keys are compared case-insensitively (<see cref="System.StringComparer.OrdinalIgnoreCase"/>),
    /// because the keys used at runtime are hamburger menu labels taken from
    /// <c>MainWindow.xaml</c>.
    /// </para>
    /// </remarks>
    public interface INavigationService
    {
        /// <summary>
        /// Creates the page registered under <paramref name="pageKey"/>.
        /// </summary>
        /// <param name="pageKey">
        /// The key identifying the page (for example "Home" or "Inventory"). Matched
        /// case-insensitively against the registered keys.
        /// </param>
        /// <returns>
        /// A newly created page instance. A new instance is created on every call — pages are not
        /// cached. If the registration supplied a view model type, an instance of it is assigned to
        /// the page's <see cref="System.Windows.FrameworkElement.DataContext"/>; otherwise the
        /// <c>DataContext</c> is left null.
        /// </returns>
        /// <exception cref="System.ArgumentNullException">
        /// <paramref name="pageKey"/> is <c>null</c>.
        /// </exception>
        /// <exception cref="System.ArgumentException">
        /// <paramref name="pageKey"/> is empty or consists only of whitespace. Note this is a plain
        /// <see cref="System.ArgumentException"/>, distinct from the
        /// <see cref="System.ArgumentNullException"/> thrown for a null key.
        /// </exception>
        /// <exception cref="System.InvalidOperationException">
        /// No page is registered under <paramref name="pageKey"/>. <c>MainWindow</c> catches this
        /// exception type specifically, so it is part of the contract rather than an
        /// implementation detail.
        /// </exception>
        Page NavigateTo(string pageKey);

        /// <summary>
        /// Maps a page key to a page type and an optional view model type.
        /// </summary>
        /// <param name="pageKey">
        /// The key to identify the page. Stored case-insensitively; registering a key that already
        /// exists replaces the previous registration (last registration wins).
        /// </param>
        /// <param name="pageType">
        /// The type of the page to create. Must derive from
        /// <see cref="System.Windows.Controls.Page"/>.
        /// </param>
        /// <param name="viewModelType">
        /// The type of the view model to assign to the page's <c>DataContext</c>, or <c>null</c>
        /// for a page that needs none. The type is not validated at registration time.
        /// </param>
        /// <exception cref="System.ArgumentNullException">
        /// <paramref name="pageKey"/> or <paramref name="pageType"/> is <c>null</c>.
        /// </exception>
        /// <exception cref="System.ArgumentException">
        /// <paramref name="pageKey"/> is empty or consists only of whitespace, or
        /// <paramref name="pageType"/> does not derive from
        /// <see cref="System.Windows.Controls.Page"/>.
        /// </exception>
        /// <remarks>
        /// <paramref name="pageKey"/> is validated before <paramref name="pageType"/>, so when both
        /// are invalid the key's exception is the one raised.
        /// </remarks>
        void RegisterPage(string pageKey, System.Type pageType, System.Type viewModelType = null);
    }
}
