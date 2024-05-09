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
using CommunityToolkit.Mvvm.Input;
using System.Threading.Tasks;
using ShiroProcessReporter.Models;

namespace ShiroProcessReporter.Views
{
    [ObservableObject]
    public sealed partial class FilterRuleSettings : Page
    {
        private readonly ReportService _reportService;

        [ObservableProperty]
        private FilterRule? _selected;

        [ObservableProperty]
        private ObservableCollection<FilterRule> _filterRules;

        public FilterRuleSettings()
        {
            this.InitializeComponent();
            this._reportService = App.ServiceProvider!.GetService<ReportService>()!;
            this._filterRules = new ObservableCollection<FilterRule>(_reportService.FilterRules);
        }

        [RelayCommand]
        private void Add()
        {
            _filterRules.Add(FilterRuleDialog.DataContext as FilterRule);
            _reportService!.FilterRules = [.. _filterRules];
        }

        [RelayCommand]
        private void Update()
        {
            _filterRules[FilterRuleListView.SelectedIndex] = FilterRuleDialog.DataContext as FilterRule;
            _reportService!.FilterRules = [.. _filterRules];
        }

        [RelayCommand]
        private void Delete()
        {
            _filterRules.RemoveAt(FilterRuleListView.SelectedIndex);
            _reportService!.FilterRules = [.. _filterRules];
        }

        private async Task OpenNewDialogAsync(object sender, object e)
        {
            await ShowAddDialogAsync();
        }

        private void ListView_RightTapped(object sender, RightTappedRoutedEventArgs e)
        {
            var rule = (e.OriginalSource as FrameworkElement).DataContext as FilterRule;
            Selected = rule;
        }

        private async void ListView_ItemClick(object sender, ItemClickEventArgs e)
        {
            FilterRule rule = e.ClickedItem as FilterRule;
            Selected = rule;
            await ShowEditDialogAsync(rule);
        }

        private async void Delete_Click(object sender, RoutedEventArgs e)
        {
            if (FilterRuleListView.SelectedItem is FilterRule rule)
            {
                Selected = rule;
                DeleteDialog.Title = $"Filter \"{rule.Original}\"";
                await DeleteDialog.ShowAsync();
            }
        }

        private async void Edit_Click(object sender, RoutedEventArgs e)
        {
            if (FilterRuleListView.SelectedItem is FilterRule rule)
            {
                await ShowEditDialogAsync(rule);
            }
        }

        private async void Duplicate_Click(object sender, RoutedEventArgs e)
        {
            if (FilterRuleListView.SelectedItem is FilterRule rule)
            {
                await ShowAddDialogAsync(rule);
            }
        }

        private void ReorderButtonUp_Click(object sender, RoutedEventArgs e)
        {
            if (FilterRuleListView.SelectedItem is FilterRule rule)
            {
                var index = FilterRules.IndexOf(rule);
                if (index > 0)
                {
                    FilterRules.Move(index, index - 1);
                    _reportService!.FilterRules = [.. _filterRules];

                }
            }
        }

        private void ReorderButtonDown_Click(object sender, RoutedEventArgs e)
        {
            if (FilterRuleListView.SelectedItem is FilterRule rule)
            {
                var index = FilterRules.IndexOf(rule);
                if (index < FilterRules.Count - 1)
                {
                    FilterRules.Move(index, index + 1);
                    _reportService!.FilterRules = [.. _filterRules];
                }
            }
        }

        private void Item_GotFocus(object sender, RoutedEventArgs e)
        {
            var element = sender as FrameworkElement;
            var rule = element.DataContext as FilterRule;

            if (rule is not null)
            {
                Selected = rule;
            }
            else if (FilterRuleListView.SelectedItem == null && FilterRuleListView.Items.Count > 0)
            {
                FilterRuleListView.SelectedItem = 0;
            }
        }

        private async Task ShowEditDialogAsync(FilterRule rule)
        {
            if (Selected is null)
            {
                return;
            }
            FilterRuleDialog.Title = "Edit filter rule";
            FilterRuleDialog.PrimaryButtonCommand = UpdateCommand;
            var clone = Selected.Clone();
            FilterRuleDialog.DataContext = clone;
            await FilterRuleDialog.ShowAsync();
        }

        private async Task ShowAddDialogAsync(FilterRule? template = null)
        {
            FilterRuleDialog.Title = "Add new filter rule";
            FilterRuleDialog.PrimaryButtonCommand = AddCommand;
            if (template is not null)
            {
                var clone = template.Clone();
                FilterRuleDialog.DataContext = clone;
            }

            await FilterRuleDialog.ShowAsync();
        }
    }
}
