using DestilatechPlatform.API.Shared.Domain.Model.ValueObjects;

namespace DestilatechPlatform.API.AlertsNotifications.Domain.Model.ValueObjects;

public readonly record struct AlertId(int Value) : IEntityId<int>
{
    public static implicit operator int(AlertId id) => id.Value;
    public static implicit operator AlertId(int value) => new(value);

    public override string ToString() => Value.ToString();
}