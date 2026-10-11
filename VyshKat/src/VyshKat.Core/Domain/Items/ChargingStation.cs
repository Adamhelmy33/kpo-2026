namespace VyshKat.Core.Domain.Items;

/// <summary>Док-станция (зарядная станция) для самокатов.</summary>
public sealed class ChargingStation : Equipment, IEnergyConsumer
{
    public ChargingStation(string inventoryNumber, string name, int ports, decimal dailyEnergyConsumptionKwh)
        : base(inventoryNumber, name)
    {
        Ports = DomainGuard.Positive(ports, nameof(ports), "У станции должен быть хотя бы один разъём.");
        DailyEnergyConsumptionKwh = DomainGuard.NonNegativeEnergy(
            dailyEnergyConsumptionKwh, nameof(dailyEnergyConsumptionKwh));
    }

    public override string Kind => "Зарядная станция";

    public int Ports { get; }

    public decimal DailyEnergyConsumptionKwh { get; }
}
