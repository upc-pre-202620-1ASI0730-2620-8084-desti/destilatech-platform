namespace DestilatechPlatform.API.AlertsNotifications.Domain.Model.ValueObjects;

public sealed record AccountId
{

    public int Value { get; }
    
    public AccountId(int value)
    {
        if (value <= 0)
            throw new ArgumentException("AccountId must be greater than zero.", nameof(value));
        Value = value;
    }
    
    public override string ToString() => Value.ToString();
}