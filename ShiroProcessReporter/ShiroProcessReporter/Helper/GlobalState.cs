using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.UI.Xaml;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShiroProcessReporter.Helper
{
    public partial class GlobalState : ObservableObject
    {
        [ObservableProperty]
        private ApplicationTheme _theme = Application.Current.RequestedTheme;

        private static GlobalState? _instance;
        public static GlobalState Instance => _instance ??= new GlobalState();
    }
}
