// Autofac's IsRegistered/Resolve are extension methods in Autofac.ResolutionExtensions,
// so the namespace must be imported — fully qualifying Autofac.IContainer is not enough.
using Autofac;
using System;
using System.Collections.Generic;
using System.Windows.Controls;

namespace AutoLotManager.Desktop.Navigation
{
    /// <summary>
    /// Service that provides convention-based navigation for WPF pages
    /// </summary>
    public class NavigationService : INavigationService
    {
        private readonly Dictionary<string, PageRegistration> _pageRegistrations;
        private readonly Autofac.IContainer _container;

        public NavigationService(Autofac.IContainer container)
        {
            _pageRegistrations = new Dictionary<string, PageRegistration>(StringComparer.OrdinalIgnoreCase);
            _container = container;
        }

        /// <summary>
        /// Navigate to a page by its key
        /// </summary>
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
        /// Register a page with its view model
        /// </summary>
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

        private class PageRegistration
        {
            public Type PageType { get; set; }
            public Type ViewModelType { get; set; }
        }
    }
}
