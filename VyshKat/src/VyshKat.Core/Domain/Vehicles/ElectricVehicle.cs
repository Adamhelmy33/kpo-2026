namespace VyshKat.Core.Domain.Vehicles;

/// <summary>
/// Транспорт с электроприводом. Хранит и проверяет суточный расход энергии,
/// чтобы самокат, электровелосипед и будущие виды (моноколесо, электроскутер)
/// не дублировали эту проверку.
/// </summary>
public abstract class ElectricVehicle : Vehicle, IEnergyConsumer
{
    protected ElectricVehicle(
        string inventoryNumber,
        string name,
        int easeOfUse,
        decimal dailyEnergyConsumptionKwh,
        TechnicalCondition? condition)
        : base(inventoryNumber, name, easeOfUse, condition)
    {
        DailyEnergyConsumptionKwh = DomainGuard.NonNegativeEnergy(
            dailyEnergyConsumptionKwh, nameof(dailyEnergyConsumptionKwh));
    }

    public decimal DailyEnergyConsumptionKwh { get; }
}
