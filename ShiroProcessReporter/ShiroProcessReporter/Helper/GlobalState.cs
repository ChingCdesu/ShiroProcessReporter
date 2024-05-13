using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.UI.Xaml;
using ShiroProcessReporter.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.UI.Dispatching;

namespace ShiroProcessReporter.Helper
{
    public partial class GlobalState : ObservableObject
    {
        [ObservableProperty]
        private ApplicationTheme _theme = Application.Current.RequestedTheme;

        [ObservableProperty]
        private ObservableCollection<LogEntry> _logs = [];

        public DispatcherQueue? LogViewDispatcherQueue { get; set; }

        private static GlobalState? _instance;
        public static GlobalState Instance => _instance ??= new GlobalState();
    }
}
