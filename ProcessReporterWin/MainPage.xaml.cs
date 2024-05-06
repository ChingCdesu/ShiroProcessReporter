using System.Collections.ObjectModel;
using System.Windows.Input;
using CommunityToolkit.Mvvm.Input;
using H.NotifyIcon;
using ProcessReporterWin.Entities;
using ProcessReporterWin.Services;

namespace ProcessReporterWin;

public partial class MainPage : ContentPage
{
    private readonly ReportService _reportService;
    private readonly TraceWorkerService _traceWorkerService;

    public MainPage(TraceWorkerService traceWorkerService, ReportService reportService)
    {
        InitializeComponent();

        _reportService = reportService;
        _traceWorkerService = traceWorkerService;

        BindingContext = this;

        ReplaceRules = new ObservableCollection<ReplaceRule>(
            _reportService.ReplaceRules.AsEnumerable().Select(x => new ReplaceRule
            {
                Key = x.Key,
                Value = x.Value,
                Editing = false
            }));

        ReplaceRuleList.ItemsSource = ReplaceRules;
        ReplaceRuleEmptyTip.IsVisible = NoReplaceRule;

        FilterRules = new ObservableCollection<FilterRule>(
            _reportService.FilterRules.Select(x => new FilterRule
            {
                KeyWord = x,
                Editing = false
            }));

        FilterRuleList.ItemsSource = FilterRules;
        FilterRuleEmptyTip.IsVisible = NoFilterRule;
    }

    public bool HidePassword { get; set; } = false;

    public ObservableCollection<ReplaceRule> ReplaceRules { get; }

    public bool NoReplaceRule => ReplaceRules.Count == 0;

    public ObservableCollection<FilterRule> FilterRules { get; }

    public bool NoFilterRule => FilterRules.Count == 0;

    public ICommand EditReplaceRuleCommand => new Command<ReplaceRule>(EditReplaceRule);
    public ICommand SaveReplaceRuleCommand => new Command<ReplaceRule>(SaveReplaceRule);
    public ICommand DeleteReplaceRuleCommand => new Command<ReplaceRule>(DeleteReplaceRule);

    public ICommand EditFilterRuleCommand => new Command<FilterRule>(EditFilterRule);
    public ICommand SaveFilterRuleCommand => new Command<FilterRule>(SaveFilterRule);
    public ICommand DeleteFilterRuleCommand => new Command<FilterRule>(DeleteFilterRule);

    public string Endpoint
    {
        get => _reportService.Endpoint;
        set => _reportService.Endpoint = value.Trim();
    }

    public string ApiKey
    {
        get => _reportService.ApiKey;
        set => _reportService.ApiKey = value.Trim();
    }

    protected override void OnAppearing()
    {
        Window.Width = 600;
        Window.Height = 450;
        base.OnAppearing();
    }

    [RelayCommand]
    public void ShowWindow()
    {
        Window.Show();
    }

    [RelayCommand]
    public void ExitApp()
    {
        Application.Current?.Quit();
    }

    public void AddReplaceRule(object sender, EventArgs args)
    {
        Dispatcher.Dispatch(() =>
        {
            ReplaceRules.Add(new ReplaceRule { Editing = true });
            ReplaceRuleEmptyTip.IsVisible = NoReplaceRule;
        });
    }

    public void DeleteReplaceRule(ReplaceRule rule)
    {
        Dispatcher.Dispatch(() =>
        {
            ReplaceRules.Remove(rule);

            SaveReplaceRuleToPreference();
            ReplaceRuleEmptyTip.IsVisible = NoReplaceRule;
        });
    }

    public void SaveReplaceRule(ReplaceRule rule)
    {
        Dispatcher.Dispatch(() =>
        {
            var index = -1;
            // 删除当前正在编辑的和之前的
            while (ReplaceRules.Any(r => r.Key == rule.Key))
            {
                var old = ReplaceRules.First(r => r.Key == rule.Key);
                var i = ReplaceRules.IndexOf(old);
                if (index == -1) index = i;
                ReplaceRules.Remove(old);
            }

            rule.Editing = false;
            ReplaceRules.Insert(index, rule);

            SaveReplaceRuleToPreference();
            ReplaceRuleEmptyTip.IsVisible = NoReplaceRule;
        });
    }

    public void EditReplaceRule(ReplaceRule rule)
    {
        Dispatcher.Dispatch(() =>
        {
            var old = ReplaceRules.First(r => r.Key == rule.Key);
            var index = ReplaceRules.IndexOf(old);
            ReplaceRules.Remove(old);

            rule.Editing = true;
            ReplaceRules.Insert(index, rule);
        });
    }

    private void SaveReplaceRuleToPreference()
    {
        var rules = new Dictionary<string, string>();
        foreach (var item in ReplaceRules) rules.Add(item.Key, item.Value);
        _reportService.ReplaceRules = rules;
    }

    public void AddFilterRule(object sender, EventArgs args)
    {
        Dispatcher.Dispatch(() =>
        {
            FilterRules.Add(new FilterRule { Editing = true });
            FilterRuleEmptyTip.IsVisible = NoFilterRule;
        });
    }

    public void DeleteFilterRule(FilterRule rule)
    {
        Dispatcher.Dispatch(() =>
        {
            FilterRules.Remove(rule);

            SaveFilterRuleToPreference();
            FilterRuleEmptyTip.IsVisible = NoFilterRule;
        });
    }

    public void SaveFilterRule(FilterRule rule)
    {
        Dispatcher.Dispatch(() =>
        {
            var index = -1;
            // 删除当前正在编辑的和之前的
            while (FilterRules.Any(r => r.KeyWord == rule.KeyWord))
            {
                var old = FilterRules.First(r => r.KeyWord == rule.KeyWord);
                var i = FilterRules.IndexOf(old);
                if (index == -1) index = i;
                FilterRules.Remove(old);
            }

            rule.Editing = false;
            FilterRules.Insert(index, rule);

            SaveFilterRuleToPreference();
            FilterRuleEmptyTip.IsVisible = NoFilterRule;
        });
    }

    public void EditFilterRule(FilterRule rule)
    {
        Dispatcher.Dispatch(() =>
        {
            var old = FilterRules.First(r => r.KeyWord == rule.KeyWord);
            var index = FilterRules.IndexOf(old);
            FilterRules.Remove(old);

            rule.Editing = true;
            FilterRules.Insert(index, rule);
        });
    }

    private void SaveFilterRuleToPreference()
    {
        _reportService.FilterRules = FilterRules.Select(x => x.KeyWord).ToList();
    }
}