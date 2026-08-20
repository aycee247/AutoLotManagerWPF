using System;
using System.Collections.Generic;
using AutoLotManager.Core.Navigation;
using NUnit.Framework;

namespace AutoLotManager.Tests
{
    /// <summary>
    /// PageNavigator is the seam that lets a netstandard2.0 view model ask for navigation
    /// without referencing WPF. It validates the key and relays the request; the shell does
    /// the actual work.
    /// </summary>
    [TestFixture]
    public class PageNavigatorTests
    {
        [Test]
        public void NavigateTo_RaisesNavigationRequestedWithThePageKey()
        {
            var navigator = new PageNavigator();
            var received = new List<string>();
            navigator.NavigationRequested += (s, e) => received.Add(e.PageKey);

            navigator.NavigateTo("Inventory");

            Assert.That(received, Is.EqualTo(new[] { "Inventory" }));
        }

        [Test]
        public void NavigateTo_PassesTheNavigatorAsSender()
        {
            var navigator = new PageNavigator();
            object sender = null;
            navigator.NavigationRequested += (s, e) => sender = s;

            navigator.NavigateTo("Inventory");

            Assert.That(sender, Is.SameAs(navigator));
        }

        [Test]
        public void NavigateTo_WithNoSubscriber_DoesNotThrow()
        {
            // The shell subscribes at construction, but a view model may request navigation
            // in a test or before the window exists. That must not fault.
            var navigator = new PageNavigator();

            Assert.That(() => navigator.NavigateTo("Inventory"), Throws.Nothing);
        }

        [Test]
        public void NavigateTo_NullKey_ThrowsArgumentNullException()
        {
            var navigator = new PageNavigator();

            Assert.That(() => navigator.NavigateTo(null), Throws.TypeOf<ArgumentNullException>());
        }

        // Empty and whitespace keys are not null, so they must not surface as
        // ArgumentNullException. Throws.TypeOf is an exact match, which is the point:
        // ArgumentNullException derives from ArgumentException and would satisfy a looser
        // assertion. This mirrors NavigationService's contract deliberately.
        [TestCase("")]
        [TestCase("   ")]
        [TestCase("\t")]
        public void NavigateTo_EmptyOrWhitespaceKey_ThrowsArgumentExceptionNotArgumentNull(string pageKey)
        {
            var navigator = new PageNavigator();

            Assert.That(() => navigator.NavigateTo(pageKey), Throws.TypeOf<ArgumentException>());
        }

        [Test]
        public void NavigateTo_InvalidKey_DoesNotRaiseTheEvent()
        {
            var navigator = new PageNavigator();
            var raised = false;
            navigator.NavigationRequested += (s, e) => raised = true;

            Assert.That(() => navigator.NavigateTo(""), Throws.TypeOf<ArgumentException>());
            Assert.That(raised, Is.False, "validation must happen before the request is relayed");
        }
    }
}
