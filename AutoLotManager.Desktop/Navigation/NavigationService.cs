// IsRegistered/Resolve are extension methods (defined on the Autofac.ResolutionExtensions type),
// so the Autofac namespace must be imported — fully qualifying Autofac.IContainer is not enough,
// because extension method lookup goes through imported namespaces rather than the type's name.
using Autofac;
using System;
using System.Collections.Generic;
using System.Windows.Controls;

namespace AutoLotManager.Desktop.Navigation
{
    /// <summary>
    /// Default <see cref="INavigationService"/> implementation: an in-memory registry of page keys
    /// mapped to page types, plus optional view model types.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Registration is explicit. Keys are mapped by hand — normally through
    /// <see cref="NavigationConfiguration.RegisterPages"/> — and nothing is inferred from the key by
    /// naming convention. This type does not reflect over type names, so adding a page means adding
    /// a <see cref="RegisterPage"/> call. GitHub issue #38 tracks the convention-based registration
    /// that the repository's older documentation described but which was never implemented.
    /// </para>
    /// <para>
    /// The registry uses <see cref="StringComparer.OrdinalIgnoreCase"/>, so lookups are
    /// case-insensitive, and re-registering an existing key replaces the previous mapping.
    /// </para>
    /// <para>
    /// Pages are always created with <see cref="Activator.CreateInstance(Type)"/> and therefore need
    /// a public parameterless constructor. View models are created either from the Autofac container
    /// or by <see cref="Activator.CreateInstance(Type)"/>; see <see cref="NavigateTo"/> for exactly
    /// when each path is taken.
    /// </para>
    /// </remarks>
    public class NavigationService : INavigationService
    {
        /// <summary>
        /// Page key to registration map, keyed case-insensitively.
        /// </summary>
        private readonly Dictionary<string, PageRegistration> _pageRegistrations;

        /// <summary>
        /// Container used to resolve view models, or <c>null</c> when none was supplied.
        /// A null container is treated the same as a container with nothing registered.
        /// </summary>
        private readonly Autofac.IContainer _container;

        /// <summary>
        /// Initializes a new instance with an empty, case-insensitive page registry.
        /// </summary>
        /// <param name="container">
        /// The Autofac container used to resolve view model instances. May be <c>null</c>: view
        /// models are then always constructed with <see cref="Activator.CreateInstance(Type)"/>,
        /// which is how the unit tests exercise this type. The argument is not validated.
        /// </param>
        public NavigationService(Autofac.IContainer container)
        {
            _pageRegistrations = new Dictionary<string, PageRegistration>(StringComparer.OrdinalIgnoreCase);
            _container = container;
        }

        /// <summary>
        /// Creates the page registered under <paramref name="pageKey"/> and attaches its view model.
        /// </summary>
        /// <param name="pageKey">
        /// The key identifying the page (for example "Home" or "Inventory"). Matched
        /// case-insensitively.
        /// </param>
        /// <returns>
        /// A newly created page instance — a fresh instance on every call, as nothing is cached.
        /// When the registration carries a view model type, an instance of it is assigned to the
        /// page's <see cref="System.Windows.FrameworkElement.DataContext"/>; otherwise the
        /// <c>DataContext</c> is left null.
        /// </returns>
        /// <remarks>
        /// View model creation takes one of two paths. If a container was supplied and the view
        /// model type is registered in it (<c>IsRegistered</c>), the instance comes from the
        /// container's <c>Resolve</c>. Otherwise — that is, when the type is not registered — it is
        /// constructed directly with
        /// <see cref="Activator.CreateInstance(Type)"/>. The direct construction is not a fallback
        /// for a failed resolve: if <c>Resolve</c> itself throws, that exception propagates to the
        /// caller and no instance is constructed here.
        /// </remarks>
        /// <exception cref="ArgumentNullException"><paramref name="pageKey"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentException">
        /// <paramref name="pageKey"/> is empty or consists only of whitespace. This is a plain
        /// <see cref="ArgumentException"/>, deliberately distinct from the
        /// <see cref="ArgumentNullException"/> thrown for a null key.
        /// </exception>
        /// <exception cref="InvalidOperationException">
        /// No page is registered under <paramref name="pageKey"/>. <c>MainWindow</c> catches this
        /// type specifically, so it is part of the contract.
        /// </exception>
        public Page NavigateTo(string pageKey)
        {
            if (pageKey == null)
            {
                throw new ArgumentNullException(nameof(pageKey));
            }

            if (string.IsNullOrWhiteSpace(pageKey))
            {
                throw new ArgumentException("Page key must not be empty or whitespace.", nameof(pageKey));
            }

            if (!_pageRegistrations.TryGetValue(pageKey, out var registration))
            {
                throw new InvalidOperationException($"Page '{pageKey}' is not registered. Please register it using RegisterPage method.");
            }

            // Create page instance
            var page = (Page)Activator.CreateInstance(registration.PageType);

            // If there's a view model type, create and set it
            if (registration.ViewModelType != null)
            {
                object viewModel;

                // Try to resolve from container if available
                if (_container != null && _container.IsRegistered(registration.ViewModelType))
                {
                    viewModel = _container.Resolve(registration.ViewModelType);
                }
                else
                {
                    // Not registered in the container — construct it directly. Note this is not a
                    // fallback for a failed Resolve: if Resolve itself throws, the exception bubbles up.
                    viewModel = Activator.CreateInstance(registration.ViewModelType);
                }

                page.DataContext = viewModel;
            }

            return page;
        }

