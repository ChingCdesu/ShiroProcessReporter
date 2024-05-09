using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using ShiroProcessReporter.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using WinUIEx;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace ShiroProcessReporter
{
    public sealed partial class MainView : WindowEx
    {
        private readonly TraceWorkerService _traceWorkerService;

        public MainView()
        {
            this.InitializeComponent();
            _traceWorkerService = App.ServiceProvider.GetService<TraceWorkerService>();

            ExtendsContentIntoTitleBar = true;
            SetTitleBar(titleBar);
            var title = "Process Reporter Settings";
            Title = title;
            AppTitleTextBlock.Text = title;
            Activated += MainWindow_Activated;
        }

        private void MainWindow_Activated(object sender, WindowActivatedEventArgs args)
        {
            if (args.WindowActivationState == WindowActivationState.Deactivated)
            {
                AppTitleTextBlock.Foreground = (SolidColorBrush)App.Current.Resources["WindowCaptionForegroundDisabled"];
            }
            else
            {
                AppTitleTextBlock.Foreground = (SolidColorBrush)App.Current.Resources["WindowCaptionForeground"];
            }
        }

    }
}
