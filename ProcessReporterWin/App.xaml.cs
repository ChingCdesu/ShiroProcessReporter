using ProcessReporterWin.Layouts;

namespace ProcessReporterWin;

public partial class App : Application
{
    public App()
    {
        InitializeComponent();

        MainPage = new Main();
    }
}