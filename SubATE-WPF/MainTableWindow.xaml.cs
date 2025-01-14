using System.Windows;

namespace SubATE_WPF;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainTableWindow : Window
{
    private SubscriberTableViewModel _viewModel = new();
    public MainTableWindow()
    {
        InitializeComponent();
        DataContext = _viewModel;
    }

    private void AddRow_OnClick(object sender, RoutedEventArgs e)
    {
        var newSubscriber = new Subscriber();
        var subscriberFormWindow = new SubscriberFormWindow("Add", new SubscriberViewModel(newSubscriber));
        if (subscriberFormWindow.ShowDialog() == true)
        {
            _viewModel.SubscribersTable.Add(newSubscriber);
        }
    }

    private void EditRow_OnClick(object sender, RoutedEventArgs e)
    {
        var subscriber = (Subscriber) SubscribersDataGrid.SelectedItem;
        if (subscriber == null)
        {
            MessageBox.Show("Please select a row to edit.");
            return;
        }
        var subscriberFormWindow = new SubscriberFormWindow("Edit", new SubscriberViewModel(subscriber));
        if (subscriberFormWindow.ShowDialog() == true)
        {
            SubscribersDataGrid.Items.Refresh();
        }
    }
}