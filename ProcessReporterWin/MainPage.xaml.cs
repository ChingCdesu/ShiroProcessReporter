using H.NotifyIcon;
using Microsoft.Windows.AppNotifications;
using ProcessReporterWin.Models;
using ProcessReporterWin.Services;
using CommunityToolkit.Mvvm.Input;

namespace ProcessReporterWin
{
    public partial class MainPage : ContentPage
    {
        private MainPageViewModel _viewModel;

        private readonly ReportService _reportService;

        public MainPage(MainPageViewModel viewModel, ReportService reportService)
        {
            InitializeComponent();

            _reportService = reportService;
            _viewModel = viewModel;

            BindingContext = this;
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
    }
}
