using VyshKat.Core.Domain.Vehicles;
using VyshKat.Core.Inspection;

namespace VyshKat.Tests.Fakes;

/// <summary>
/// Ручной фейк техосмотра. Возвращает заранее заданное решение и запоминает,
/// какой транспорт ему передавали, — так тест проверяет, что FleetService
/// действительно спрашивает подставленную зависимость.
/// </summary>
public sealed class FakeInspectionService : IInspectionService
{
    private readonly Func<Vehicle, InspectionResult> _decide;
    private readonly List<Vehicle> _inspected = [];

    private FakeInspectionService(Func<Vehicle, InspectionResult> decide)
    {
        _decide = decide;
    }

    public IReadOnlyList<Vehicle> InspectedVehicles => _inspected;

    public static FakeInspectionService AlwaysApprove() => new(_ => InspectionResult.Approved());

    public static FakeInspectionService AlwaysReject(string reason = "фейковый отказ") =>
        new(_ => InspectionResult.Rejected(reason));

    /// <summary>Одобряет только транспорт с указанными инвентарными номерами.</summary>
    public static FakeInspectionService ApproveOnly(params string[] inventoryNumbers) =>
        new(v => inventoryNumbers.Contains(v.InventoryNumber, StringComparer.OrdinalIgnoreCase)
            ? InspectionResult.Approved()
            : InspectionResult.Rejected("не в списке разрешённых"));

    public InspectionResult Inspect(Vehicle vehicle)
    {
        _inspected.Add(vehicle);
        return _decide(vehicle);
    }
}
