using AutoLotManager.Core;
using Bogus;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace AutoLotManager.ViewModel.Pages.Inventory
{
    public class ExportInventoryListPageViewModel : ViewModelBase
    {
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
