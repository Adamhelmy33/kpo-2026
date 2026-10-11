namespace VyshKat.Core.Domain.Vehicles;

/// <summary>Электровелосипед.</summary>
public sealed class ElectricBicycle : ElectricVehicle
{
    public ElectricBicycle(
        string inventoryNumber,
        string name,
        int easeOfUse,
        decimal dailyEnergyConsumptionKwh,
        TechnicalCondition? condition = null)
        : base(inventoryNumber, name, easeOfUse, dailyEnergyConsumptionKwh, condition)
    {
    }

    public override string Kind => "Электровелосипед";
}
