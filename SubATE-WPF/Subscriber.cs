namespace SubATE_WPF;

public class Subscriber
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Phone { get; set; }
    public bool IsPremium { get; set; }
    public SubscriberType Type { get; set; }
    public DateOnly RegistrationDate { get; set; }
    

    public Subscriber(int id, string name, string phone, bool isPremium, SubscriberType type, DateOnly registrationDate)
    {
        Id = id;
        Name = name;
        Phone = phone;
        IsPremium = isPremium;
        Type = type;
        RegistrationDate = registrationDate;
    }

    public Subscriber()
    {
        Id = 0;
        Name = "None";
        Phone = "+7(000) 111-22-33";
        IsPremium = false;
        Type = SubscriberType.LegalPerson;
        RegistrationDate = DateOnly.FromDateTime(DateTime.Today);
    }
}