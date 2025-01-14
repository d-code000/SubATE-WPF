using System.IO;
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

    private void Open_OnClick(object sender, RoutedEventArgs e)
    {
        var openFileDialog = new Microsoft.Win32.OpenFileDialog
        {
            Filter = "CSV Files (*.csv)|*.csv|All Files (*.*)|*.*",
            Title = "Open Subscriber Data"
        };

        if (openFileDialog.ShowDialog() == true)
        {
            try
            {
                var lines = File.ReadAllLines(openFileDialog.FileName);
                _viewModel.SubscribersTable.Clear();
                for (int i = 1; i < lines.Length; i++)
                {
                    var values = lines[i].Split(',');
                    var subscriber = new Subscriber
                    {
                        Id = int.Parse(values[0]),
                        Name = values[1],
                        Phone = values[2],
                        IsPremium = bool.Parse(values[3]),
                        Type = values[4] == "Legal" ? SubscriberType.Legal : SubscriberType.Natural,
                        RegistrationDate = DateOnly.Parse(values[5])
                    };
                    _viewModel.SubscribersTable.Add(subscriber);
                }

                MessageBox.Show("File opened successfully");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error opening file: {ex.Message}");
            }
        }
    }


    private void Save_OnClick(object sender, RoutedEventArgs e)
    {
        var saveFileDialog = new Microsoft.Win32.SaveFileDialog
        {
            Filter = "CSV Files (*.csv)|*.csv|All Files (*.*)|*.*",
            Title = "Save Subscriber Data"
        };

        if (saveFileDialog.ShowDialog() == true)
        {
            try
            {
                using (var writer = new StreamWriter(saveFileDialog.FileName))
                {
                    writer.WriteLine("Id,Name,Phone,IsPremium,Type,RegistrationDate");
                    
                    foreach (var subscriber in _viewModel.SubscribersTable)
                    {
                        var line = 
                                   $"{subscriber.Id}," +
                                   $"{subscriber.Name}," +
                                   $"{subscriber.Phone}," +
                                   $"{subscriber.IsPremium}," +
                                   $"{subscriber.Type}," +
                                   $"{subscriber.RegistrationDate}";
                        writer.WriteLine(line);
                    }
                }

                MessageBox.Show("File saved successfully");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error saving file: {ex.Message}");
            }
        }
    }



    private void Exit_OnClick(object sender, RoutedEventArgs e)
    {
        var result = MessageBox.Show(
            "Are you sure you want to exit?", 
            "Exit", 
            MessageBoxButton.YesNo, 
            MessageBoxImage.Question);
        if (result == MessageBoxResult.Yes)
        {
            Application.Current.Shutdown();
        }
    }

}