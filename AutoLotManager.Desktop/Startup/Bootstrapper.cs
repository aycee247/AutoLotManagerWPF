using Autofac;
using AutoLotManager.ViewModel;
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
            builder.RegisterType<MainWindow>().AsSelf();
            builder.RegisterType<MainWindowViewModel>().AsSelf();

            return builder.Build();
        }
    }
}
