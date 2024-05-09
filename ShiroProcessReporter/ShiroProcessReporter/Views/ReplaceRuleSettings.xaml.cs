using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using ShiroProcessReporter.Components;
using ShiroProcessReporter.Models;
using ShiroProcessReporter.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using System.Windows.Input;
using Windows.Foundation;
using Windows.Foundation.Collections;

namespace ShiroProcessReporter.Views
{
    [ObservableObject]
    public sealed partial class ReplaceRuleSettings : Page
    {
        private readonly ReportService _reportService;

        [ObservableProperty]
        private ReplaceRule? _selected;

        [ObservableProperty]
        private ObservableCollection<ReplaceRule> _replaceRules;

        public ReplaceRuleSettings()
        {
            this.InitializeComponent();
            this._reportService = App.ServiceProvider!.GetService<ReportService>()!;
            this._replaceRules = new ObservableCollection<ReplaceRule>(_reportService.ReplaceRules);
        }

        [RelayCommand]
        private void Add()
        {
            _replaceRules.Add(ReplaceRuleDialog.DataContext as ReplaceRule);
            _reportService!.ReplaceRules = [.. _replaceRules];
        }

        [RelayCommand]
        private void Update()
        {
            _replaceRules[ReplaceRuleListView.SelectedIndex] = ReplaceRuleDialog.DataContext as ReplaceRule;
            _reportService!.ReplaceRules = [.. _replaceRules];
        }

        [RelayCommand]
        private void Delete()
        {
            _replaceRules.RemoveAt(ReplaceRuleListView.SelectedIndex);
            _reportService!.ReplaceRules = [.. _replaceRules];
        }

        private async Task OpenNewDialogAsync(object sender, object e)
        {
            await ShowAddDialogAsync();
        }

        private void ListView_RightTapped(object sender, RightTappedRoutedEventArgs e)
        {
            var rule = (e.OriginalSource as FrameworkElement).DataContext as ReplaceRule;
            Selected = rule;
        }

        private async void ListView_ItemClick(object sender, ItemClickEventArgs e)
        {
            ReplaceRule rule = e.ClickedItem as ReplaceRule;
            Selected = rule;
            await ShowEditDialogAsync(rule);
        }

        private async void Delete_Click(object sender, RoutedEventArgs e)
        {
            if (ReplaceRuleListView.SelectedItem is ReplaceRule rule)
            {
                Selected = rule;
                DeleteDialog.Title = $"Replace \"{rule.Original}\" to {rule.Replacement}";
                await DeleteDialog.ShowAsync();
            }
        }

        private async void Edit_Click(object sender, RoutedEventArgs e)
        {
            if (ReplaceRuleListView.SelectedItem is ReplaceRule rule)
            {
                await ShowEditDialogAsync(rule);
            }
        }

        private async void Duplicate_Click(object sender, RoutedEventArgs e)
        {
            if (ReplaceRuleListView.SelectedItem is ReplaceRule rule)
            {
                await ShowAddDialogAsync(rule);
            }
        }

        private void ReorderButtonUp_Click(object sender, RoutedEventArgs e)
        {
            if (ReplaceRuleListView.SelectedItem is ReplaceRule rule)
            {
                var index = ReplaceRules.IndexOf(rule);
                if (index > 0)
                {
                    ReplaceRules.Move(index, index - 1);
                    _reportService!.ReplaceRules = [.. _replaceRules];
                }
            }
        }

        private void ReorderButtonDown_Click(object sender, RoutedEventArgs e)
        {
            if (ReplaceRuleListView.SelectedItem is ReplaceRule rule)
            {
                var index = ReplaceRules.IndexOf(rule);
                if (index < ReplaceRules.Count - 1)
                {
                    ReplaceRules.Move(index, index + 1);
                    _reportService!.ReplaceRules = [.. _replaceRules];
                }
            }
        }

        private void Item_GotFocus(object sender, RoutedEventArgs e)
        {
            var element = sender as FrameworkElement;
            var rule = element.DataContext as ReplaceRule;

            if (rule is not null)
            {
                Selected = rule;
            }
            else if (ReplaceRuleListView.SelectedItem == null && ReplaceRuleListView.Items.Count > 0)
            {
                ReplaceRuleListView.SelectedItem = 0;
            }
        }

        private async Task ShowEditDialogAsync(ReplaceRule rule)
        {
            if (Selected is null)
            {
                return;
            }
            ReplaceRuleDialog.Title = "Edit replace rule";
            ReplaceRuleDialog.PrimaryButtonCommand = UpdateCommand;
            var clone = Selected.Clone();
            ReplaceRuleDialog.DataContext = clone;
            await ReplaceRuleDialog.ShowAsync();
        }

        private async Task ShowAddDialogAsync(ReplaceRule? template = null)
        {
            ReplaceRuleDialog.Title = "Add new replace rule";
            ReplaceRuleDialog.PrimaryButtonCommand = AddCommand;
            if (template is not null)
            {
                var clone = template.Clone();
                ReplaceRuleDialog.DataContext = clone;
            }

            await ReplaceRuleDialog.ShowAsync();
        }
    }
}
