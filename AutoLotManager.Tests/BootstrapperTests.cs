using System;
using System.Threading;
using Autofac;
using AutoLotManager.Desktop;
using AutoLotManager.Desktop.Navigation;
using AutoLotManager.Desktop.Pages;
using AutoLotManager.Desktop.Startup;
using AutoLotManager.ViewModel;
using AutoLotManager.ViewModel.Pages.Inventory;
using NUnit.Framework;

namespace AutoLotManager.Tests
{
    /// <summary>
    /// Integration tests for the Autofac object graph (issue #18).
    ///
    /// The unit tests in this project all construct their subjects by hand, so nothing so far
    /// has proven that <see cref="Bootstrapper.Bootstrap"/> can actually build the graph the
    /// application asks for at startup. A missing registration compiles cleanly and only
    /// surfaces when App.Application_Startup runs, which is to say: in front of a user.
    ///
    /// The whole fixture runs on an STA thread because navigation returns WPF Pages, and
    /// creating any DispatcherObject off an STA thread throws.
    /// </summary>
    [TestFixture]
    [Apartment(ApartmentState.STA)]
    public class BootstrapperTests
    {
        private IContainer _container;
        private Exception _bootstrapFailure;

        [OneTimeSetUp]
        public void BootstrapOnce()
        {
            // MainWindowViewModel's constructor generates 1000 Bogus records, so the container is
            // built exactly once for the fixture rather than once per test.
            try
            {
                _container = new Bootstrapper().Bootstrap();
            }
            catch (Exception ex)
            {
                // Captured rather than allowed to escape: an exception out of [OneTimeSetUp] aborts
                // every test in the fixture with the same error, which hides which parts of the
                // graph are sound. Bootstrap_ReturnsAContainer asserts on this directly instead.
                _bootstrapFailure = ex;
            }
        }

        [OneTimeTearDown]
        public void DisposeContainer()
        {
            if (_container != null)
            {
                _container.Dispose();
            }
        }

        /// <summary>
        /// Container-backed tests go through this so that, if bootstrapping fails, they report as
        /// ignored behind the one real failure rather than adding noise to it.
        /// </summary>
        private IContainer RequireContainer()
        {
            if (_container == null)
            {
                Assert.Ignore(
                    "Bootstrap() did not return a container, so the object graph cannot be exercised: " +
                    (_bootstrapFailure == null ? "no exception recorded" : _bootstrapFailure.ToString()));
            }

            return _container;
        }

        #region Bootstrapping

        [Test]
        public void Bootstrap_ReturnsAContainer()
        {
            // This is the assertion that stands in for launching the application: if Bootstrap()
            // cannot complete, App.Application_Startup cannot either.
            Assert.That(
                _bootstrapFailure,
                Is.Null,
                "Bootstrap() threw: " + (_bootstrapFailure == null ? string.Empty : _bootstrapFailure.ToString()));
            Assert.That(_container, Is.Not.Null);
        }

        #endregion

        #region Registered types resolve

        [Test]
        public void Resolve_MainWindowViewModel_Succeeds()
        {
            var container = RequireContainer();

            Assert.That(container.Resolve<MainWindowViewModel>(), Is.Not.Null);
        }

        [Test]
        public void Resolve_MainHomePageViewModel_Succeeds()
        {
            var container = RequireContainer();

            Assert.That(container.Resolve<MainHomePageViewModel>(), Is.Not.Null);
        }

        [Test]
        public void Resolve_InventoryHomePageViewModel_Succeeds()
        {
            var container = RequireContainer();

            Assert.That(container.Resolve<InventoryHomePageViewModel>(), Is.Not.Null);
        }

        [Test]
        public void Resolve_NavigationService_Succeeds()
        {
            var container = RequireContainer();

            Assert.That(container.Resolve<INavigationService>(), Is.Not.Null);
        }

        [Test]
        public void Resolve_NavigationService_ReturnsTheConcreteNavigationService()
        {
            var container = RequireContainer();

            // The registration is RegisterType<NavigationService>().As<INavigationService>(), so the
            // interface is the only service exposed — the concrete type must arrive behind it.
            Assert.That(container.Resolve<INavigationService>(), Is.InstanceOf<NavigationService>());
        }

        [Test]
        public void NavigationService_IsRegisteredAsASingleton()
        {
            var container = RequireContainer();

            // SingleInstance() in the bootstrapper is load-bearing: Bootstrap() hands one instance to
            // NavigationConfiguration.RegisterPages, and MainWindow resolves INavigationService for
            // itself. A per-dependency registration would give MainWindow an empty page registry.
            Assert.That(container.Resolve<INavigationService>(), Is.SameAs(container.Resolve<INavigationService>()));
        }

        [Test]
        public void ViewModels_AreRegisteredPerDependency()
        {
            var container = RequireContainer();

            // No lifetime is specified for the view models, so Autofac's default (a new instance per
            // resolve) applies. Pinned because navigating twice to a page should not reuse state.
            Assert.That(
                container.Resolve<MainHomePageViewModel>(),
                Is.Not.SameAs(container.Resolve<MainHomePageViewModel>()));
        }

