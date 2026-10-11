namespace VyshKat.Core.Domain.Items;

/// <summary>Шлем. Инвентаризируется, но энергию не потребляет.</summary>
public sealed class Helmet : Equipment
{
    public Helmet(string inventoryNumber, string name)
        : base(inventoryNumber, name)
    {
    }

    public override string Kind => "Шлем";
}
