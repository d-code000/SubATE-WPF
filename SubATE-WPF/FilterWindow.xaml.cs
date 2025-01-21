using System.Windows;

namespace SubATE_WPF;

public partial class FilterWindow : Window
{
    public FilterWindow(SubscriberViewModel viewModel)
    {
        InitializeComponent();
        DataContext = viewModel;
    }

    private void Filter_OnClick(object sender, RoutedEventArgs e)
    {
        DialogResult = true;
        
    }
}