namespace SubATE_WPF;

public enum SubscriberType
{
    Физическое,
    Юридическое
}

public static class SubscriberTypeExtensions
{
    public static IEnumerable<SubscriberType> GetValues()
    {
        return Enum.GetValues(typeof(SubscriberType)).Cast<SubscriberType>();
    }
}

