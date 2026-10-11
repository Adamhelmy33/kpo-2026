namespace VyshKat.Core.Domain.Items;

/// <summary>Зарядный шкаф для съёмных аккумуляторов.</summary>
public sealed class ChargingCabinet : Equipment, IEnergyConsumer
{
    public ChargingCabinet(string inventoryNumber, string name, int batterySlots, decimal dailyEnergyConsumptionKwh)
        : base(inventoryNumber, name)
    {
        BatterySlots = DomainGuard.Positive(
            batterySlots, nameof(batterySlots), "В шкафу должна быть хотя бы одна ячейка.");
        DailyEnergyConsumptionKwh = DomainGuard.NonNegativeEnergy(
            dailyEnergyConsumptionKwh, nameof(dailyEnergyConsumptionKwh));
    }

    public override string Kind => "Зарядный шкаф";

    public int BatterySlots { get; }

    public decimal DailyEnergyConsumptionKwh { get; }
}
