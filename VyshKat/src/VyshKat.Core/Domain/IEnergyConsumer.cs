namespace VyshKat.Core.Domain;

/// <summary>
/// Устройство, потребляющее электроэнергию.
/// Реализуют только те объекты, которым это свойство действительно присуще
/// (электросамокат, зарядная станция), но не шлем и не обычный велосипед.
/// </summary>
public interface IEnergyConsumer
{
    /// <summary>Суточный расход энергии в кВт·ч. Всегда неотрицателен.</summary>
    decimal DailyEnergyConsumptionKwh { get; }
}
