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
        /// Creates the view model and fills <see cref="Cars"/>.
        /// </summary>
        /// <remarks>
        /// No real inventory source is wired up yet, so the constructor
        /// generates 1000 fake <see cref="Car"/> records with Bogus. That work
        /// runs synchronously on the constructing thread and produces random
        /// placeholder data unrelated to the records held by
        /// <see cref="AutoLotManager.ViewModel.MainWindowViewModel"/>.
        /// Nothing constructs this view model at present: the export page is
        /// not registered with the navigation service and its code-behind never
        /// sets a DataContext, so the cost is not currently paid at runtime.
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

            for (int i = 0; i < 1000; i++)
            {
                var car = cars.Generate();
                Cars.Add(car);
            };
        }

        private ObservableCollection<Car> _cars;

        /// <summary>
        /// The vehicles the export page is meant to list. Populated in the
        /// constructor with 1000 randomly generated placeholder records;
        /// replacing the whole collection raises a change notification so the
        /// view rebinds. The page's grid binds to this name, but the binding is
        /// inert until something makes this view model the page's DataContext.
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
