using VyshKat.Core.Domain.Vehicles;

namespace VyshKat.Core.Inspection;

/// <summary>
/// Реальный сервисный центр: проверяет тормоза, раму и световое оборудование.
/// Транспорт допускается только при исправности всех узлов.
/// </summary>
public sealed class ServiceCenter : IInspectionService
{
    public InspectionResult Inspect(Vehicle vehicle)
    {
        ArgumentNullException.ThrowIfNull(vehicle);

        var condition = vehicle.Condition;
        var remarks = new List<string>();

        if (!condition.BrakesWorking)
        {
            remarks.Add("неисправны тормоза");
        }

        if (!condition.FrameIntact)
        {
            remarks.Add("повреждена рама");
        }

        if (!condition.LightsWorking)
        {
            remarks.Add("не работает световое оборудование");
        }

        return remarks.Count == 0
            ? InspectionResult.Approved()
            : InspectionResult.Rejected(remarks.ToArray());
    }
}
