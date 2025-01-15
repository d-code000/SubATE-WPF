using System.Diagnostics;
using System.Windows;
using System.Windows.Navigation;

namespace SubATE_WPF;

public partial class InfoWindow : Window
{
    public InfoWindow()
    {
        InitializeComponent();
    }

    private void Hyperlink_RequestNavigate(object sender, RequestNavigateEventArgs e)
    {
        Process.Start(new ProcessStartInfo
        {
            FileName = e.Uri.AbsoluteUri,
            UseShellExecute = true
        });
        e.Handled = true;
    }
}