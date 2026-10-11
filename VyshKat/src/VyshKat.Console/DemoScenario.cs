using VyshKat.Core.Domain.Items;
using VyshKat.Core.Domain.Vehicles;
using VyshKat.Core.Fleet;
using VyshKat.Core.Reports;

namespace VyshKat.ConsoleApp;

/// <summary>
/// Автоматический прогон всех сценариев задания: успешный и неуспешный техосмотр,
/// новичковый и «сложный» транспорт, вещь без энергопотребления,
/// дубликат номера и ошибка валидации. Работает на том же парке, что и меню.
/// </summary>
public sealed class DemoScenario
{
    private readonly FleetService _fleet;
    private readonly FleetReportService _reports;
    private readonly ReportPrinter _printer;
    private readonly ConsoleUi _ui;
    private int _runNumber;

    public DemoScenario(FleetService fleet, FleetReportService reports, ReportPrinter printer, ConsoleUi ui)
    {
        _fleet = fleet;
        _reports = reports;
        _printer = printer;
        _ui = ui;
    }

    public void Run()
    {
        _runNumber++;
        var prefix = $"D{_runNumber}";   // уникальные номера при повторном запуске демо

        _ui.Header($"Демонстрация, прогон №{_runNumber}");

        _ui.Info("Шаг 1. Исправный самокат для новичков (простота 8) → техосмотр пройден.");
        _printer.PrintAcceptance(_fleet.SubmitVehicle(
            new ElectricScooter($"{prefix}-SC", "ВышКат Lite (демо)", easeOfUse: 8, dailyEnergyConsumptionKwh: 0.50m)));

        _ui.Info("Шаг 2. Электровелосипед с повреждённой рамой и без света → отклонён, в парк не попадает.");
        var rejected = _fleet.SubmitVehicle(new ElectricBicycle(
            $"{prefix}-EB",
            "ВышКат E-Bike (после ДТП)",
            easeOfUse: 7,
            dailyEnergyConsumptionKwh: 0.90m,
            new TechnicalCondition(BrakesWorking: true, FrameIntact: false, LightsWorking: false)));
        _printer.PrintAcceptance(rejected);
        _ui.Info($"   В активном парке: {(_fleet.ActiveVehicles.Contains(rejected.Vehicle) ? "да" : "нет")}; " +
                 $"номер {rejected.Vehicle.InventoryNumber} занят: {(_fleet.IsInventoryNumberTaken(rejected.Vehicle.InventoryNumber) ? "да" : "нет")}.");

        _ui.Info("Шаг 3. Спортивный велосипед (простота 3) → принят, но новичкам не выдаётся.");
        _printer.PrintAcceptance(_fleet.SubmitVehicle(
            new Bicycle($"{prefix}-BK", "Шоссейный велосипед (демо)", easeOfUse: 3)));

        _ui.Info("Шаг 4. Шлем ставится на баланс без техосмотра и не влияет на энергопотребление.");
        var energyBefore = ReportEnergyTotal();
        var helmet = new Helmet($"{prefix}-HL", "Шлем детский, размер S");
        _fleet.AddEquipment(helmet);
        _ui.Success($"{helmet} поставлен на баланс. Энергия до: {ReportPrinter.FormatKwh(energyBefore)}, " +
                    $"после: {ReportPrinter.FormatKwh(ReportEnergyTotal())}.");

        _ui.Info("Шаг 5. Повторное использование номера SC-001 (в любом регистре) → отказ.");
        try
        {
            _fleet.SubmitVehicle(new ElectricScooter("sc-001", "Самокат-двойник", 7, 0.4m));
        }
        catch (DuplicateInventoryNumberException ex)
        {
            _ui.Error(ex.Message);
        }

        _ui.Info("Шаг 6. Недопустимая «простота» 11 → ошибка валидации, объект не создаётся.");
        try
        {
            _ = new ElectricScooter($"{prefix}-BAD", "Самокат с ошибкой", easeOfUse: 11, dailyEnergyConsumptionKwh: 0.4m);
        }
        catch (ArgumentException ex)
        {
            _ui.Error(UserMessage.From(ex));
        }

        _ui.Info("Шаг 7. Отрицательный расход энергии → ошибка валидации.");
        try
        {
            _ = new ChargingStation($"{prefix}-CS", "Станция с ошибкой", ports: 4, dailyEnergyConsumptionKwh: -1m);
        }
        catch (ArgumentException ex)
        {
            _ui.Error(UserMessage.From(ex));
        }

        _ui.Info("Шаг 8. Итоговые отчёты.");
        _printer.PrintSummary();
        _printer.PrintBeginnerFriendly();
        _printer.PrintEnergy();
        _printer.PrintInventory();
    }

    private decimal ReportEnergyTotal() => _reports.GetDailyEnergyReport().TotalKwh;
}
