using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.Logging;
using ProcessReporterWin.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Media.Control;

namespace ProcessReporterWin.Models
{
    public class MainPageViewModel
    {
        private readonly ILogger<MainPageViewModel> _logger;

        private readonly ProcessTraceService _windowTraceService;

        private readonly MediaTraceService _mediaTraceService;

        private readonly ReportService _reportService;

        public MainPageViewModel(
            ILogger<MainPageViewModel> logger, 
            ProcessTraceService windowTraceService,
            MediaTraceService mediaTraceService,
            ReportService reportService)
        {
            _logger = logger;
            _windowTraceService = windowTraceService;
            _mediaTraceService = mediaTraceService;
            _reportService = reportService;

            _mediaTraceService.OnMediaPlaybackChanged += OnMediaPlaybackChanged;
            _windowTraceService.OnFrontWindowChanged += OnFrontWindowChanged;
        }

        private void OnFrontWindowChanged(string windowTitle, string processName)
        {
            _logger.LogInformation("Working on {title} - {process}", windowTitle, processName);
            _reportService.ReportProcess(windowTitle, processName.Replace(".exe", ""));
        }

        private void OnMediaPlaybackChanged(GlobalSystemMediaTransportControlsSessionMediaProperties properties, GlobalSystemMediaTransportControlsSessionPlaybackStatus status, string processName)
        {
            _logger.LogInformation("Now Playing: {Artist} - {Title} => {status}", properties.Artist, properties.Title, status);
            _reportService.ReportMedia(properties, status, processName.Replace(".exe", ""));
        }
    }
}

