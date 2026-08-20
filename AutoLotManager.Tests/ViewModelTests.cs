using System;
using System.Collections.Generic;
using AutoLotManager.Core.Navigation;
using AutoLotManager.ViewModel;
using AutoLotManager.ViewModel.Pages.Inventory;
using NUnit.Framework;

namespace AutoLotManager.Tests
{
    [TestFixture]
    public class ViewModelBaseTests
    {
        private class TestableViewModel : ViewModelBase
        {
            private string _value;

            public string Value
            {
                get { return _value; }
                set
                {
                    _value = value;
                    OnPropertyChanged();
                }
            }
        }

        [Test]
        public void OnPropertyChanged_UsesCallerMemberNameAsPropertyName()
        {
            var viewModel = new TestableViewModel();
            var raised = new List<string>();
            viewModel.PropertyChanged += (s, e) => raised.Add(e.PropertyName);

            viewModel.Value = "anything";

            // [CallerMemberName] should supply "Value" without the setter naming it.
            Assert.That(raised, Is.EqualTo(new[] { "Value" }));
        }

        [Test]
        public void OnPropertyChanged_PassesTheViewModelAsSender()
        {
            var viewModel = new TestableViewModel();
            object sender = null;
            viewModel.PropertyChanged += (s, e) => sender = s;

            viewModel.Value = "anything";

            Assert.That(sender, Is.SameAs(viewModel));
        }

        [Test]
        public void OnPropertyChanged_WithNoSubscribers_DoesNotThrow()
        {
            var viewModel = new TestableViewModel();

            Assert.That(() => viewModel.Value = "anything", Throws.Nothing);
        }
    }

    [TestFixture]
    public class MainWindowViewModelTests
    {
        // Each test constructs its own view model. This fixture previously shared one
        // instance created in [OneTimeSetUp] because the constructor generated 1000 Bogus
        // Car records and doing that seven times was wasteful. That generation was removed
        // in #68 — nothing displayed it — so construction is cheap again and the shared
        // instance, along with its order-dependence hazard, is no longer needed.

        [Test]
        public void Constructor_WiresUpAllCommands()
        {
            var viewModel = new MainWindowViewModel();

            Assert.Multiple(() =>
            {
                Assert.That(viewModel.WindowLoadedCommand, Is.Not.Null);
                Assert.That(viewModel.ProgressTileClickedCommand, Is.Not.Null);
                Assert.That(viewModel.GithubIconClickedCommand, Is.Not.Null);
            });
        }

        [Test]
        public void WindowLoaded_SetsTheWindowTitle()
        {
            var viewModel = new MainWindowViewModel();
            Assert.That(viewModel.WindowTitle, Is.Null, "precondition: title starts unset");

            viewModel.WindowLoaded();

            Assert.That(viewModel.WindowTitle, Is.Not.Null.And.Not.Empty);
        }

        [Test]
        public void ProgressTileClickedCommand_TogglesDisplayProgressRing()
        {
            var viewModel = new MainWindowViewModel();
            Assert.That(viewModel.DisplayProgressRing, Is.False, "precondition: ring starts hidden");

            viewModel.ProgressTileClickedCommand.Execute(null);
            Assert.That(viewModel.DisplayProgressRing, Is.True);

            viewModel.ProgressTileClickedCommand.Execute(null);
            Assert.That(viewModel.DisplayProgressRing, Is.False);
        }

        [Test]
        public void SettingWindowTitle_RaisesPropertyChangedForThatProperty()
        {
            var viewModel = new MainWindowViewModel();
            var raised = new List<string>();
            viewModel.PropertyChanged += (s, e) => raised.Add(e.PropertyName);

            viewModel.WindowTitle = "new title";

            Assert.That(raised, Does.Contain("WindowTitle"));
        }

        [Test]
        public void SettingSelectedMenuItem_RaisesPropertyChangedForThatProperty()
        {
            var viewModel = new MainWindowViewModel();
            var raised = new List<string>();
            viewModel.PropertyChanged += (s, e) => raised.Add(e.PropertyName);

            viewModel.SelectedMenuItem = "Inventory";

            Assert.That(raised, Does.Contain("SelectedMenuItem"));
        }
    }

