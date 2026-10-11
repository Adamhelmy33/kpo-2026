namespace VyshKat.Core.Fleet;

/// <summary>Попытка поставить на баланс объект с уже занятым инвентарным номером.</summary>
public sealed class DuplicateInventoryNumberException : InvalidOperationException
{
    public DuplicateInventoryNumberException(string inventoryNumber)
        : base($"Инвентарный номер {inventoryNumber} уже занят другим объектом на балансе.")
    {
        InventoryNumber = inventoryNumber;
    }

    public string InventoryNumber { get; }
}
