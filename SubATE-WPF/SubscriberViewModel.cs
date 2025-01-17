using System.ComponentModel;

namespace SubATE_WPF;

public sealed class SubscriberViewModel(Subscriber subscriber) : INotifyPropertyChanged
{
    public string Id
    {
        get => $"{subscriber.Id}";
        set
        {
            if (int.TryParse(value, out var id))
            {
                if (id > 0)
                {
                    subscriber.Id = id;
                    OnPropertyChanged(nameof(Id));
                }
            }
        }
    }
    
    public string Name
    {
        get => subscriber.Name;
        set
        {
            if (value.Length > 0 && value.Length <= 50)
            {
                if (value.All(x => char.IsLetter(x) || char.IsWhiteSpace(x)))
                {
                    subscriber.Name = value;
                    OnPropertyChanged(nameof(Name));
                }
            }
        }
    }

    public string Phone
    {
        get => subscriber.Phone;
        set
        {
            var regex = new System.Text.RegularExpressions.Regex(@"^\+7\(\d{3}\)\d{3}-\d{2}-\d{2}$");
            if (regex.IsMatch(value))
            {
                subscriber.Phone = value;
                OnPropertyChanged(nameof(Phone));
            }
        }
    }

    public bool IsPremium
    {
        get => subscriber.IsPremium;
        set
        {
            subscriber.IsPremium = value;
            OnPropertyChanged(nameof(IsPremium));
        }
    }
    
    public SubscriberType Type
    {
        get => subscriber.Type;
        set
        {
            subscriber.Type = value;
            OnPropertyChanged(nameof(Type));
        }
    }
    
    public DateTime RegistrationDate
    {
        get => subscriber.RegistrationDate.ToDateTime(new TimeOnly());
        set
        {
            if (value <= DateTime.Now)
            {
                subscriber.RegistrationDate = DateOnly.FromDateTime(value);
                OnPropertyChanged(nameof(RegistrationDate));
            }
        }
    }
    
    public IEnumerable<SubscriberType> SubscriberTypes => SubscriberTypeExtensions.GetValues();
    
    public event PropertyChangedEventHandler? PropertyChanged;

    private void OnPropertyChanged(string propertyName)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}