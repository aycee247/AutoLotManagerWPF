﻿using Autofac;
using AutoLotManager.Desktop.Startup;
using AutoLotManager.ViewModel;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace AutoLotManager.Desktop
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private void Application_Startup(object sender, StartupEventArgs e)
        {
            Syncfusion.Licensing.SyncfusionLicenseProvider.RegisterLicense("MTMxNjQ2MUAzMjMwMmUzNDJlMzBPOC81RW1sOGQxWUdSRmc1bmRWeE5KTjcrWUxkamJxbHFkcjk5Mk03SmxVPQ==");


            var bootstrapper = new Bootstrapper();
            var container = bootstrapper.Bootstrap();

            var mainWindow = container.Resolve<MainWindow>();
            mainWindow.Show();
        }

    }
}
