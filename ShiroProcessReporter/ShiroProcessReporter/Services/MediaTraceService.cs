using System;
using Windows.Media.Control;
using Microsoft.Extensions.Logging;
using ShiroProcessReporter.Helper;

namespace ShiroProcessReporter.Services;

public class MediaTraceService
{
    public delegate void MediaPlaybackChangeHandler(GlobalSystemMediaTransportControlsSessionMediaProperties properties,
        GlobalSystemMediaTransportControlsSessionPlaybackStatus status, string processName);

    private readonly ILogger<MediaTraceService> _logger;

    private GlobalSystemMediaTransportControlsSessionManager? _manager;

    private string _processName = string.Empty;

    private GlobalSystemMediaTransportControlsSessionMediaProperties? _properties;

    private GlobalSystemMediaTransportControlsSessionPlaybackStatus _status;

    public MediaTraceService()
    {
        _logger = AppLogger.Factory.CreateLogger<MediaTraceService>();
        _properties = null;
        _status = GlobalSystemMediaTransportControlsSessionPlaybackStatus.Stopped;
        InitManager();
    }

    public event MediaPlaybackChangeHandler OnMediaPlaybackChanged;

    private async void InitManager()
    {
        _manager = await GlobalSystemMediaTransportControlsSessionManager.RequestAsync();
        if (_manager is null)
        {
            _logger.LogError("Failed to get MediaSessionManager");
            return;
        }

        // 初始化触发一次
        MediaSessionChanged(_manager, null);
        _manager.CurrentSessionChanged += MediaSessionChanged;
    }

    private void MediaSessionChanged(GlobalSystemMediaTransportControlsSessionManager manager,
        CurrentSessionChangedEventArgs? args)
    {
        var session = manager.GetCurrentSession();
        if (session is null)
        {
            _processName = string.Empty;
            _properties = null;
            _status = GlobalSystemMediaTransportControlsSessionPlaybackStatus.Stopped;

            _logger.LogWarning("Media session is null");
            return;
        }

        // 初始化触发一次
        MediaPropertiesChanged(session, null);
        PlaybackInfoChanged(session, null);

        _processName = session.SourceAppUserModelId;
        session.MediaPropertiesChanged += MediaPropertiesChanged;
        session.PlaybackInfoChanged += PlaybackInfoChanged;
    }

    private void PlaybackInfoChanged(GlobalSystemMediaTransportControlsSession sender,
        PlaybackInfoChangedEventArgs? args)
    {
        var playbackInfo = sender.GetPlaybackInfo();
        _status = playbackInfo.PlaybackStatus;

        _logger.LogInformation("Play status change to {status}", playbackInfo.PlaybackStatus);

        CallEvent();
    }

    private async void MediaPropertiesChanged(GlobalSystemMediaTransportControlsSession sender,
        MediaPropertiesChangedEventArgs? args)
    {
        try
        {
            var mediaProperities = await sender.TryGetMediaPropertiesAsync();
            if (mediaProperities is null)
            {
                _logger.LogError("Media properties is null");
                return;
            }

            _properties = mediaProperities;

            _logger.LogInformation("Playing {artist} - {title}", mediaProperities.Artist, mediaProperities.Title);

            CallEvent();
        }
        catch (Exception)
        {
            _logger.LogWarning("Failed to get media properties");
        }
    }

    private void CallEvent()
    {
        if (_properties is not null)
            if (OnMediaPlaybackChanged is not null)
                OnMediaPlaybackChanged(_properties, _status, _processName);
    }
}