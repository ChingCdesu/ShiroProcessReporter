using H.NotifyIcon;
using Microsoft.Windows.AppNotifications;
using ProcessReporterWin.Services;
using CommunityToolkit.Mvvm.Input;
using System.Collections;
using System.Collections.ObjectModel;
using ProcessReporterWin.Entities;
using System.Windows.Input;

namespace ProcessReporterWin
{
    public partial class MainPage : ContentPage
    {
        private readonly TraceWorkerService _traceWorkerService;
        
        private readonly ReportService _reportService;

        public bool HidePassword { get; set; } = false;

        private ObservableCollection<ReplaceRule> _replaceRules;

        public ObservableCollection<ReplaceRule> ReplaceRules
        {
            get => _replaceRules;
        }

        public bool NoReplaceRule
        {
            get => _replaceRules is null || _replaceRules.Count == 0;
        }

        private ObservableCollection<FilterRule> _filterRules;

        public ObservableCollection<FilterRule> FilterRules
        {
            get => _filterRules;
        }

        public bool NoFilterRule
        {
            get => _filterRules is null || _filterRules.Count == 0;
        }

        public ICommand EditReplaceRuleCommand => new Command<ReplaceRule>(EditReplaceRule);
        public ICommand SaveReplaceRuleCommand => new Command<ReplaceRule>(SaveReplaceRule);
        public ICommand DeleteReplaceRuleCommand => new Command<ReplaceRule>(DeleteReplaceRule);

        public ICommand EditFilterRuleCommand => new Command<FilterRule>(EditFilterRule);
        public ICommand SaveFilterRuleCommand => new Command<FilterRule>(SaveFilterRule);
        public ICommand DeleteFilterRuleCommand => new Command<FilterRule>(DeleteFilterRule);

        public MainPage(TraceWorkerService traceWorkerService, ReportService reportService)
        {
            InitializeComponent();

            _reportService = reportService;
            _traceWorkerService = traceWorkerService;
            
            BindingContext = this;

            _replaceRules = new ObservableCollection<ReplaceRule>(
                _reportService.ReplaceRules.AsEnumerable().Select(x => new ReplaceRule
                {
                    Key = x.Key,
                    Value = x.Value,
                    Editing = false,
                }));

            ReplaceRuleList.ItemsSource = ReplaceRules;
            ReplaceRuleEmptyTip.IsVisible = NoReplaceRule;

            _filterRules = new ObservableCollection<FilterRule>(
                _reportService.FilterRules.Select(x => new FilterRule
                {
                    KeyWord = x,
                    Editing = false,
                }));

            FilterRuleList.ItemsSource = FilterRules;
            FilterRuleEmptyTip.IsVisible = NoFilterRule;
        }

        protected override void OnAppearing()
        {
            this.Window.Width = 600;
            this.Window.Height = 450;
            base.OnAppearing();
        }

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

        [RelayCommand]
        public void ShowWindow()
        {
            this.Window.Show();
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
                this._replaceRules.Add(new ReplaceRule { Editing = true });
                ReplaceRuleEmptyTip.IsVisible = NoReplaceRule;
            });
        }

        public void DeleteReplaceRule(ReplaceRule rule)
        {
            Dispatcher.Dispatch(() =>
            {
                this._replaceRules.Remove(rule);

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
                while (this._replaceRules.Any(r => r.Key == rule.Key))
                {
                    var old = this._replaceRules.First(r => r.Key == rule.Key);
                    var i = this._replaceRules.IndexOf(old);
                    if (index == -1)
                    {
                        index = i;
                    }
                    this._replaceRules.Remove(old);
                }

                rule.Editing = false;
                this._replaceRules.Insert(index, rule);

                SaveReplaceRuleToPreference();
                ReplaceRuleEmptyTip.IsVisible = NoReplaceRule;
            });
        }

        public void EditReplaceRule(ReplaceRule rule)
        {
            Dispatcher.Dispatch(() =>
            {
                var old = this._replaceRules.First(r => r.Key == rule.Key);
                var index = this._replaceRules.IndexOf(old);
                this._replaceRules.Remove(old);

                rule.Editing = true;
                this._replaceRules.Insert(index, rule);
            });
        }

        private void SaveReplaceRuleToPreference()
        {
            var rules = new Dictionary<string, string>();
            foreach (var item in this._replaceRules)
            {
                rules.Add(item.Key, item.Value);
            }
            this._reportService.ReplaceRules = rules;
        }

        public void AddFilterRule(object sender, EventArgs args)
        {
            Dispatcher.Dispatch(() =>
            {
                this._filterRules.Add(new FilterRule { Editing = true });
                FilterRuleEmptyTip.IsVisible = NoFilterRule;
            });
        }

        public void DeleteFilterRule(FilterRule rule)
        {
            Dispatcher.Dispatch(() =>
            {
                this._filterRules.Remove(rule);

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
                while (this._filterRules.Any(r => r.KeyWord == rule.KeyWord))
                {
                    var old = this._filterRules.First(r => r.KeyWord == rule.KeyWord);
                    var i = this._filterRules.IndexOf(old);
                    if (index == -1)
                    {
                        index = i;
                    }
                    this._filterRules.Remove(old);
                }

                rule.Editing = false;
                this._filterRules.Insert(index, rule);

                SaveFilterRuleToPreference();
                FilterRuleEmptyTip.IsVisible = NoFilterRule;
            });
        }

        public void EditFilterRule(FilterRule rule)
        {
            Dispatcher.Dispatch(() =>
            {
                var old = this._filterRules.First(r => r.KeyWord == rule.KeyWord);
                var index = this._filterRules.IndexOf(old);
                this._filterRules.Remove(old);

                rule.Editing = true;
                this._filterRules.Insert(index, rule);
            });
        }

        private void SaveFilterRuleToPreference()
        {
            this._reportService.FilterRules = this._filterRules.Select(x => x.KeyWord).ToList();
        }
    }
}
