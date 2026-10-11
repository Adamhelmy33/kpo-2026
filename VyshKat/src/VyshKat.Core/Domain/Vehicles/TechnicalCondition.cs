namespace VyshKat.Core.Domain.Vehicles;

/// <summary>
/// Техническое состояние транспорта на момент сдачи в парк.
/// Эти данные проверяет сервис техосмотра.
/// </summary>
public sealed record TechnicalCondition(bool BrakesWorking, bool FrameIntact, bool LightsWorking)
{
    /// <summary>Полностью исправное состояние.</summary>
    public static TechnicalCondition Good { get; } = new(true, true, true);
}
