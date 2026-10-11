using VyshKat.Core.Fleet;
using VyshKat.Core.Reports;
using VyshKat.Core.Rules;

namespace VyshKat.ConsoleApp;

/// <summary>
/// Превращает данные отчётов в текст для консоли. Ничего не вычисляет —
/// все расчёты выполняет <see cref="FleetReportService"/> в ядре.
/// </summary>
public sealed class ReportPrinter
{
    private readonly FleetReportService _reports;
    private readonly FleetService _fleet;
    private readonly IBeginnerSuitabilityRule _beginnerRule;
    private readonly ConsoleUi _ui;

    public ReportPrinter(
        FleetReportService reports,
        FleetService fleet,
        IBeginnerSuitabilityRule beginnerRule,
        ConsoleUi ui)
    {
        _reports = reports;
        _fleet = fleet;
        _beginnerRule = beginnerRule;
        _ui = ui;
    }

    public static string FormatKwh(decimal kwh) => $"{kwh:N2} кВт·ч";

    public void PrintSummary()
    {
        var summary = _reports.GetSummary();
        _ui.Header("Сводка парка");
        _ui.Info($"Транспорт в активном парке: {summary.ActiveVehicles}");
        _ui.Info($"Вещи на балансе:            {summary.EquipmentItems}");
        _ui.Info($"Всего на балансе:           {summary.TotalOnBalance}");
        _ui.Info($"Отклонено техосмотром:      {summary.RejectedSubmissions} (не на балансе)");
        _ui.Info("По видам:");
        foreach (var kind in summary.ByKind)
        {
            _ui.Info($"  {kind.Kind,-20} {kind.Count,3} шт.");
        }
    }

    public void PrintInventory()
    {
        var items = _reports.GetInventoryList();
        _ui.Header("Инвентарные номера: транспорт и вещи");
        if (items.Count == 0)
        {
            _ui.Info("На балансе пока ничего нет.");
            return;
        }

        _ui.Info($"{"Инв. номер",-12} {"Вид",-18} Наименование");
        foreach (var item in items)
        {
            _ui.Info($"{item.InventoryNumber,-12} {item.Kind,-18} {item.Name}");
        }
    }

    public void PrintBeginnerFriendly()
    {
        var vehicles = _reports.GetBeginnerFriendlyVehicles();
        _ui.Header($"Транспорт для новичков (правило: {_beginnerRule.Description})");
        if (vehicles.Count == 0)
        {
            _ui.Info("В активном парке нет транспорта для новичков.");
            return;
        }

        foreach (var vehicle in vehicles)
        {
            _ui.Info($"{vehicle.InventoryNumber,-12} {vehicle.Kind,-18} {vehicle.Name} — простота {vehicle.EaseOfUse}/10");
        }

        var hidden = _fleet.ActiveVehicles.Count - vehicles.Count;
        if (hidden > 0)
        {
            _ui.Info($"Не подходят новичкам: {hidden} ед. активного транспорта.");
        }
    }

    public void PrintEnergy()
    {
        var report = _reports.GetDailyEnergyReport();
        _ui.Header("Суточное энергопотребление");
        _ui.Info("Учитываются все потребители энергии на балансе: активный транспорт и вещи.");
        _ui.Info("Отклонённый техосмотром транспорт и объекты без потребления (шлемы, велосипеды) не учитываются.");
        foreach (var entry in report.Entries)
        {
            _ui.Info($"{entry.InventoryNumber,-12} {entry.Kind,-18} {FormatKwh(entry.DailyKwh),14}  {entry.Name}");
        }

        _ui.Info(new string('-', 60));
        _ui.Info($"Транспорт: {FormatKwh(report.VehiclesTotalKwh)}");
        _ui.Info($"Вещи:      {FormatKwh(report.EquipmentTotalKwh)}");
        _ui.Info($"ИТОГО:     {FormatKwh(report.TotalKwh)} в сутки");
    }

    public void PrintInspectionLog()
    {
        _ui.Header("Журнал техосмотров");
        if (_fleet.InspectionLog.Count == 0)
        {
            _ui.Info("Техосмотров ещё не было.");
            return;
        }

        foreach (var result in _fleet.InspectionLog)
        {
            PrintAcceptance(result);
        }
    }

    public void PrintAcceptance(AcceptanceResult result)
    {
        if (result.IsAccepted)
        {
            _ui.Success($"{result.Vehicle} — техосмотр пройден, принят в активный парк.");
        }
        else
        {
            _ui.Warning($"{result.Vehicle} — отклонён техосмотром: {string.Join("; ", result.Inspection.Remarks)}.");
        }
    }
}
