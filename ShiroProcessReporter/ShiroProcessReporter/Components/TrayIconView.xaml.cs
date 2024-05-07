using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.WinUI.UI.Helpers;
using H.NotifyIcon;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Media.Imaging;
using Microsoft.UI.Xaml.Navigation;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace ShiroProcessReporter.Components
{
    [ObservableObject]
    public sealed partial class TrayIconView : UserControl
    {
        [ObservableProperty]
        private bool isWindowVisible;

        [ObservableProperty]
        private ApplicationTheme theme = Application.Current.RequestedTheme;

        private static readonly ImageSource LightIcon = new BitmapImage(new Uri("ms-appx:///Resources/TrayIcon/flower.ico"));
        private static readonly ImageSource DarkIcon = new BitmapImage(new Uri("ms-appx:///Resources/TrayIcon/flower-light.ico"));

        public TrayIconView()
        {
            this.InitializeComponent();

            this.ActualThemeChanged += OnThemeChanged;
        }

        private void OnThemeChanged(object sender, object? args)
        {
            Theme = Application.Current.RequestedTheme;
        }

        [RelayCommand]
        public void ShowHideWindow()
        {
            var window = App.MainWindow;
            if (window == null)
            {
                return;
            }

            if (window.Visible)
            {
                window?.Hide();
            }
            else
            {
                window?.Show();
            }
            IsWindowVisible = window?.Visible ?? false;
        }

        [RelayCommand]
        public void ExitApplication()
        {
            this.TrayIcon.Dispose();
            App.MainWindow?.Close();
        }
    }
}
