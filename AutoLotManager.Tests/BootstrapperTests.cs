using System;
using System.Threading;
using Autofac;
using AutoLotManager.Core.Navigation;
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
    /// Every other test in this project constructs its subject by hand, so until now nothing
    /// proved that <see cref="Bootstrapper.Bootstrap"/> can build the graph the application asks
    /// for at startup. A missing registration compiles cleanly and only surfaces when
    /// App.Application_Startup runs — which is to say, in front of a user.
    ///
    /// These were written against a live defect: NavigationService took an Autofac.IContainer, which
    /// Autofac never self-registers, so Bootstrap() threw DependencyResolutionException and the
    /// application died on startup. The constructor now takes ILifetimeScope, which Autofac does
    /// self-register, and these tests are the regression guard — widen that parameter back to a type
    /// the container cannot supply and every test in this fixture fails.
    ///
    /// The fixture runs on an STA thread because navigation constructs WPF Pages, and creating a
    /// DispatcherObject off an STA thread throws.
    /// </summary>
    [TestFixture]
    [Apartment(ApartmentState.STA)]
    public class BootstrapperTests
    {
        private IContainer _container;

        [OneTimeSetUp]
        public void BootstrapOnce()
        {
            // One container for the fixture: Bootstrap() is itself under test and there is no reason
            // to repeat it per test, and sharing the result keeps the expensive resolves down to one
            // each. (MainWindowViewModel's 1000-record Bogus generation was removed in #68,
            // but resolving the whole graph is still worth doing once rather than per test.)
            //
            // Deliberately not wrapped in a try/catch. If Bootstrap() throws, every test in the
            // fixture should fail with that exception: the object graph not building is precisely
            // the failure this fixture exists to catch.
            _container = new Bootstrapper().Bootstrap();
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
        /// Runs the navigation and hands back whatever it threw, or null. Used where the only thing
        /// that can be asserted is <em>which</em> failure occurred, because the page itself cannot be
        /// constructed in a test host.
        /// </summary>
        private static Exception CaptureNavigationFailure(INavigationService navigationService, string pageKey)
        {
            try
            {
                navigationService.NavigateTo(pageKey);
                return null;
            }
            catch (Exception ex)
            {
                return ex;
            }
        }

        #region Bootstrapping

        /// <summary>
        /// The regression guard for the startup crash. NavigationService's constructor must take a
        /// service Autofac actually supplies: ContainerBuilder.Build adds exactly one
        /// self-registration, exposing ILifetimeScope and IComponentContext and nothing else. When it
        /// took IContainer, this threw DependencyResolutionException ("Cannot resolve parameter
        /// 'Autofac.IContainer container'") and App.Application_Startup died on that line before
        /// MainWindow was ever shown.
        /// </summary>
        [Test]
        public void Bootstrap_BuildsTheObjectGraphWithoutThrowing()
        {
            Assert.That(_container, Is.Not.Null);
        }

        #endregion

        #region Registered types resolve

        [Test]
        public void Resolve_MainWindowViewModel_Succeeds()
        {
            var container = _container;

            Assert.That(container.Resolve<MainWindowViewModel>(), Is.Not.Null);
        }

        [Test]
        public void Resolve_MainHomePageViewModel_Succeeds()
        {
            var container = _container;

            Assert.That(container.Resolve<MainHomePageViewModel>(), Is.Not.Null);
        }

        [Test]
        public void Resolve_InventoryHomePageViewModel_Succeeds()
        {
            var container = _container;

            Assert.That(container.Resolve<InventoryHomePageViewModel>(), Is.Not.Null);
        }

        [Test]
        public void Resolve_PageNavigator_Succeeds()
        {
            var container = _container;

            // The view-model-facing navigation seam. Registered as a singleton because the
            // view models raising requests and MainWindow handling them must share one
            // instance — a per-dependency registration would mean the shell never hears them.
            Assert.That(container.Resolve<IPageNavigator>(), Is.Not.Null);
        }

        [Test]
        public void PageNavigator_IsRegisteredAsASingleton()
        {
            var container = _container;

            Assert.That(container.Resolve<IPageNavigator>(), Is.SameAs(container.Resolve<IPageNavigator>()));
        }

        [Test]
        public void Resolve_ExportInventoryListPageViewModel_Succeeds()
        {
            var container = _container;

            Assert.That(container.Resolve<ExportInventoryListPageViewModel>(), Is.Not.Null);
        }

        [Test]
        public void Resolve_NavigationService_ReturnsTheConcreteNavigationService()
        {
            var container = _container;

            // The registration is RegisterType<NavigationService>().As<INavigationService>(), so the
            // interface is the only service exposed and the concrete type must arrive behind it.
            Assert.That(container.Resolve<INavigationService>(), Is.InstanceOf<NavigationService>());
        }

        [Test]
        public void NavigationService_IsRegisteredAsASingleton()
        {
            var container = _container;

            // SingleInstance() in the bootstrapper is load-bearing: Bootstrap() hands one instance to
            // NavigationConfiguration.RegisterPages, and MainWindow resolves INavigationService for
            // itself. A per-dependency registration would give MainWindow an empty page registry and
            // every menu click would report the page as unregistered.
            Assert.That(container.Resolve<INavigationService>(), Is.SameAs(container.Resolve<INavigationService>()));
        }

        [Test]
        public void ViewModels_AreRegisteredPerDependency()
        {
            var container = _container;

            // No lifetime is specified for the view models, so Autofac's default — a new instance per
            // resolve — applies. Pinned because navigating twice to a page must not reuse its state.
            Assert.That(
                container.Resolve<MainHomePageViewModel>(),
                Is.Not.SameAs(container.Resolve<MainHomePageViewModel>()));
        }

        [Test]
        public void MainWindow_IsRegistered()
        {
            var container = _container;

            // Registration only, deliberately. MainWindow is a MetroWindow whose XAML pulls the
            // MahApps/MaterialDesign dictionaries declared in App.xaml, so resolving it without a
            // running Application throws for reasons that have nothing to do with the container. What
            // is worth pinning is that App.Application_Startup's Resolve<MainWindow>() would find a
            // registration at all, and that its three dependencies — MainWindowViewModel,
            // INavigationService and IPageNavigator — are resolvable, which the tests above cover.
            // Keep this list in step with MainWindow's constructor: it is the only guide for
            // keeping the #50 regression guard complete.
            Assert.That(container.IsRegistered<MainWindow>(), Is.True);
        }

        #endregion

        #region Navigation through the bootstrapped container

        [TestCase("Settings")]
        [TestCase("Sales")]
        [TestCase("About")]
        public void BootstrappedNavigationService_NavigatesToEachDocumentedKey(string pageKey)
        {
            var container = _container;
            var navigationService = container.Resolve<INavigationService>();

            // End to end: Bootstrap() registered the pages on this very instance, so a Page coming
            // back proves the container registration and the navigation configuration agree.
            // Only the pages that can be constructed in a test host are listed — see
            // RegisterPages_KeysWhosePagesCannotBeConstructedInATestHost for Home and Inventory.
            Assert.That(navigationService.NavigateTo(pageKey), Is.Not.Null);
        }

        [Test]
        public void BootstrappedNavigationService_UnknownKey_ThrowsInvalidOperationException()
        {
            var container = _container;
            var navigationService = container.Resolve<INavigationService>();

            // MainWindow catches InvalidOperationException around NavigateTo, so this type is the
            // contract for "no such menu item", not an implementation detail.
            Assert.That(
                () => navigationService.NavigateTo("NoSuchPage"),
                Throws.TypeOf<InvalidOperationException>());
        }

        [Test]
        public void BootstrappedNavigationService_ResolvesARegisteredViewModelIntoTheDataContext()
        {
            var container = _container;
            var navigationService = container.Resolve<INavigationService>();

            // The container-backed half of navigation: with a container present, NavigateTo resolves
            // a registered view model type through it rather than falling back to
            // Activator.CreateInstance. MainHomePageViewModel is a real container registration; the
            // page is a double only because MainHomePage's XAML cannot load here, and the page type
            // is irrelevant to the branch being exercised. A distinct key keeps the fixture's shared
            // singleton navigation service free of collisions with the real registrations.
            navigationService.RegisterPage("ContainerResolutionProbe", typeof(TestPage), typeof(MainHomePageViewModel));

            var page = navigationService.NavigateTo("ContainerResolutionProbe");

            Assert.That(page, Is.InstanceOf<TestPage>());
            Assert.That(page.DataContext, Is.InstanceOf<MainHomePageViewModel>());
        }

        #endregion

        #region NavigationConfiguration registrations

        /// <summary>
        /// NavigationConfiguration is exercised on its own service instance here so these assertions
        /// hold whether or not the container can be built.
        /// </summary>
        private static INavigationService ConfiguredNavigationService()
        {
            var navigationService = new NavigationService(null);
            NavigationConfiguration.RegisterPages(navigationService);
            return navigationService;
        }

        [TestCase("Settings", typeof(SettingsHomePage))]
        [TestCase("Sales", typeof(SalesHomePage))]
        [TestCase("About", typeof(AboutPage))]
        public void RegisterPages_MapsEachKeyToItsPageType(string pageKey, Type expectedPageType)
        {
            var navigationService = ConfiguredNavigationService();

            // "Sales" has no menu item in MainWindow.xaml yet (issue #8), but the mapping is part of
            // this configuration's contract and is asserted alongside the reachable ones.
            Assert.That(navigationService.NavigateTo(pageKey), Is.InstanceOf(expectedPageType));
        }

        [TestCase("Home")]
        [TestCase("Inventory")]
        // Added in #65: reached from the inventory page's tiles rather than the menu. Like
        // Home and Inventory these are real XAML pages, so the honest assertion is that the
        // key resolves to *something* — an unregistered key is refused with
        // InvalidOperationException before the page type is touched at all.
        [TestCase("AddEditInventory")]
        [TestCase("ExportInventoryList")]
        public void RegisterPages_KeysWhosePagesCannotBeConstructedInATestHost(string pageKey)
        {
            var navigationService = ConfiguredNavigationService();

            // MainHomePage and InventoryHomePage cannot be constructed outside the running
            // application, so the strongest honest assertion is that the key is registered.
            // Measured on the CI runner:
            //   Home      -> XamlParseException, "Cannot find resource named 'MahApps.Brushes.Accent4'"
            //                (that key lives in the merged dictionaries declared in App.xaml, and
            //                Application.Current is null in a test host)
            //   Inventory -> FileNotFoundException for Syncfusion.SfSkinManager.WPF, which the Desktop
            //                project's packages.config pulls in but which does not reach the test
            //                assembly's probing path
            // Both arrive wrapped in TargetInvocationException from Activator.CreateInstance, i.e.
            // from *constructing* the page. An unregistered key never gets that far: NavigateTo
            // rejects it with InvalidOperationException before touching the page type at all, so
            // anything other than InvalidOperationException means the lookup succeeded.
            Assert.That(
                CaptureNavigationFailure(navigationService, pageKey),
                Is.Not.InstanceOf<InvalidOperationException>());
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
