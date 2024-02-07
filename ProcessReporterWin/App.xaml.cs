using ProcessReporterWin.Services;

namespace ProcessReporterWin
{
    public partial class App : Application
    {

        public App()
        {
            InitializeComponent();

            MainPage = new AppShell();
        }
    }
}
