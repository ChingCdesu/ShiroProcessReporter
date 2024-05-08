using Windows.Media.Control;
using Microsoft.Extensions.Logging;
using ShiroProcessReporter.Helper;
using ShiroProcessReporter;
using Microsoft.Extensions.DependencyInjection;

namespace ShiroProcessReporter.Services;

public class TraceWorkerService
{
    private readonly ILogger<TraceWorkerService> _logger;

    private readonly MediaTraceService _mediaTraceService;

    private readonly ProcessTraceService _processTraceService;

    private readonly ReportService _reportService;

    public TraceWorkerService()
    {
        _logger = AppLogger.Factory.CreateLogger<TraceWorkerService>();
        _mediaTraceService = App.ServiceProvider.GetService<MediaTraceService>();
        _processTraceService = App.ServiceProvider.GetService<ProcessTraceService>();
        _reportService = App.ServiceProvider.GetService<ReportService>();

        _mediaTraceService.OnMediaPlaybackChanged += OnMediaPlaybackChanged;
        _processTraceService.OnFrontWindowChanged += OnFrontWindowChanged;
    }

    private void OnFrontWindowChanged(string windowTitle, string processName)
    {
        _logger.LogInformation("Working on {title} - {process}", windowTitle, processName);
        _reportService.ReportProcess(windowTitle, processName);
    }

    private void OnMediaPlaybackChanged(GlobalSystemMediaTransportControlsSessionMediaProperties properties,
        GlobalSystemMediaTransportControlsSessionPlaybackStatus status, string processName)
    {
        _logger.LogInformation("Now Playing: {Artist} - {Title} => {status}", properties.Artist, properties.Title,
            status);
        _reportService.ReportMedia(properties, status, processName);
    }
}