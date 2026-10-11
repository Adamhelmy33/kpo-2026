using VyshKat.Core.Domain.Vehicles;

namespace VyshKat.Core.Rules;

/// <summary>
/// Базовое правило из задания: «простота для новичка» ≥ 6.
/// Это единственное место в коде, где задан порог.
/// </summary>
public sealed class EaseOfUseBeginnerRule : IBeginnerSuitabilityRule
{
    public const int DefaultMinimumEaseOfUse = 6;

    public EaseOfUseBeginnerRule(int minimumEaseOfUse = DefaultMinimumEaseOfUse)
    {
        if (minimumEaseOfUse is < Vehicle.MinEaseOfUse or > Vehicle.MaxEaseOfUse)
        {
            throw new ArgumentOutOfRangeException(
                nameof(minimumEaseOfUse),
                minimumEaseOfUse,
                $"Порог должен быть от {Vehicle.MinEaseOfUse} до {Vehicle.MaxEaseOfUse}.");
        }

        MinimumEaseOfUse = minimumEaseOfUse;
    }

    public int MinimumEaseOfUse { get; }

    public string Description => $"простота ≥ {MinimumEaseOfUse}";

    public bool IsSuitableForBeginners(Vehicle vehicle)
    {
        ArgumentNullException.ThrowIfNull(vehicle);
        return vehicle.EaseOfUse >= MinimumEaseOfUse;
    }
}
