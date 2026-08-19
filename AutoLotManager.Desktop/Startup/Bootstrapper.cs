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

            // Configure navigation registrations. If anything here throws, the container built above
            // would otherwise be abandoned without being disposed, leaking whatever it had already
            // created — so dispose it before letting the exception reach Application_Startup.
            try
            {
                var navigationService = container.Resolve<INavigationService>();
                NavigationConfiguration.RegisterPages(navigationService);
            }
            catch
            {
                container.Dispose();
                throw;
            }

            return container;
        }
    }
}
