namespace VyshKat.Core.Domain.Vehicles;

/// <summary>
/// Обычный велосипед. Энергию не потребляет, поэтому не реализует
/// <see cref="IEnergyConsumer"/> (ISP: не навязываем ненужный контракт).
/// </summary>
public sealed class Bicycle : Vehicle
{
    public Bicycle(string inventoryNumber, string name, int easeOfUse, TechnicalCondition? condition = null)
        : base(inventoryNumber, name, easeOfUse, condition)
    {
    }

    public override string Kind => "Велосипед";
}
