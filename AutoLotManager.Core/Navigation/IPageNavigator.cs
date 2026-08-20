using System;

namespace AutoLotManager.Core.Navigation
{
    /// <summary>
    /// Lets a view model ask for navigation without depending on WPF.
    /// </summary>
    /// <remarks>
    /// <para>
    /// The navigation service that actually creates pages lives in the Desktop project and
    /// returns a <c>System.Windows.Controls.Page</c>, so view models cannot use it directly:
    /// <c>AutoLotManager.ViewModel</c> targets netstandard2.0 and the Desktop project already
    /// references it, which would make the dependency circular as well as UI-bound.
    /// </para>
    /// <para>
    /// This interface is the seam. A view model calls <see cref="NavigateTo"/>; the shell
    /// (MainWindow) listens for <see cref="NavigationRequested"/> and performs the actual
    /// navigation. Nothing here knows what a page is.
    /// </para>
    /// </remarks>
    public interface IPageNavigator
    {
        /// <summary>
        /// Requests navigation to the page registered under <paramref name="pageKey"/>.
        /// </summary>
        /// <param name="pageKey">
        /// The registered page key, matched case-insensitively by the navigation service —
        /// for example "Inventory". Must not be null, empty or whitespace.
        /// </param>
        /// <exception cref="ArgumentNullException"><paramref name="pageKey"/> is null.</exception>
        /// <exception cref="ArgumentException">
        /// <paramref name="pageKey"/> is empty or consists only of whitespace.
        /// </exception>
        void NavigateTo(string pageKey);

        /// <summary>
        /// Raised when navigation has been requested. The shell subscribes to this; if
        /// nothing is subscribed the request is silently a no-op.
        /// </summary>
        event EventHandler<PageNavigationEventArgs> NavigationRequested;
    }

    /// <summary>
    /// Carries the requested page key for <see cref="IPageNavigator.NavigationRequested"/>.
    /// </summary>
    public class PageNavigationEventArgs : EventArgs
    {
        /// <summary>
        /// Creates the event arguments for a navigation request.
        /// </summary>
        /// <param name="pageKey">The requested page key.</param>
        public PageNavigationEventArgs(string pageKey)
        {
            PageKey = pageKey;
        }

        /// <summary>
        /// The requested page key.
        /// </summary>
        public string PageKey { get; }
    }
}
