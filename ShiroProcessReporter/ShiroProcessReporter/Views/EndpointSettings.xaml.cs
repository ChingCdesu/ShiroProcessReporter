using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using ShiroProcessReporter.Services;
using ShiroProcessReporter.Components;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using ShiroProcessReporter.Helper;

namespace ShiroProcessReporter.Views
{
    [ObservableObject]
    public sealed partial class EndpointSettings : Page
    {
        private readonly ReportService _reportService;

        public EndpointSettings()
        {
            this.InitializeComponent();
            _reportService = App.ServiceProvider!.GetService<ReportService>()!;
        }

        [RelayCommand]
        private void UpdateEndpoint()
        {
            var context = EndpointEditDialog.DataContext as DataContextWrapper<string>;
            _reportService!.Endpoint = context.Value;
        }

        [RelayCommand]
        private void UpdateApiKey()
        {
            var context = EndpointEditDialog.DataContext as DataContextWrapper<string>;
            _reportService!.ApiKey = context.Value;
        }

        private async void EditEndpoint(object sender, RoutedEventArgs e)
        {
            EndpointEditDialog.DataContext = new DataContextWrapper<string>(_reportService!.Endpoint);
            EndpointEditDialog.Title = "Edit Endpoint";
            EndpointEditDialog.PrimaryButtonCommand = UpdateEndpointCommand;
            await EndpointEditDialog.ShowAsync();
        }

        private async void EditApiKey(object sender, RoutedEventArgs e)
        {
            EndpointEditDialog.DataContext = new DataContextWrapper<string>(_reportService!.ApiKey);
            EndpointEditDialog.Title = "Edit API Key";
            EndpointEditDialog.PrimaryButtonCommand = UpdateApiKeyCommand;
            await EndpointEditDialog.ShowAsync();
        }
    }
}
