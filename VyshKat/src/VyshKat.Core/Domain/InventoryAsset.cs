namespace VyshKat.Core.Domain;

/// <summary>
/// Базовый класс для всего, что ставится на баланс: хранит и проверяет
/// инвентарный номер и наименование, чтобы не дублировать валидацию в наследниках.
/// </summary>
public abstract class InventoryAsset : IInventoryItem
{
    public const int MaxInventoryNumberLength = 32;

    protected InventoryAsset(string inventoryNumber, string name)
    {
        InventoryNumber = NormalizeInventoryNumber(inventoryNumber);
        Name = DomainGuard.NotBlank(name, nameof(name), "Наименование не может быть пустым.");
    }

    public string InventoryNumber { get; }

    public string Name { get; }

    public abstract string Kind { get; }

    public override string ToString() => $"{Kind} «{Name}» [{InventoryNumber}]";

    /// <summary>
    /// Приводит номер к единому виду (без пробелов по краям, в верхнем регистре),
    /// чтобы «sc-001» и «SC-001» считались одним и тем же номером.
    /// Допустимы латинские/кириллические буквы, цифры и дефис.
    /// </summary>
    private static string NormalizeInventoryNumber(string inventoryNumber)
    {
        var normalized = DomainGuard
            .NotBlank(inventoryNumber, nameof(inventoryNumber), "Инвентарный номер не может быть пустым.")
            .ToUpperInvariant();

        if (normalized.Length > MaxInventoryNumberLength)
        {
            throw new ArgumentException(
                $"Инвентарный номер не может быть длиннее {MaxInventoryNumberLength} символов.",
                nameof(inventoryNumber));
        }

        if (!normalized.All(c => char.IsLetterOrDigit(c) || c == '-'))
        {
            throw new ArgumentException(
                "Инвентарный номер может содержать только буквы, цифры и дефис.",
                nameof(inventoryNumber));
        }

        return normalized;
    }
}
