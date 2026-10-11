using VyshKat.Core.Domain.Items;
using VyshKat.Core.Domain.Vehicles;

namespace VyshKat.ConsoleApp;

/// <summary>Данные, введённые пользователем для нового транспорта.</summary>
public sealed record VehicleDraft(
    string InventoryNumber,
    string Name,
    int EaseOfUse,
    decimal DailyEnergyKwh,
    TechnicalCondition Condition);

/// <summary>Пункт меню «вид транспорта»: как назвать и как создать объект.</summary>
public sealed record VehicleKindOption(string Title, bool ConsumesEnergy, Func<VehicleDraft, Vehicle> Create);

/// <summary>Данные, введённые пользователем для новой вещи.</summary>
public sealed record EquipmentDraft(string InventoryNumber, string Name, int Capacity, decimal DailyEnergyKwh);

/// <summary>Пункт меню «вид вещи». CapacityPrompt = null — вещь без ёмкости (шлем).</summary>
public sealed record EquipmentKindOption(
    string Title,
    string? CapacityPrompt,
    bool ConsumesEnergy,
    Func<EquipmentDraft, Equipment> Create);

/// <summary>
/// Каталоги видов для консольного ввода. Новый вид транспорта появляется здесь
/// одной строкой; FleetService и отчёты при этом не меняются.
/// </summary>
public static class Catalogs
{
    public static IReadOnlyList<VehicleKindOption> VehicleKinds { get; } =
    [
        new("Электросамокат", ConsumesEnergy: true,
            d => new ElectricScooter(d.InventoryNumber, d.Name, d.EaseOfUse, d.DailyEnergyKwh, d.Condition)),
        new("Велосипед", ConsumesEnergy: false,
            d => new Bicycle(d.InventoryNumber, d.Name, d.EaseOfUse, d.Condition)),
        new("Электровелосипед", ConsumesEnergy: true,
            d => new ElectricBicycle(d.InventoryNumber, d.Name, d.EaseOfUse, d.DailyEnergyKwh, d.Condition)),
    ];

    public static IReadOnlyList<EquipmentKindOption> EquipmentKinds { get; } =
    [
        new("Шлем", CapacityPrompt: null, ConsumesEnergy: false,
            d => new Helmet(d.InventoryNumber, d.Name)),
        new("Зарядная станция", "Число разъёмов: ", ConsumesEnergy: true,
            d => new ChargingStation(d.InventoryNumber, d.Name, d.Capacity, d.DailyEnergyKwh)),
        new("Зарядный шкаф", "Число ячеек для аккумуляторов: ", ConsumesEnergy: true,
            d => new ChargingCabinet(d.InventoryNumber, d.Name, d.Capacity, d.DailyEnergyKwh)),
    ];
}
