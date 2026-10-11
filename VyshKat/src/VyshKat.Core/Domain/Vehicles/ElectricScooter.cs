namespace VyshKat.Core.Domain.Vehicles;

/// <summary>Электросамокат.</summary>
public sealed class ElectricScooter : ElectricVehicle
{
    public ElectricScooter(
        string inventoryNumber,
        string name,
        int easeOfUse,
        decimal dailyEnergyConsumptionKwh,
        TechnicalCondition? condition = null)
        : base(inventoryNumber, name, easeOfUse, dailyEnergyConsumptionKwh, condition)
    {
    }

    public override string Kind => "Электросамокат";
}
