namespace SubATE_WPF;

public enum SubscriberType
{
    Natural,
    Legal
}

public static class SubscriberTypeExtensions
{
    public static IEnumerable<SubscriberType> GetValues()
    {
        return Enum.GetValues(typeof(SubscriberType)).Cast<SubscriberType>();
    }
}

