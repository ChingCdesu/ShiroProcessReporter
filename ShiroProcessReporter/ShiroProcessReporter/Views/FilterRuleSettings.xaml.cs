using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.ObjectModel;
using Microsoft.Extensions.DependencyInjection;
using ShiroProcessReporter.Services;

namespace ShiroProcessReporter.Views
{
    [ObservableObject]
    public sealed partial class FilterRuleSettings : Page
    {
        private readonly ReportService _reportService;

        [ObservableProperty]
        private string? _selected;

        [ObservableProperty]
        private ObservableCollection<string> _filterRules;

        public FilterRuleSettings()
        {
            this.InitializeComponent();
            this._reportService = App.ServiceProvider!.GetService<ReportService>()!;
            this._filterRules = new ObservableCollection<string>(_reportService.FilterRules);
        }
    }
}
