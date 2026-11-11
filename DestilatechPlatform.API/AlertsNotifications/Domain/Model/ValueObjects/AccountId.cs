using DestilatechPlatform.API.Shared.Domain.Model.ValueObjects;

namespace DestilatechPlatform.API.AlertsNotifications.Domain.Model.ValueObjects;

public sealed record AccountId : IEntityId<int>
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