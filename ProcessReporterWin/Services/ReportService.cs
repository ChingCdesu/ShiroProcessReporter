using System.Net.Http.Json;
using System.Text.Json;
using System.Text.RegularExpressions;
using Windows.Media.Control;
using Microsoft.Extensions.Logging;

namespace ProcessReporterWin.Services;

public class ReportService
{
    private readonly HttpClient _httpClient = new()
    {
        Timeout = TimeSpan.FromSeconds(5)
    };

    private readonly ILogger<ReportService> _logger;
    private readonly string ApiKeyDefault = string.Empty;

    private readonly Dictionary<string, string> BuiltinReplaceRules = new()
    {
        { @"\.[Ee][Xx][Ee]", "" }, // 删除exe后缀

        { "[Ee]xplorer", "explorer" },

        { "msedge", "Microsoft Edge" },
        { "WINWORD", "Microsoft Word" },
        { "EXCEL", "Microsoft Excel" },
        { "POWERPNT", "Microsoft PowerPoint" },
        { "ONENOTE", "Microsoft OneNote" },

        { "idea64", "IntelliJ IDEA" },
        { "goland64", "GoLand" },
        { "pycharm64", "PyCharm" },

        { "GitHubDesktop", "GitHub Desktop" },
        { "chrome", "Chrome" }
    };

    private readonly string EndpointDefault = string.Empty;
    private readonly List<string> FilterRulesDefault = [];

    private readonly string IdApiKey = "api_key";

    private readonly string IdEndpoint = "endpoint";

    private readonly string IdFilterRules = "filter_rules";

    private readonly string IdReplaceRules = "replace_rules";
    private readonly Dictionary<string, string> ReplaceRulesDefault = new();

    private readonly string UserAgent
        = $"Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/121.0.0.0 Safari/537.36 ProcessReporter/{AppInfo.Current.VersionString}";

    public ReportService(ILogger<ReportService> logger)
    {
        _logger = logger;
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

    public Dictionary<string, string> ReplaceRules
    {
        get => JsonSerializer.Deserialize<Dictionary<string, string>>(Preferences.Get(IdReplaceRules, "{}")) ??
               ReplaceRulesDefault;
        set => Preferences.Set(IdReplaceRules, JsonSerializer.Serialize(value));
    }

    public List<string> FilterRules
    {
        get => JsonSerializer.Deserialize<List<string>>(Preferences.Get(IdFilterRules, "[]")) ?? FilterRulesDefault;
        set => Preferences.Set(IdFilterRules, JsonSerializer.Serialize(value));
    }

    private Dictionary<string, string> MergedReplaceRules =>
        BuiltinReplaceRules
            .Where(pair => !ReplaceRules.ContainsKey(pair.Key))
            .Concat(ReplaceRules)
            .ToDictionary(pair => pair.Key, pair => pair.Value);

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
            var regex = new Regex(Regex.Unescape(rule.Key));
            processName = regex.Replace(processName, rule.Value);
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
            var regex = new Regex(Regex.Unescape(rule.Key));
            processName = regex.Replace(processName, rule.Value);
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