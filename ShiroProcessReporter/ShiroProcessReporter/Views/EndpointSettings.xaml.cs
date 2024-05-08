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

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace ShiroProcessReporter.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class EndpointSettings : Page
    {
        private readonly ReportService _reportService;

        public EndpointSettings()
        {
            this.InitializeComponent();
            _reportService = App.ServiceProvider!.GetService<ReportService>()!;
        }

        private async void EditEndpoint(object sender, RoutedEventArgs e)
        {
            var content = new OneLineInputDialog();
            content.Input.Text = _reportService?.Endpoint;

            var dialog = new ContentDialog
            {
                XamlRoot = this.XamlRoot,
                Style = Application.Current.Resources["DefaultContentDialogStyle"] as Style,
                Width = 400,
                Title = "Edit Endpoint",
                PrimaryButtonText = "Save",
                SecondaryButtonText = "Cancel",
                DefaultButton = ContentDialogButton.Primary,
                Content = content
            };

            var result = await dialog.ShowAsync();
            if (result.Equals(ContentDialogResult.Primary))
            {
                _reportService!.Endpoint = content.Input.Text;
            }
        }

        private async void EditApiKey(object sender, RoutedEventArgs e)
        {
            var content = new OneLineInputDialog();
            content.Input.Text = _reportService?.ApiKey;

            var dialog = new ContentDialog
            {
                XamlRoot = this.XamlRoot,
                Style = Application.Current.Resources["DefaultContentDialogStyle"] as Style,
                Width = 400,
                Title = "Edit ApiKey",
                PrimaryButtonText = "Save",
                SecondaryButtonText = "Cancel",
                DefaultButton = ContentDialogButton.Primary,
                Content = content
            };

            var result = await dialog.ShowAsync();
            if (result.Equals(ContentDialogResult.Primary))
            {
                _reportService!.ApiKey = content.Input.Text;
            }
        }
    }
}
