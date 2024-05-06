using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.Input;
using H.NotifyIcon;

namespace ProcessReporterWin.Layouts;

public partial class SideBar : ContentPage
{
    public SideBar()
    {
        InitializeComponent();
        
        OnAppThemeChanged(this, new AppThemeChangedEventArgs(Application.Current!.RequestedTheme));
        
        Application.Current!.RequestedThemeChanged += OnAppThemeChanged;

        BindingContext = this;
    }
    
    [RelayCommand]
    private void ShowWindow()
    {
        Window!.Show();
    }

    [RelayCommand]
    private void ExitApp()
    {
        Application.Current?.Quit();
    }

    private void OnAppThemeChanged(object? sender, AppThemeChangedEventArgs e)
    {
        this.NTaskbarIcon.Icon = new Icon(e.RequestedTheme == AppTheme.Dark ? "flower_light.ico" : "flower.ico");
    }
}