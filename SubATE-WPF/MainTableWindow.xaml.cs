using System.Collections.ObjectModel;
using System.Data;
using System.IO;
using System.Text;
using System.Windows;
using Database;

namespace SubATE_WPF;

public partial class MainTableWindow
{
    private readonly SubscriberTableViewModel _viewModel = new();
    private string _currentFilePath = string.Empty;
    private Connection _connection;
    private int _currentLineIndex;
    private int _fileLinesCount;
    private const int LinesPerPage = 1000;

    public MainTableWindow()
    {
        InitializeComponent();
        DataContext = _viewModel;
        _connection = new Connection();
        _viewModel.SubscribersTable = new ObservableCollection<Subscriber>(_connection.GetSubscribersFromDataTable());
    }

    private void AddRow_OnClick(object sender, RoutedEventArgs e)
    {
        var newSubscriber = new Subscriber();
        var subscriberFormWindow = new SubscriberFormWindow("Добавить", new SubscriberViewModel(newSubscriber));
        if (subscriberFormWindow.ShowDialog() == true)
        {
            _connection.AddSubscriber(newSubscriber);
            UpdateViewModel();
        }
    }

    private void EditRow_OnClick(object sender, RoutedEventArgs e)
    {
        var subscriber = GetSelectedSubscriber();
        if (subscriber == null) return;

        var subscriberFormWindow = new SubscriberFormWindow("Редактировать", new SubscriberViewModel(subscriber));
        if (subscriberFormWindow.ShowDialog() == true)
        {
            _connection.UpdateSubscriber(subscriber);
            UpdateViewModel();
        }
    }

    private void UpdateViewModel()
    {
        _viewModel.SubscribersTable.Clear();
        foreach (var subscriber in _connection.GetSubscribersFromDataTable())
        {
            _viewModel.SubscribersTable.Add(subscriber);
        }
    }

    private Subscriber? GetSelectedSubscriber()
    {
        var subscriber = (Subscriber) SubscribersDataGrid.SelectedItem;
        if (subscriber == null)
        {
            MessageBox.Show("Пожалуйста, выберите строку для редактирования.");
        }
        return subscriber;
    }

    private void Open_OnClick(object sender, RoutedEventArgs e)
    {
        var openFileDialog = CreateFileDialog("Открыть данные абонентов");
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
                MessageBox.Show($"Файл не найден: {ex.Message}");
            }
            catch (UnauthorizedAccessException ex)
            {
                MessageBox.Show($"Ошибка доступа: {ex.Message}");
            }
            catch (IOException ex)
            {
                MessageBox.Show($"Ошибка ввода-вывода: {ex.Message}");
            }
        }
    }

    private Microsoft.Win32.OpenFileDialog CreateFileDialog(string title)
    {
        return new Microsoft.Win32.OpenFileDialog
        {
            Filter = "CSV файлы (*.csv)|*.csv|Все файлы (*.*)|*.*",
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

            foreach (var line in lines.Skip(1))
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
            MessageBox.Show($"Ошибка при чтении файла: {ex.Message}");
        }
    }

    private Subscriber? ParseSubscriber(string line)
    {
        var values = line.Split(',');
        try
        {
            if (values.Length != 6) throw new DataException("Неверная длина данных");
            return new Subscriber
            {
                Id = int.Parse(values[0]),
                Name = values[1],
                Phone = values[2],
                IsPremium = bool.Parse(values[3]),
                Type = values[4] == "Юридическое" ? SubscriberType.Юридическое : SubscriberType.Физическое,
                RegistrationDate = DateOnly.Parse(values[5])
            };
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Ошибка при разборе данных абонента: {ex.Message}");
            return null;
        }
    }

    private void Save_OnClick(object sender, RoutedEventArgs e)
    {
        var saveFileDialog = CreateFileDialog("Сохранить данные абонентов");
        if (saveFileDialog.ShowDialog() == true)
        {
            try
            {
                SaveSubscribersToFile(saveFileDialog.FileName);
            }
            catch (FileNotFoundException ex)
            {
                MessageBox.Show($"Файл не найден: {ex.Message}");
            }
            catch (UnauthorizedAccessException ex)
            {
                MessageBox.Show($"Ошибка доступа: {ex.Message}");
            }
            catch (IOException ex)
            {
                MessageBox.Show($"Ошибка ввода-вывода: {ex.Message}");
            }
        }
    }

    private void SaveSubscribersToFile(string filePath)
    {
        using (var writer = new StreamWriter(filePath))
        {
            var header = "Id,Имя,Телефон,Премиум,Тип,Дата регистрации";
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
            "Вы уверены, что хотите выйти?", 
            "Выход", 
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
            _connection.DeleteSubscriber(subscriber.Id);
            UpdateViewModel();
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
