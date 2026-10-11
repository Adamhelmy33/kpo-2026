namespace VyshKat.Core.Domain;

/// <summary>
/// Объект, стоящий на балансе «ВышКат» и имеющий инвентарный номер.
/// Реализуют транспорт и вещи; сотрудники — нет.
/// </summary>
public interface IInventoryItem
{
    /// <summary>Уникальный инвентарный номер (хранится в верхнем регистре).</summary>
    string InventoryNumber { get; }

    /// <summary>Наименование объекта, например «Ninebot Max G30».</summary>
    string Name { get; }

    /// <summary>Вид объекта для отчётов, например «Электросамокат» или «Шлем».</summary>
    string Kind { get; }
}
