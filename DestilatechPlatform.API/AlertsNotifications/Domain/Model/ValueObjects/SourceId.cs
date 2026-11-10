using DestilatechPlatform.API.Shared.Domain.Model.ValueObjects;

namespace DestilatechPlatform.API.AlertsNotifications.Domain.Model.ValueObjects;


public sealed record SourceId : IEntityId<int>
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