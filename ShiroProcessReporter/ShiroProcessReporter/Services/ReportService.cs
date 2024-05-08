using System.Net.Http.Json;
using System.Text.Json;
using System.Text.RegularExpressions;
using Windows.Media.Control;
using Microsoft.Extensions.Logging;
using System;
using System.Net.Http;
using System.Collections.Generic;
using Windows.ApplicationModel;
using ShiroProcessReporter.Helper;
using System.Linq;
using ShiroProcessReporter.Models;
using Windows.ApplicationModel.Appointments.AppointmentsProvider;

namespace ShiroProcessReporter.Services;

public class ReportService
{
    private readonly HttpClient _httpClient = new()
    {
        Timeout = TimeSpan.FromSeconds(5)
    };

    private readonly ILogger<ReportService> _logger;
    private readonly string ApiKeyDefault = string.Empty;

    private readonly List<ReplaceRule> BuiltinReplaceRules = new()
    {
        new ReplaceRule{ Original = @"\.[Ee][Xx][Ee]", Replacement = "" }, // 删除exe后缀

        new ReplaceRule{ Original = "[Ee]xplorer", Replacement = "explorer" },

        new ReplaceRule{ Original = "msedge", Replacement = "Microsoft Edge" },
        new ReplaceRule{ Original = "WINWORD", Replacement = "Microsoft Word" },
        new ReplaceRule{ Original = "EXCEL", Replacement = "Microsoft Excel" },
        new ReplaceRule{ Original = "POWERPNT", Replacement = "Microsoft PowerPoint" },
        new ReplaceRule{ Original = "ONENOTE", Replacement = "Microsoft OneNote" },

        new ReplaceRule{ Original = "idea64", Replacement = "IntelliJ IDEA" },
        new ReplaceRule{ Original = "goland64", Replacement = "GoLand" },
        new ReplaceRule{ Original = "pycharm64", Replacement = "PyCharm" },

        new ReplaceRule{ Original = "GitHubDesktop", Replacement = "GitHub Desktop" },
        new ReplaceRule{ Original = "chrome", Replacement = "Chrome" },
    };

    private readonly string EndpointDefault = string.Empty;
    private readonly List<string> FilterRulesDefault = [];
    private readonly List<ReplaceRule> ReplaceRulesDefault = new();

    private readonly string IdApiKey = "api_key";

    private readonly string IdEndpoint = "endpoint";

    private readonly string IdFilterRules = "filter_rules";

    private readonly string IdReplaceRules = "replace_rules";

    private readonly string UserAgent
        = $"Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/121.0.0.0 Safari/537.36 ProcessReporter/{Package.Current.Id.Version}";

    public ReportService()
    {
        _logger = AppLogger.Factory.CreateLogger<ReportService>();
        _httpClient.DefaultRequestHeaders.Add("User-Agent", UserAgent);
    }

    public string Endpoint
    {
        get => Preferences.Get(IdEndpoint, EndpointDefault);
        set => Preferences.Set(IdEndpoint, value);
    }

    public string ApiKey
    {
        get => Preferences.Get(IdApiKey, ApiKeyDefault);
        set => Preferences.Set(IdApiKey, value);
    }

    public List<ReplaceRule> ReplaceRules
    {
        get => JsonSerializer.Deserialize<List<ReplaceRule>>(Preferences.Get(IdReplaceRules, "[]")) ?? ReplaceRulesDefault;
        set => Preferences.Set(IdReplaceRules, JsonSerializer.Serialize(value));
    }

    public List<string> FilterRules
    {
        get => JsonSerializer.Deserialize<List<string>>(Preferences.Get(IdFilterRules, "[]")) ?? FilterRulesDefault;
        set => Preferences.Set(IdFilterRules, JsonSerializer.Serialize(value));
    }

    private List<ReplaceRule> MergedReplaceRules => [.. BuiltinReplaceRules, .. ReplaceRules];

