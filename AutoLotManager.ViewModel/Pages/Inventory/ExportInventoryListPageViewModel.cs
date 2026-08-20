using AutoLotManager.Core;
using Bogus;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace AutoLotManager.ViewModel.Pages.Inventory
{
    /// <summary>
    /// View model behind the "export inventory list" page. It holds the list of
    /// vehicles the page is meant to display and export; the export action
    /// itself is not implemented here.
    /// </summary>
    public class ExportInventoryListPageViewModel : ViewModelBase
    {
        /// <summary>
        /// How many placeholder vehicles the constructor generates.
        /// </summary>
        /// <remarks>
        /// Was 1000 while nothing constructed this view model. Now that the export page is
        /// registered, this runs on the UI thread on every navigation to it — view models are
        /// registered per-dependency, so the cost is paid per visit, not once. Kept small
        /// until issue #74 replaces the generation with seed data from the database.
        /// </remarks>
        private const int SeedRecordCount = 50;

        /// <summary>
        /// Creates the view model and fills <see cref="Cars"/>.
        /// </summary>
        /// <remarks>
        /// No real inventory source is wired up yet, so the constructor generates
        /// <see cref="SeedRecordCount"/> fake <see cref="Car"/> records with Bogus. That work
        /// runs synchronously on the constructing thread — the UI thread, in the running
        /// application — and produces random placeholder data unrelated to any other view
        /// model's. Since issue #67 the export page is registered with the navigation service
        /// and this view model is resolved from the container, so the cost is paid on every
        /// navigation to that page. Issue #74 replaces it with seed data.
        /// </remarks>
        public ExportInventoryListPageViewModel()
        {
            Cars = new ObservableCollection<Car>();
            var cars = new Faker<Car>()
                .RuleFor(c => c.Vin, a => a.Vehicle.Vin())
                .RuleFor(c => c.Make, a => a.Vehicle.Manufacturer())
                .RuleFor(c => c.Model, a => a.Vehicle.Model())
                .RuleFor(c => c.Year, a => a.Random.Number(1980, 2024))
                .RuleFor(c => c.Color, a => a.Commerce.Color());

            for (int i = 0; i < SeedRecordCount; i++)
            {
                var car = cars.Generate();
                Cars.Add(car);
            };
        }

        private ObservableCollection<Car> _cars;

        /// <summary>
        /// The vehicles the export page lists. Populated in the constructor with
        /// <see cref="SeedRecordCount"/> randomly generated placeholder records; replacing the
        /// whole collection raises a change notification so the view rebinds. The page's grid
        /// binds to this name, and since issue #67 the navigation service supplies this view
        /// model as the page's DataContext, so the binding resolves.
        /// </summary>
        public ObservableCollection<Car> Cars
        {
            get
            {
                return _cars;
            }
            set
            {
                _cars = value;
                OnPropertyChanged();
            }
        }
    }
}
