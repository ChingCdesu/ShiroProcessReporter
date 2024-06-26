﻿using H.NotifyIcon;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml;
using ShiroProcessReporter.Services;
using ShiroProcessReporter.Components;
using ShiroProcessReporter.Helper;
using System;

namespace ShiroProcessReporter
{
    public partial class App : Application
    {
        public static Window? MainWindow { get; private set; }

        public static IServiceProvider? ServiceProvider { get; private set; }

        public App()
        {
            this.InitializeComponent();

            AppLogHelper.ConfigureLogging();
        }

        private void ConfigureService(IServiceCollection services)
        {
            services.AddSingleton<MediaTraceService>();
            services.AddSingleton<ProcessTraceService>();
            services.AddSingleton<ReportService>();
            services.AddSingleton<TraceWorkerService>();
        }

        protected override void OnLaunched(LaunchActivatedEventArgs args)
        {
            Preferences.Initialize();
            var services = new ServiceCollection();
            ConfigureService(services);
            ServiceProvider = services.BuildServiceProvider();

            MainWindow = new MainView();

            MainWindow.Closed += (sender, args) =>
            {
                args.Handled = true;
                MainWindow.Hide();
            };

            MainWindow.Activate();
        }
    }
}
