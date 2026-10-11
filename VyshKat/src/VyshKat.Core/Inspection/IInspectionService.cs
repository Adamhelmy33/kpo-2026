using VyshKat.Core.Domain.Vehicles;

namespace VyshKat.Core.Inspection;

/// <summary>
/// Техосмотр: решает, можно ли принять транспорт в активный парк.
/// FleetService зависит от этой абстракции, а не от ServiceCenter (DIP),
/// поэтому в тестах её подменяют фейком.
/// </summary>
public interface IInspectionService
{
    InspectionResult Inspect(Vehicle vehicle);
}
