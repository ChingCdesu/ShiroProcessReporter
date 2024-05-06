namespace ProcessReporterWin.Helper;

public static class ServiceHelper
{
    public static IServiceProvider Services { get; private set; }

    public static void Initialize(IServiceProvider serviceProvider)
    {
        Services = serviceProvider;
    }

    public static T? GetService<T>()
    {
        return Services.GetService<T>();
    }
}