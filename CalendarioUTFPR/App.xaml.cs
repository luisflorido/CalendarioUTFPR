using System.Reflection;
using System.Windows;

namespace CalendarioUTFPR
{
    /// <summary>
    /// Interação lógica para App.xaml
    /// </summary>
    public partial class App : Application
    {

        public static MainWindow mw;

        private void App_Startup(object sender, StartupEventArgs e)
        {
            StartupRegistry();
            bool minimized = false;

            foreach (string s in e.Args)
            {
                if (s == "-silent")
                    minimized = true;
            }

            mw = new MainWindow(minimized);
            if (minimized)
            {
                mw.WindowState = WindowState.Minimized;
            }
        }

        private void StartupRegistry()
        {
            try
            {
                Microsoft.Win32.RegistryKey key = Microsoft.Win32.Registry.CurrentUser.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true);
                Assembly curAssembly = Assembly.GetExecutingAssembly();
                key.SetValue(curAssembly.GetName().Name, curAssembly.Location+" -silent");
            }
            catch { }
        }
    }
}
