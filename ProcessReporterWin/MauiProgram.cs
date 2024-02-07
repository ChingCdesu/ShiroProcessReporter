using Microsoft.Extensions.Logging;
using Microsoft.Maui.LifecycleEvents;
using ProcessReporterWin.Helper;
using ProcessReporterWin.Services;

namespace ProcessReporterWin
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();

            builder.ConfigureLifecycleEvents(lifecycle =>
            {
                lifecycle.AddWindows(windows =>
                {
                    windows.OnWindowCreated(window =>
                    {
                        var handle = WinRT.Interop.WindowNative.GetWindowHandle(window);
                        var id = Microsoft.UI.Win32Interop.GetWindowIdFromWindow(handle);

                        var appWindow = Microsoft.UI.Windowing.AppWindow.GetFromWindowId(id);

                        appWindow.Closing += (_, e) =>
                        {
                            // 按关闭按钮最小化到托盘
                            e.Cancel = true;
                            appWindow.Hide();
                        };
                    });
                });
            });

            builder.Services.AddSingleton<ProcessTraceService>();
            builder.Services.AddSingleton<MediaTraceService>();
            builder.Services.AddSingleton<ReportService>();
            builder.Services.AddSingleton<TraceWorkerService>();
                        
            builder.Services.AddTransient<MainPage>();

            builder
                .UseMauiApp<App>()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                });

#if DEBUG
    		builder.Logging.AddDebug().AddConsole();
#endif

            var app = builder.Build();

            ServiceHelper.Initialize(app.Services);

            return app;
        }
    }
}
