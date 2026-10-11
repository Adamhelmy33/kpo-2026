namespace VyshKat.Core.Reports;

/// <summary>Количество объектов одного вида.</summary>
public sealed record KindCount(string Kind, int Count);

/// <summary>Сводка по числу единиц на балансе.</summary>
public sealed record FleetSummary(
    int ActiveVehicles,
    int EquipmentItems,
    int RejectedSubmissions,
    IReadOnlyList<KindCount> ByKind)
{
    public int TotalOnBalance => ActiveVehicles + EquipmentItems;
}

/// <summary>Строка энергоотчёта: один потребитель энергии.</summary>
public sealed record EnergyEntry(string InventoryNumber, string Kind, string Name, decimal DailyKwh, bool IsVehicle);

/// <summary>
/// Суточное энергопотребление всего, что стоит на балансе и реализует IEnergyConsumer:
/// активного транспорта и вещей (станций, шкафов). Отклонённый транспорт не входит.
/// </summary>
public sealed record EnergyReport(IReadOnlyList<EnergyEntry> Entries)
{
    public decimal VehiclesTotalKwh => Entries.Where(e => e.IsVehicle).Sum(e => e.DailyKwh);

    public decimal EquipmentTotalKwh => Entries.Where(e => !e.IsVehicle).Sum(e => e.DailyKwh);

    public decimal TotalKwh => Entries.Sum(e => e.DailyKwh);
}