    /// <summary>
    /// The home page's tiles bind to ProgressTileClickedCommand. Before #65 that command
    /// existed only on MainWindowViewModel, so the binding stopped resolving the moment the
    /// navigation service assigned this view model as the page's DataContext and clicking a
    /// tile silently did nothing. These tests pin the command to the view model the page
    /// actually receives.
    /// </summary>
    [TestFixture]
    public class MainHomePageViewModelTests
    {
        [Test]
        public void Constructor_ExposesTheCommandTheHomePageBindsTo()
        {
            var viewModel = new MainHomePageViewModel();

            Assert.That(viewModel.ProgressTileClickedCommand, Is.Not.Null);
        }

        [Test]
        public void ProgressTileClickedCommand_TogglesDisplayProgressRing()
        {
            var viewModel = new MainHomePageViewModel();
            Assert.That(viewModel.DisplayProgressRing, Is.False, "precondition: ring starts hidden");

            viewModel.ProgressTileClickedCommand.Execute(null);
            Assert.That(viewModel.DisplayProgressRing, Is.True);

            viewModel.ProgressTileClickedCommand.Execute(null);
            Assert.That(viewModel.DisplayProgressRing, Is.False);
        }

        [Test]
        public void SettingDisplayProgressRing_RaisesPropertyChanged()
        {
            var viewModel = new MainHomePageViewModel();
            var raised = new List<string>();
            viewModel.PropertyChanged += (s, e) => raised.Add(e.PropertyName);

            viewModel.DisplayProgressRing = true;

            Assert.That(raised, Does.Contain("DisplayProgressRing"));
        }
    }

    /// <summary>
    /// The inventory page's two tiles bound to commands that existed on no view model at
    /// all, so both clicks did nothing. These tests pin the commands and the page keys they
    /// navigate to.
    /// </summary>
    [TestFixture]
    public class InventoryHomePageViewModelTests
    {
        private sealed class RecordingPageNavigator : IPageNavigator
        {
            public List<string> Requested { get; } = new List<string>();

            public event EventHandler<PageNavigationEventArgs> NavigationRequested;

            public void NavigateTo(string pageKey)
            {
                Requested.Add(pageKey);
                NavigationRequested?.Invoke(this, new PageNavigationEventArgs(pageKey));
            }
        }

        [Test]
        public void Constructor_ExposesBothCommandsTheInventoryPageBindsTo()
        {
            var viewModel = new InventoryHomePageViewModel(new RecordingPageNavigator());

            Assert.Multiple(() =>
            {
                Assert.That(viewModel.OpenAddEditInventoryPageCommand, Is.Not.Null);
                Assert.That(viewModel.OpenExportInventoryListCommand, Is.Not.Null);
            });
        }

        [Test]
        public void Constructor_NullNavigator_ThrowsArgumentNullException()
        {
            // A null navigator would restore the original defect in a harder-to-find form:
            // the command would exist and then fail at click time.
            Assert.That(
                () => new InventoryHomePageViewModel(null),
                Throws.TypeOf<ArgumentNullException>());
        }

        [Test]
        public void OpenAddEditInventoryPageCommand_RequestsTheAddEditPage()
        {
            var navigator = new RecordingPageNavigator();
            var viewModel = new InventoryHomePageViewModel(navigator);

            viewModel.OpenAddEditInventoryPageCommand.Execute(null);

            Assert.That(navigator.Requested, Is.EqualTo(new[] { "AddEditInventory" }));
        }

        [Test]
        public void OpenExportInventoryListCommand_RequestsTheExportPage()
        {
            var navigator = new RecordingPageNavigator();
            var viewModel = new InventoryHomePageViewModel(navigator);

            viewModel.OpenExportInventoryListCommand.Execute(null);

            Assert.That(navigator.Requested, Is.EqualTo(new[] { "ExportInventoryList" }));
        }

        // The keys the commands use must match the ones NavigationConfiguration registers.
        // They are constants precisely so this cannot drift silently.
        [Test]
        public void PageKeyConstants_MatchTheRegisteredKeys()
        {
            Assert.Multiple(() =>
            {
                Assert.That(InventoryHomePageViewModel.AddEditInventoryPageKey, Is.EqualTo("AddEditInventory"));
                Assert.That(InventoryHomePageViewModel.ExportInventoryListPageKey, Is.EqualTo("ExportInventoryList"));
            });
        }
    }
}
