namespace VyshKat.Core.Domain.Items;

/// <summary>
/// Вещь на балансе (не прокатный транспорт): шлем, зарядная станция, зарядный шкаф и т. п.
/// В отличие от <see cref="Vehicles.Vehicle"/>, не проходит техосмотр и не выдаётся в прокат.
/// </summary>
public abstract class Equipment : InventoryAsset
{
    protected Equipment(string inventoryNumber, string name)
        : base(inventoryNumber, name)
    {
    }
}
