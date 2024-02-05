using Microsoft.Extensions.Logging;
using System.Net.Http.Json;
using System.Text.Json;
using Windows.Media.Control;

namespace ProcessReporterWin.Services
{
    public class ReportService
    {
        private readonly string UserAgent
            = $"Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/121.0.0.0 Safari/537.36 ProcessReporter/{AppInfo.Current.VersionString}";

        private readonly string IdEndpoint = "endpoint";
        private readonly string EndpointDefault = string.Empty;

        private readonly string IdApiKey = "api_key";
        private readonly string ApiKeyDefault = string.Empty;

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

        private readonly ILogger<ReportService> _logger;

        public ReportService(ILogger<ReportService> logger)
        {
            _logger = logger;
        }

        public async void ReportProcess(string windowTitle, string processName)
        {
            long timeStamp = DateTimeOffset.Now.ToUnixTimeMilliseconds();
            var httpClient = new HttpClient
            {
                Timeout = TimeSpan.FromSeconds(5),
            };
            httpClient.DefaultRequestHeaders.Add("User-Agent", UserAgent);
            try
            {
                var response = await httpClient.PostAsJsonAsync(Endpoint, new Dictionary<string, object>
                {
                    { "timestamp", timeStamp },
                    { "process", processName },
                    { "title", windowTitle },
                    { "key", ApiKey },
                });

                response.EnsureSuccessStatusCode();

                var data = await response.Content.ReadFromJsonAsync<Dictionary<string, JsonElement>>();

                if (data is null)
                {
                    throw new Exception("Api server return empty content");
                }

                if (data.TryGetValue("ok", out var eOk))
                {
                    var ok = eOk.GetInt32();
                    if (ok == 0)
                    {
                        if (data.TryGetValue("message", out var eMsg))
                        {
                            var msg = eMsg.ToString();
                            throw new Exception(msg);
                        }
                    }
                }

                _logger.LogInformation("Report process success");
            }
            catch (Exception ex)
            {
                _logger.LogError("Report failed, reason: {message}", ex.Message);
            }
        }

        public async void ReportMedia(GlobalSystemMediaTransportControlsSessionMediaProperties properties, GlobalSystemMediaTransportControlsSessionPlaybackStatus status, string processName)
        {
            if (status != GlobalSystemMediaTransportControlsSessionPlaybackStatus.Playing)
            {
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

            long timeStamp = DateTimeOffset.Now.ToUnixTimeMilliseconds();
            var httpClient = new HttpClient
            {
                Timeout = TimeSpan.FromSeconds(5),
            };
            httpClient.DefaultRequestHeaders.Add("User-Agent", UserAgent);
            try
            {
                var response = await httpClient.PostAsJsonAsync(Endpoint, new Dictionary<string, object>
                {
                    { "timestamp", timeStamp },
                    { "process", processName },
                    { "media", new Dictionary<string, string> {
                            { "title", properties.Title },
                            { "artist", properties.Artist },
                        }
                    },
                    { "key", ApiKey },
                });

                response.EnsureSuccessStatusCode();

                var data = await response.Content.ReadFromJsonAsync<Dictionary<string, JsonElement>>();

                if (data is null)
                {
                    throw new Exception("Api server return empty content");
                }

                if (data.TryGetValue("ok", out var ook))
                {
                    var ok = ook.GetInt32();
                    if (ok == 0)
                    {
                        if (data.TryGetValue("message", out var omsg))
                        {
                            var msg = omsg.ToString();
                            throw new Exception(msg);
                        }
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
}
