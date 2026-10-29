namespace DestilatechPlatform.API.AlertsNotifications.Domain.Model.ValueObjects;

public sealed record AlertMessage
{
    private const int MaxLength = 255;

    public string Value { get; }
    
    public AlertMessage(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("AlertMessage cannot be null, empty, or whitespace.", nameof(value));
        if (value.Trim().Length > MaxLength)
            throw new ArgumentException($"AlertMessage cannot exceed {MaxLength} characters.", nameof(value));
        Value = value.Trim();
    }
    
    public override string ToString() => Value;
}