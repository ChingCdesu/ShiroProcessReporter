using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Media.Animation;
using Microsoft.UI.Xaml.Navigation;
using ShiroProcessReporter.Views;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.ApplicationSettings;

namespace ShiroProcessReporter.Layouts
{
    [ObservableObject]
    public sealed partial class Navigation : Page
    {
        public Navigation()
        {
            this.InitializeComponent();
        }

        [ObservableProperty]
        private string _title = "";

        private void NavigationView_Loaded(object sender, RoutedEventArgs e)
        {
            NavView.SelectedItem = NavView.MenuItems[0];
            Navigate(typeof(EndpointSettings), new EntranceNavigationTransitionInfo());
        }

        private void NavigationView_ItemInvoked(NavigationView sender, NavigationViewItemInvokedEventArgs args)
        {
            if (args.IsSettingsInvoked == true)
            {
                //Navigate(typeof(SettingsPage), args.RecommendedNavigationTransitionInfo);
            }
            else if (args.InvokedItemContainer != null)
            {
                Type? navPageType = Type.GetType(args.InvokedItemContainer.Tag.ToString());
                if (navPageType is null)
                {
                    return;
                }
                Navigate(navPageType, args.RecommendedNavigationTransitionInfo);
            }
        }

        private void Navigate(Type navPageType, NavigationTransitionInfo transitionInfo)
        {
            Type preNavPageType = RouterView.CurrentSourcePageType;
            if (navPageType is not null && !Type.Equals(preNavPageType, navPageType))
            {
                if (RouterView.Content is Page currentPage && currentPage is IDisposable disposable)
                {
                    disposable.Dispose();
                }
                RouterView.Navigate(navPageType, null, transitionInfo);
                Title = (NavView.SelectedItem as NavigationViewItem).Content.ToString();
            }
        }
    }
}
