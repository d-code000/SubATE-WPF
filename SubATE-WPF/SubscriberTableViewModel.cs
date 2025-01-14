using System.Collections.ObjectModel;

namespace SubATE_WPF;

public class SubscriberTableViewModel
{
    public ObservableCollection<Subscriber> SubscribersTable { get; set; }
    
    public SubscriberTableViewModel()
    {
        SubscribersTable = new ObservableCollection<Subscriber>();
    }
}