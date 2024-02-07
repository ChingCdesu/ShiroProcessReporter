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

        public ICommand EditReplaceRuleCommand => new Command<ReplaceRule>(EditReplaceRule);
        public ICommand SaveReplaceRuleCommand => new Command<ReplaceRule>(SaveReplaceRule);
        public ICommand DeleteReplaceRuleCommand => new Command<ReplaceRule>(DeleteReplaceRule);

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
                }).ToList());

            ReplaceRuleList.ItemsSource = ReplaceRules;

            ReplaceRuleEmptyTip.IsVisible = NoReplaceRule;
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
    }
}
