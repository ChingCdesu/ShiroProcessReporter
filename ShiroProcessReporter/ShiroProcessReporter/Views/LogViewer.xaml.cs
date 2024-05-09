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
using ShiroProcessReporter.Models;
using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using ShiroProcessReporter.Helper;
using CommunityToolkit.Mvvm.Input;

namespace ShiroProcessReporter.Views
{
    [ObservableObject]
    public sealed partial class LogViewer : Page
    {
        [ObservableProperty]
        private bool _isFollow = false;

        public LogViewer()
        {
            this.InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            GlobalState.Instance.Logs.CollectionChanged += Logs_CollectionChanged;
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            base.OnNavigatedFrom(e);
            GlobalState.Instance.Logs.CollectionChanged -= Logs_CollectionChanged;
        }

        private void Logs_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (IsFollow)
            {
                ScrollToBottom();
            }
        }

        private void FollowToggleButton_Checked(object sender, RoutedEventArgs e)
        {
            IsFollow = true;
            ScrollToBottom();
        }

        private void FollowToggleButton_Unchecked(object sender, RoutedEventArgs e)
        {
            IsFollow = false;
        }

        private void ScrollToBottom()
        {
            var scrollViewer = GetScrollViewer(LogListView);
            if (scrollViewer != null)
            {
                var scrollableHeight = scrollViewer.ScrollableHeight;
                scrollViewer.ChangeView(null, scrollableHeight, null, true);
            }
        }

        // 辅助方法：递归查找ListView中的ScrollViewer
        private ScrollViewer GetScrollViewer(DependencyObject dependencyObject)
        {
            if (dependencyObject is ScrollViewer)
            {
                return dependencyObject as ScrollViewer;
            }

            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(dependencyObject); i++)
            {
                var child = VisualTreeHelper.GetChild(dependencyObject, i);
                var result = GetScrollViewer(child);
                if (result != null)
                {
                    return result;
                }
            }
            return null;
        }

        [RelayCommand]
        private void ClearLogs()
        {
            GlobalState.Instance.Logs.Clear();
        }
    }
}
