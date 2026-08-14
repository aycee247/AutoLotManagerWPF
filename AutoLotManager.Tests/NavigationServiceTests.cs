using System;
using System.Threading;
using System.Windows.Controls;
using AutoLotManager.Desktop.Navigation;
using NUnit.Framework;

namespace AutoLotManager.Tests
{
    /// <summary>
    /// Creating a WPF Page requires an STA thread, so the whole fixture runs on one.
    /// The container is null throughout: NavigationService treats that as "nothing is
    /// registered" and falls back to Activator.CreateInstance, which is the path worth
    /// pinning down here. Container-backed resolution belongs to the integration tests
    /// in issue #18.
    /// </summary>
    [TestFixture]
    [Apartment(ApartmentState.STA)]
    public class NavigationServiceTests
    {
        private NavigationService CreateService()
        {
            return new NavigationService(null);
        }

        #region RegisterPage validation

        [Test]
        public void RegisterPage_NullKey_ThrowsArgumentNullException()
        {
            var service = CreateService();

            Assert.That(
                () => service.RegisterPage(null, typeof(TestPage)),
                Throws.TypeOf<ArgumentNullException>());
        }

        // Empty and whitespace keys are not null, so they must not surface as
        // ArgumentNullException. Throws.TypeOf is an exact match, which is the point:
        // ArgumentNullException derives from ArgumentException and would satisfy a
        // looser assertion.
        [TestCase("")]
        [TestCase("   ")]
        [TestCase("\t")]
        public void RegisterPage_EmptyOrWhitespaceKey_ThrowsArgumentExceptionNotArgumentNull(string pageKey)
        {
            var service = CreateService();

            Assert.That(
                () => service.RegisterPage(pageKey, typeof(TestPage)),
                Throws.TypeOf<ArgumentException>());
        }

        [Test]
        public void RegisterPage_NullPageType_ThrowsArgumentNullException()
        {
            var service = CreateService();

            Assert.That(
                () => service.RegisterPage("Home", null),
                Throws.TypeOf<ArgumentNullException>());
        }

        [Test]
        public void RegisterPage_TypeThatIsNotAPage_ThrowsArgumentException()
        {
            var service = CreateService();

            Assert.That(
                () => service.RegisterPage("Home", typeof(NotAPage)),
                Throws.TypeOf<ArgumentException>());
        }

        #endregion

        #region NavigateTo validation

        [Test]
        public void NavigateTo_NullKey_ThrowsArgumentNullException()
        {
            var service = CreateService();

            Assert.That(
                () => service.NavigateTo(null),
                Throws.TypeOf<ArgumentNullException>());
        }

        [TestCase("")]
        [TestCase("   ")]
        public void NavigateTo_EmptyOrWhitespaceKey_ThrowsArgumentExceptionNotArgumentNull(string pageKey)
        {
            var service = CreateService();

            Assert.That(
                () => service.NavigateTo(pageKey),
                Throws.TypeOf<ArgumentException>());
        }

        [Test]
        public void NavigateTo_UnregisteredKey_ThrowsInvalidOperationException()
        {
            var service = CreateService();

            // MainWindow catches InvalidOperationException specifically for this case,
            // so the exception type is part of the contract, not an implementation detail.
            Assert.That(
                () => service.NavigateTo("NoSuchPage"),
                Throws.TypeOf<InvalidOperationException>());
        }

        #endregion

        #region Navigation behaviour

        [Test]
        public void NavigateTo_RegisteredPage_ReturnsInstanceOfThatPageType()
        {
            var service = CreateService();
            service.RegisterPage("Home", typeof(TestPage));

            var page = service.NavigateTo("Home");

            Assert.That(page, Is.InstanceOf<TestPage>());
        }

        [Test]
        public void NavigateTo_RegisteredPage_ReturnsANewInstanceEachTime()
        {
            var service = CreateService();
            service.RegisterPage("Home", typeof(TestPage));

            var first = service.NavigateTo("Home");
            var second = service.NavigateTo("Home");

            Assert.That(first, Is.Not.SameAs(second));
        }

        [Test]
        public void NavigateTo_PageWithViewModel_SetsDataContextToThatViewModel()
        {
            var service = CreateService();
            service.RegisterPage("Home", typeof(TestPage), typeof(TestViewModel));

            var page = service.NavigateTo("Home");

            Assert.That(page.DataContext, Is.InstanceOf<TestViewModel>());
        }

        [Test]
        public void NavigateTo_PageWithoutViewModel_LeavesDataContextNull()
        {
            var service = CreateService();
            service.RegisterPage("About", typeof(TestPage));

            var page = service.NavigateTo("About");

            Assert.That(page.DataContext, Is.Null);
        }

        // The registry is built with StringComparer.OrdinalIgnoreCase. Menu labels come
        // from XAML, so a casing change there should not silently break navigation.
        [TestCase("inventory")]
        [TestCase("INVENTORY")]
        [TestCase("InVeNtOrY")]
        public void NavigateTo_KeyLookupIsCaseInsensitive(string lookupKey)
        {
            var service = CreateService();
            service.RegisterPage("Inventory", typeof(TestPage));

            var page = service.NavigateTo(lookupKey);

            Assert.That(page, Is.InstanceOf<TestPage>());
        }

        [Test]
        public void RegisterPage_SameKeyTwice_LastRegistrationWins()
        {
            var service = CreateService();
            service.RegisterPage("Home", typeof(TestPage));
            service.RegisterPage("Home", typeof(AnotherTestPage));

            var page = service.NavigateTo("Home");

            Assert.That(page, Is.InstanceOf<AnotherTestPage>());
        }

        #endregion
    }
}
