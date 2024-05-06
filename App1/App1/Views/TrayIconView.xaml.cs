using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using H.NotifyIcon;
using H.NotifyIcon.Core;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App1.Views
{
    [ObservableObject]
    public sealed partial class TrayIconView : UserControl
    {
        [ObservableProperty]
        private bool _isWindowVisible;

        [ObservableProperty]
        private ApplicationTheme _theme;

        public TrayIconView()
        {
            InitializeComponent();
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
                window.Hide();
            }
            else
            {
                window.Show();
            }
            IsWindowVisible = window.Visible;
        }

        [RelayCommand]
        public void ExitApplication()
        {
            this.TrayIcon.Dispose();
            App.MainWindow?.Close();
        }

    }
}