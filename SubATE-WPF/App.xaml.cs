using System.Windows;
using DotNetEnv;

namespace SubATE_WPF;

/// <summary>
/// Interaction logic for App.xaml
/// </summary>
public partial class App : Application
{
    private void Application_Startup(object sender, StartupEventArgs e)
    {
        Env.Load();
    }
}