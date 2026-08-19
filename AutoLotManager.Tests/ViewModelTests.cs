using System.Collections.Generic;
using System.Linq;
using AutoLotManager.ViewModel;
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
        public void Constructor_PopulatesCarsWithGeneratedInventory()
        {
            var viewModel = new MainWindowViewModel();

            Assert.That(viewModel.Cars, Is.Not.Null);
            Assert.That(viewModel.Cars, Is.Not.Empty);
        }

        [Test]
        public void Constructor_GeneratesCarsWithPopulatedFields()
        {
            var viewModel = new MainWindowViewModel();

            var car = viewModel.Cars.First();

            Assert.Multiple(() =>
            {
                Assert.That(car.Vin, Is.Not.Null.And.Not.Empty);
                Assert.That(car.Make, Is.Not.Null.And.Not.Empty);
                Assert.That(car.Model, Is.Not.Null.And.Not.Empty);
                Assert.That(car.Color, Is.Not.Null.And.Not.Empty);
                Assert.That(car.Year, Is.InRange(1980, 2024));
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
}
