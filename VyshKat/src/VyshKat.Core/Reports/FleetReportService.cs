using VyshKat.Core.Domain;
using VyshKat.Core.Domain.Vehicles;
using VyshKat.Core.Fleet;
using VyshKat.Core.Rules;

namespace VyshKat.Core.Reports;

/// <summary>
/// Отчёты по парку. Только читает данные FleetService и ничего не меняет.
/// Работает через интерфейсы (IInventoryItem, IEnergyConsumer, Vehicle),
/// поэтому новые виды транспорта и вещей попадают в отчёты автоматически.
/// </summary>
public sealed class FleetReportService
{
    private readonly FleetService _fleet;
    private readonly IBeginnerSuitabilityRule _beginnerRule;

    public FleetReportService(FleetService fleet, IBeginnerSuitabilityRule beginnerRule)
    {
        _fleet = fleet ?? throw new ArgumentNullException(nameof(fleet));
        _beginnerRule = beginnerRule ?? throw new ArgumentNullException(nameof(beginnerRule));
    }

    public FleetSummary GetSummary()
    {
        var byKind = _fleet.InventoryItems
            .GroupBy(item => item.Kind)
            .Select(group => new KindCount(group.Key, group.Count()))
            .OrderByDescending(k => k.Count)
            .ThenBy(k => k.Kind, StringComparer.CurrentCulture)
            .ToList();

        return new FleetSummary(
            ActiveVehicles: _fleet.ActiveVehicles.Count,
            EquipmentItems: _fleet.EquipmentItems.Count,
            RejectedSubmissions: _fleet.InspectionLog.Count(r => !r.IsAccepted),
            ByKind: byKind);
    }

    /// <summary>Активный транспорт, который можно выдавать новичкам (самые простые — первыми).</summary>
    public IReadOnlyList<Vehicle> GetBeginnerFriendlyVehicles() =>
        _fleet.ActiveVehicles
            .Where(_beginnerRule.IsSuitableForBeginners)
            .OrderByDescending(v => v.EaseOfUse)
            .ThenBy(v => v.InventoryNumber, StringComparer.Ordinal)
            .ToList();

    public EnergyReport GetDailyEnergyReport()
    {
        var entries = _fleet.InventoryItems
            .Where(item => item is IEnergyConsumer)
            .Select(item => new EnergyEntry(
                item.InventoryNumber,
                item.Kind,
                item.Name,
                ((IEnergyConsumer)item).DailyEnergyConsumptionKwh,
                IsVehicle: item is Vehicle))
            .ToList();

        return new EnergyReport(entries);
    }

    /// <summary>Все объекты на балансе: сначала транспорт, затем вещи, внутри — по номеру.</summary>
    public IReadOnlyList<IInventoryItem> GetInventoryList() =>
        _fleet.InventoryItems
            .OrderBy(item => item is Vehicle ? 0 : 1)
            .ThenBy(item => item.InventoryNumber, StringComparer.Ordinal)
            .ToList();
}
