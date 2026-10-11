using VyshKat.Core.Domain.Vehicles;
using VyshKat.Core.Inspection;

namespace VyshKat.Core.Fleet;

/// <summary>Итог приёмки транспорта: какое ТС подавали и что решил техосмотр.</summary>
public sealed record AcceptanceResult(Vehicle Vehicle, InspectionResult Inspection)
{
    /// <summary>true — транспорт попал в активный парк.</summary>
    public bool IsAccepted => Inspection.IsApproved;
}
