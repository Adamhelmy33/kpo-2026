using VyshKat.Core.Domain.Vehicles;

namespace VyshKat.Core.Rules;

/// <summary>
/// Правило «можно выдавать новичку». Вынесено в абстракцию, потому что задание
/// прямо допускает усложнение правила (например, учитывать скорость или вес).
/// </summary>
public interface IBeginnerSuitabilityRule
{
    /// <summary>Описание правила для отчётов, например «простота ≥ 6».</summary>
    string Description { get; }

    bool IsSuitableForBeginners(Vehicle vehicle);
}
