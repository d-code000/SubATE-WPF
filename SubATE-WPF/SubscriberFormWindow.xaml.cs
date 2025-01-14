using System.Windows;

namespace SubATE_WPF;

public partial class SubscriberFormWindow : Window
{
    public SubscriberFormWindow(string title, SubscriberViewModel viewModel)
    {
        InitializeComponent();
        Title = title;
        DataContext = viewModel;
    }

    private void SaveButton_Click(object sender, RoutedEventArgs e)
    {
        DialogResult = true;
    }
}