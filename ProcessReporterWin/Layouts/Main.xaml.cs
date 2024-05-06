using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.Input;
using H.NotifyIcon;
using ProcessReporterWin.Helper;

namespace ProcessReporterWin.Layouts;

public partial class Main : FlyoutPage
{
    public Main()
    {
        InitializeComponent();
        
        this.SideBar.RouteList.SelectionChanged += OnRouteChanged;
    }

    void OnRouteChanged(object? sender, SelectionChangedEventArgs e)
    {
        if (e.CurrentSelection.FirstOrDefault() is SideBarItem item)
        {
            RouterView.Content = (View)Activator.CreateInstance(item.RouterType)!;
        }
    }
}