    public async void ReportProcess(string windowTitle, string processName)
    {
        if (string.IsNullOrEmpty(Endpoint))
        {
            _logger.LogInformation("Endpoint is empty, skip report");
            return;
        }

        if (string.IsNullOrEmpty(ApiKey))
        {
            _logger.LogInformation("Api key is empty, skip report");
            return;
        }

        var timeStamp = DateTimeOffset.Now.ToUnixTimeMilliseconds();

        foreach (var rule in MergedReplaceRules)
        {
            var regex = new Regex(Regex.Unescape(rule.Original));
            processName = regex.Replace(processName, rule.Replacement);
        }

        try
        {
            var response = await _httpClient.PostAsJsonAsync(Endpoint, new Dictionary<string, object>
            {
                { "timestamp", timeStamp },
                { "process", processName },
                { "title", windowTitle },
                { "key", ApiKey }
            });

            response.EnsureSuccessStatusCode();

            var dataStr = await response.Content.ReadAsStringAsync();

            response.Dispose();

            if (string.IsNullOrEmpty(dataStr)) throw new Exception("Api server return empty content");

            var data = JsonSerializer.Deserialize<Dictionary<string, object>>(dataStr) ??
                       throw new Exception("Invaild response");

            if (data.TryGetValue("ok", out var eOk))
            {
                var ok = ((JsonElement?)eOk).Value.GetInt32();
                if (ok == 0)
                    if (data.TryGetValue("message", out var eMsg))
                    {
                        var msg = ((JsonElement?)eMsg).Value.GetString();
                        throw new Exception(msg);
                    }
            }

            _logger.LogInformation("Report process success");
        }
        catch (Exception ex)
        {
            _logger.LogError("Report failed, reason: {message}", ex.Message);
        }
    }

    public async void ReportMedia(GlobalSystemMediaTransportControlsSessionMediaProperties properties,
        GlobalSystemMediaTransportControlsSessionPlaybackStatus status, string processName)
    {
        if (status != GlobalSystemMediaTransportControlsSessionPlaybackStatus.Playing) return;

        if (string.IsNullOrEmpty(Endpoint))
        {
            _logger.LogInformation("Endpoint is empty, skip report");
            return;
        }

        if (string.IsNullOrEmpty(ApiKey))
        {
            _logger.LogInformation("Api key is empty, skip report");
            return;
        }

        if (string.IsNullOrEmpty(properties.Artist))
        {
            _logger.LogWarning("No artist info found, don't think it's a music program");
            return;
        }

        if (string.IsNullOrEmpty(properties.Title))
        {
            _logger.LogWarning("No title info found, don't think it's a music program");
            return;
        }

        var timeStamp = DateTimeOffset.Now.ToUnixTimeMilliseconds();

        foreach (var rule in MergedReplaceRules)
        {
            var regex = new Regex(Regex.Unescape(rule.Original));
            processName = regex.Replace(processName, rule.Replacement);
        }

        try
        {
            var response = await _httpClient.PostAsJsonAsync(Endpoint, new Dictionary<string, object>
            {
                { "timestamp", timeStamp },
                { "process", processName },
                {
                    "media", new Dictionary<string, string>
                    {
                        { "title", properties.Title },
                        { "artist", properties.Artist }
                    }
                },
                { "key", ApiKey }
            });

            response.EnsureSuccessStatusCode();

            var dataStr = await response.Content.ReadAsStringAsync();

            response.Dispose();

            if (string.IsNullOrEmpty(dataStr)) throw new Exception("Api server return empty content");

            var data = JsonSerializer.Deserialize<Dictionary<string, object>>(dataStr) ??
                       throw new Exception("Invaild response");

            if (data.TryGetValue("ok", out var eOk))
            {
                var ok = ((JsonElement?)eOk).Value.GetInt32();
                if (ok == 0)
                    if (data.TryGetValue("message", out var eMsg))
                    {
                        var msg = ((JsonElement?)eMsg).Value.GetString();
                        throw new Exception(msg);
                    }
            }

            _logger.LogInformation("Report media success");
        }
        catch (Exception ex)
        {
            _logger.LogError("Report failed, reason: {message}", ex.Message);
        }
    }
}