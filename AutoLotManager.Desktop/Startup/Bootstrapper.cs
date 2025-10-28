using Autofac;
using AutoLotManager.Desktop.Navigation;
using AutoLotManager.ViewModel;
using AutoLotManager.ViewModel.Pages.Inventory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace AutoLotManager.Desktop.Startup
{
    public class Bootstrapper : Autofac.Module
    {
        public IContainer Bootstrap()
        {
            var builder = new ContainerBuilder();
            
            // Register main window and view model
            builder.RegisterType<MainWindow>().AsSelf();
            builder.RegisterType<MainWindowViewModel>().AsSelf();
            
            // Register view models
            builder.RegisterType<MainHomePageViewModel>().AsSelf();
            builder.RegisterType<InventoryHomePageViewModel>().AsSelf();
            
            // Register navigation service
            builder.RegisterType<NavigationService>().As<INavigationService>().SingleInstance();

            var container = builder.Build();
            
            // Configure navigation registrations
            var navigationService = container.Resolve<INavigationService>();
            NavigationConfiguration.RegisterPages(navigationService);

            return container;
        }
    }
}