        /// <summary>
        /// Maps a page key to a page type and an optional view model type.
        /// </summary>
        /// <param name="pageKey">
        /// The key to identify the page. Stored case-insensitively; registering a key that is
        /// already present replaces the previous registration — the last registration wins.
        /// </param>
        /// <param name="pageType">
        /// The page type to instantiate. Must derive from <see cref="Page"/> and, because it is
        /// created with <see cref="Activator.CreateInstance(Type)"/>, needs a public parameterless
        /// constructor.
        /// </param>
        /// <param name="viewModelType">
        /// The view model type to assign to the page's <c>DataContext</c>, or <c>null</c> for a page
        /// that needs none. It is stored as given — neither its assignability nor its
        /// constructibility is checked here, so a bad type surfaces only when
        /// <see cref="NavigateTo"/> is called.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="pageKey"/> or <paramref name="pageType"/> is <c>null</c>.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// <paramref name="pageKey"/> is empty or consists only of whitespace, or
        /// <paramref name="pageType"/> does not derive from <see cref="Page"/>.
        /// </exception>
        /// <remarks>
        /// Arguments are validated in order — <paramref name="pageKey"/> first (null, then
        /// empty/whitespace), then <paramref name="pageType"/> — so a bad key is reported even when
        /// <paramref name="pageType"/> is also invalid. <c>RegisterPage("", null)</c>, for instance,
        /// throws <see cref="ArgumentException"/> for the key rather than
        /// <see cref="ArgumentNullException"/> for the type.
        /// </remarks>
        public void RegisterPage(string pageKey, Type pageType, Type viewModelType = null)
        {
            if (pageKey == null)
            {
                throw new ArgumentNullException(nameof(pageKey));
            }

            if (string.IsNullOrWhiteSpace(pageKey))
            {
                throw new ArgumentException("Page key must not be empty or whitespace.", nameof(pageKey));
            }

            if (pageType == null)
            {
                throw new ArgumentNullException(nameof(pageType));
            }

            if (!typeof(Page).IsAssignableFrom(pageType))
            {
                throw new ArgumentException($"Type {pageType.Name} must derive from Page", nameof(pageType));
            }

            _pageRegistrations[pageKey] = new PageRegistration
            {
                PageType = pageType,
                ViewModelType = viewModelType
            };
        }

        /// <summary>
        /// A single entry in the page registry: the page type for a key and the optional view model
        /// type to attach to it.
        /// </summary>
        private class PageRegistration
        {
            /// <summary>
            /// The page type to instantiate. Always derives from <see cref="Page"/>, because
            /// <see cref="RegisterPage"/> rejects anything else.
            /// </summary>
            public Type PageType { get; set; }

            /// <summary>
            /// The view model type to assign to the page's <c>DataContext</c>, or <c>null</c> when
            /// the page has no view model.
            /// </summary>
            public Type ViewModelType { get; set; }
        }
    }
}
