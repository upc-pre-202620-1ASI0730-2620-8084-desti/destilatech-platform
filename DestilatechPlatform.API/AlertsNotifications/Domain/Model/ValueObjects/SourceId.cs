namespace DestilatechPlatform.API.AlertsNotifications.Domain.Model.ValueObjects;

public sealed record SourceId
{

    public int Value { get; }


    public SourceId(int value)
    {
        if (value <= 0)
            throw new ArgumentException("SourceId must be greater than zero.", nameof(value));
        Value = value;
    }

    public override string ToString() => Value.ToString();
}