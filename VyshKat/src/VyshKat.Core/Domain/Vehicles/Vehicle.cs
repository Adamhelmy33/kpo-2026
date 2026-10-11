namespace VyshKat.Core.Domain.Vehicles;

/// <summary>
/// Прокатный транспорт. Общая абстракция для самоката, велосипеда, электровелосипеда
/// и любых будущих видов. Сервис парка работает только с этим типом,
/// поэтому новый вид транспорта = новый класс-наследник, без правки сервиса.
/// </summary>
public abstract class Vehicle : InventoryAsset
{
    public const int MinEaseOfUse = 1;
    public const int MaxEaseOfUse = 10;

    protected Vehicle(string inventoryNumber, string name, int easeOfUse, TechnicalCondition? condition)
        : base(inventoryNumber, name)
    {
        if (easeOfUse is < MinEaseOfUse or > MaxEaseOfUse)
        {
            throw new ArgumentOutOfRangeException(
                nameof(easeOfUse),
                easeOfUse,
                $"Простота для новичка должна быть от {MinEaseOfUse} до {MaxEaseOfUse}.");
        }

        EaseOfUse = easeOfUse;
        Condition = condition ?? TechnicalCondition.Good;
    }

    /// <summary>Простота для новичка по шкале 1–10.</summary>
    public int EaseOfUse { get; }

    /// <summary>Заявленное техническое состояние, которое проверяется на техосмотре.</summary>
    public TechnicalCondition Condition { get; }
}
