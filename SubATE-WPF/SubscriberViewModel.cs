using System.Collections.ObjectModel;

namespace SubATE_WPF;

public class SubscriberViewModel
{
    public ObservableCollection<Subscriber> SubscribersTable { get; set; }
    
    public SubscriberViewModel()
    {
        SubscribersTable = new ObservableCollection<Subscriber>
        {
            new Subscriber(),
            new Subscriber(),
            new Subscriber()
        };
    }
}