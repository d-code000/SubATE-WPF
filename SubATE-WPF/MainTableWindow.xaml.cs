using System.IO;
using System.Text;
using System.Windows;

namespace SubATE_WPF;

public partial class MainTableWindow
{
    private readonly SubscriberTableViewModel _viewModel = new();
    private string _currentFilePath = string.Empty;
    private int _currentLineIndex;
    private int _fileLinesCount;
    private const int LinesPerPage = 1000;

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
        var subscriber = GetSelectedSubscriber();
        if (subscriber == null) return;

        var subscriberFormWindow = new SubscriberFormWindow("Edit", new SubscriberViewModel(subscriber));
        if (subscriberFormWindow.ShowDialog() == true)
        {
            SubscribersDataGrid.Items.Refresh();
        }
    }

    private Subscriber? GetSelectedSubscriber()
    {
        var subscriber = (Subscriber) SubscribersDataGrid.SelectedItem;
        if (subscriber == null)
        {
            MessageBox.Show("Please select a row to edit.");
        }
        return subscriber;
    }

    private void Open_OnClick(object sender, RoutedEventArgs e)
    {
        var openFileDialog = CreateFileDialog("Open Subscriber Data");
        if (openFileDialog.ShowDialog() == true)
        {
            try
            {
                _currentLineIndex = 0;
                _currentFilePath = openFileDialog.FileName;
                _fileLinesCount = File.ReadLines(_currentFilePath).Count();
                LoadSubscribers();
            }
            catch (FileNotFoundException ex)
            {
                MessageBox.Show($"File not found: {ex.Message}");
            }
            catch (UnauthorizedAccessException ex)
            {
                MessageBox.Show($"Access error: {ex.Message}");
            }
            catch (IOException ex)
            {
                MessageBox.Show($"I/O error: {ex.Message}");
            }
        }
    }

    private Microsoft.Win32.OpenFileDialog CreateFileDialog(string title)
    {
        return new Microsoft.Win32.OpenFileDialog
        {
            Filter = "CSV Files (*.csv)|*.csv|All Files (*.*)|*.*",
            Title = title
        };
    }

    private void LoadSubscribers()
    {
        if (!File.Exists(_currentFilePath)) return;
        _viewModel.SubscribersTable.Clear();
        try
        {
            var lines = File.ReadLines(_currentFilePath)
                .Skip(_currentLineIndex)
                .Take(LinesPerPage + 1)
                .ToArray();

            foreach (var line in lines.Skip(1)) // Skip header line
            {
                var subscriber = ParseSubscriber(line);
                if (subscriber != null)
                {
                    _viewModel.SubscribersTable.Add(subscriber);
                }
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Error reading file: {ex.Message}");
        }
    }

    private Subscriber? ParseSubscriber(string line)
    {
        var values = line.Split(',');

        if (values.Length != 6) return null;

        try
        {
            return new Subscriber
            {
                Id = int.Parse(values[0]),
                Name = values[1],
                Phone = values[2],
                IsPremium = bool.Parse(values[3]),
                Type = values[4] == "Legal" ? SubscriberType.Legal : SubscriberType.Natural,
                RegistrationDate = DateOnly.Parse(values[5])
            };
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Error parsing subscriber data: {ex.Message}");
            return null;
        }
    }

    private void Save_OnClick(object sender, RoutedEventArgs e)
    {
        var saveFileDialog = CreateFileDialog("Save Subscriber Data");
        if (saveFileDialog.ShowDialog() == true)
        {
            try
            {
                SaveSubscribersToFile(saveFileDialog.FileName);
            }
            catch (FileNotFoundException ex)
            {
                MessageBox.Show($"File not found: {ex.Message}");
            }
            catch (UnauthorizedAccessException ex)
            {
                MessageBox.Show($"Access error: {ex.Message}");
            }
            catch (IOException ex)
            {
                MessageBox.Show($"I/O error: {ex.Message}");
            }
        }
    }

    private void SaveSubscribersToFile(string filePath)
    {
        using (var writer = new StreamWriter(filePath))
        {
            var header = "Id,Name,Phone,IsPremium,Type,RegistrationDate";
            writer.WriteLine(header);

            var stringBuilder = new StringBuilder();
            foreach (var subscriber in _viewModel.SubscribersTable)
            {
                stringBuilder.Clear();
                stringBuilder.Append(subscriber.Id).Append(',')
                    .Append(subscriber.Name).Append(',')
                    .Append(subscriber.Phone).Append(',')
                    .Append(subscriber.IsPremium).Append(',')
                    .Append(subscriber.Type).Append(',')
                    .Append(subscriber.RegistrationDate);
                writer.WriteLine(stringBuilder.ToString());
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

    private void DeleteRow_OnClick(object sender, RoutedEventArgs e)
    {
        var subscriber = GetSelectedSubscriber();
        if (subscriber != null)
        {
            _viewModel.SubscribersTable.Remove(subscriber);
        }
    }

    private void About_OnClick(object sender, RoutedEventArgs e)
    {
        var aboutWindow = new InfoWindow();
        aboutWindow.ShowDialog();
    }

    private void UpTable_OnClick(object sender, RoutedEventArgs e)
    {
        _currentLineIndex -= LinesPerPage;
        _currentLineIndex = Math.Max(0, _currentLineIndex);
        LoadSubscribers();
    }

    private void DownTable_OnClick(object sender, RoutedEventArgs e)
    {
        if (_currentLineIndex + LinesPerPage < _fileLinesCount)
        {
            _currentLineIndex += LinesPerPage;
            LoadSubscribers();
        }
    }
}
