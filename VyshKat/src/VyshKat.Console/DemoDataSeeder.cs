using VyshKat.Core.Domain.Items;
using VyshKat.Core.Domain.Vehicles;
using VyshKat.Core.Fleet;

namespace VyshKat.ConsoleApp;

/// <summary>
/// Стартовые данные, чтобы отчёты были содержательными сразу после запуска.
/// Транспорт подаётся через обычную приёмку, то есть тоже проходит техосмотр.
/// </summary>
public sealed class DemoDataSeeder
{
    private readonly FleetService _fleet;

    public DemoDataSeeder(FleetService fleet)
    {
        _fleet = fleet;
    }

    public IReadOnlyList<AcceptanceResult> Seed()
    {
        var results = new List<AcceptanceResult>
        {
            _fleet.SubmitVehicle(new ElectricScooter("SC-001", "ВышКат Lite", easeOfUse: 7, dailyEnergyConsumptionKwh: 0.45m)),
            _fleet.SubmitVehicle(new ElectricScooter("SC-002", "ВышКат Pro Sport", easeOfUse: 4, dailyEnergyConsumptionKwh: 0.60m)),
            _fleet.SubmitVehicle(new Bicycle("BK-001", "Городской велосипед «Покровка»", easeOfUse: 9)),
            _fleet.SubmitVehicle(new ElectricBicycle("EB-001", "ВышКат E-Bike", easeOfUse: 6, dailyEnergyConsumptionKwh: 0.80m)),

            // Неисправный самокат: техосмотр его отклонит, в парк он не попадёт.
            _fleet.SubmitVehicle(new ElectricScooter(
                "SC-003",
                "ВышКат Lite (б/у)",
                easeOfUse: 8,
                dailyEnergyConsumptionKwh: 0.40m,
                new TechnicalCondition(BrakesWorking: false, FrameIntact: true, LightsWorking: true))),
        };

        _fleet.AddEquipment(new Helmet("HL-001", "Шлем городской, размер M"));
        _fleet.AddEquipment(new Helmet("HL-002", "Шлем городской, размер L"));
        _fleet.AddEquipment(new ChargingStation("CS-001", "Док-станция у корпуса на Покровке", ports: 8, dailyEnergyConsumptionKwh: 12.0m));
        _fleet.AddEquipment(new ChargingCabinet("CC-001", "Зарядный шкаф в общежитии", batterySlots: 16, dailyEnergyConsumptionKwh: 6.5m));

        return results;
    }
}