        [Test]
        public void MainWindow_IsRegistered()
        {
            var container = RequireContainer();

            // Registration only, deliberately: MainWindow is a MetroWindow whose XAML pulls the
            // MahApps/MaterialDesign resource dictionaries declared in App.xaml, so resolving it
            // without a running Application throws for reasons unrelated to the container. What is
            // worth pinning here is that App.Application_Startup's Resolve<MainWindow>() would find
            // a registration at all, and that its dependencies (MainWindowViewModel and
            // INavigationService) are themselves resolvable — covered by the tests above.
            Assert.That(container.IsRegistered<MainWindow>(), Is.True);
        }

        #endregion

        #region Navigation through the bootstrapped container

        [TestCase("Home")]
        [TestCase("Inventory")]
        [TestCase("Settings")]
        [TestCase("Sales")]
        [TestCase("About")]
        public void BootstrappedNavigationService_NavigatesToEachDocumentedKey(string pageKey)
        {
            var container = RequireContainer();
            var navigationService = container.Resolve<INavigationService>();

            // End to end: Bootstrap() registered the pages on this very instance, so a Page coming
            // back proves container registration and navigation configuration agree.
            Assert.That(navigationService.NavigateTo(pageKey), Is.Not.Null);
        }

        [Test]
        public void BootstrappedNavigationService_UnknownKey_ThrowsInvalidOperationException()
        {
            var container = RequireContainer();
            var navigationService = container.Resolve<INavigationService>();

            // MainWindow catches InvalidOperationException around NavigateTo, so this type is the
            // contract for "no such menu item", not an implementation detail.
            Assert.That(
                () => navigationService.NavigateTo("NoSuchPage"),
                Throws.TypeOf<InvalidOperationException>());
        }

        [Test]
        public void BootstrappedNavigationService_HomePage_GetsContainerResolvedViewModel()
        {
            var container = RequireContainer();
            var navigationService = container.Resolve<INavigationService>();

            var page = navigationService.NavigateTo("Home");

            Assert.That(page, Is.InstanceOf<MainHomePage>());
            Assert.That(page.DataContext, Is.InstanceOf<MainHomePageViewModel>());
        }

        [Test]
        public void BootstrappedNavigationService_InventoryPage_GetsContainerResolvedViewModel()
        {
            var container = RequireContainer();
            var navigationService = container.Resolve<INavigationService>();

            var page = navigationService.NavigateTo("Inventory");

            Assert.That(page, Is.InstanceOf<InventoryHomePage>());
            Assert.That(page.DataContext, Is.InstanceOf<InventoryHomePageViewModel>());
        }

        #endregion

        #region NavigationConfiguration registrations

        /// <summary>
        /// NavigationConfiguration is exercised on its own service instance here so that these
        /// assertions hold independently of whether the container can be built.
        /// </summary>
        private static INavigationService ConfiguredNavigationService()
        {
            var navigationService = new NavigationService(null);
            NavigationConfiguration.RegisterPages(navigationService);
            return navigationService;
        }

        [TestCase("Home", typeof(MainHomePage))]
        [TestCase("Inventory", typeof(InventoryHomePage))]
        [TestCase("Settings", typeof(SettingsHomePage))]
        [TestCase("Sales", typeof(SalesHomePage))]
        [TestCase("About", typeof(AboutPage))]
        public void RegisterPages_MapsEachKeyToItsPageType(string pageKey, Type expectedPageType)
        {
            var navigationService = ConfiguredNavigationService();

            // "Sales" has no menu item in MainWindow.xaml yet (issue #8), but the mapping is part of
            // the configuration's contract and is asserted alongside the reachable ones.
            Assert.That(navigationService.NavigateTo(pageKey), Is.InstanceOf(expectedPageType));
        }

        [TestCase("Home", typeof(MainHomePageViewModel))]
        [TestCase("Inventory", typeof(InventoryHomePageViewModel))]
        public void RegisterPages_PagesWithAViewModel_BindItToTheDataContext(string pageKey, Type expectedViewModelType)
        {
            var navigationService = ConfiguredNavigationService();

            Assert.That(navigationService.NavigateTo(pageKey).DataContext, Is.InstanceOf(expectedViewModelType));
        }

        [TestCase("Settings")]
        [TestCase("Sales")]
        [TestCase("About")]
        public void RegisterPages_PagesWithoutAViewModel_LeaveTheDataContextNull(string pageKey)
        {
            var navigationService = ConfiguredNavigationService();

            Assert.That(navigationService.NavigateTo(pageKey).DataContext, Is.Null);
        }

        [Test]
        public void RegisterPages_DoesNotRegisterAnUnknownKey()
        {
            var navigationService = ConfiguredNavigationService();

            Assert.That(
                () => navigationService.NavigateTo("NoSuchPage"),
                Throws.TypeOf<InvalidOperationException>());
        }

        #endregion
    }
}
