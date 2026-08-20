using System;

namespace AutoLotManager.Core.Navigation
{
    /// <summary>
    /// Default <see cref="IPageNavigator"/>: validates the key and relays the request to
    /// whoever is listening.
    /// </summary>
    /// <remarks>
    /// Deliberately UI-free and living in Core, so it needs no entry in the Desktop project's
    /// legacy csproj and can be unit tested without a WPF host. Register it as a singleton so
    /// the view models raising requests and the shell handling them share one instance.
    /// </remarks>
    public class PageNavigator : IPageNavigator
    {
        /// <inheritdoc />
        public event EventHandler<PageNavigationEventArgs> NavigationRequested;

        /// <inheritdoc />
        public void NavigateTo(string pageKey)
        {
            // Validation mirrors NavigationService: null and empty are distinct failures.
            if (pageKey == null)
            {
                throw new ArgumentNullException(nameof(pageKey));
            }

            if (string.IsNullOrWhiteSpace(pageKey))
            {
                throw new ArgumentException("Page key must not be empty or whitespace.", nameof(pageKey));
            }

            NavigationRequested?.Invoke(this, new PageNavigationEventArgs(pageKey));
        }
    }
}
