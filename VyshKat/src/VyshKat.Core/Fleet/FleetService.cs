using VyshKat.Core.Domain;
using VyshKat.Core.Domain.Items;
using VyshKat.Core.Domain.Vehicles;
using VyshKat.Core.Inspection;

namespace VyshKat.Core.Fleet;

/// <summary>
/// Учёт парка: приёмка транспорта через техосмотр и постановка вещей на баланс.
/// Гарантирует два инварианта:
/// 1) в активный парк попадает только транспорт, одобренный техосмотром;
/// 2) инвентарные номера уникальны среди всех объектов на балансе.
/// Хранит данные в памяти (база данных заданием не требуется).
/// </summary>
public sealed class FleetService
{
    private readonly IInspectionService _inspectionService;
    private readonly List<Vehicle> _activeVehicles = [];
    private readonly List<Equipment> _equipment = [];
    private readonly List<AcceptanceResult> _inspectionLog = [];
    private readonly HashSet<string> _usedInventoryNumbers = new(StringComparer.OrdinalIgnoreCase);

    public FleetService(IInspectionService inspectionService)
    {
        _inspectionService = inspectionService ?? throw new ArgumentNullException(nameof(inspectionService));
    }

    /// <summary>Транспорт, прошедший техосмотр.</summary>
    public IReadOnlyList<Vehicle> ActiveVehicles => _activeVehicles.AsReadOnly();

    /// <summary>Вещи на балансе (шлемы, станции, шкафы...).</summary>
    public IReadOnlyList<Equipment> EquipmentItems => _equipment.AsReadOnly();

    /// <summary>Все объекты на балансе: активный транспорт + вещи.</summary>
    public IReadOnlyList<IInventoryItem> InventoryItems =>
        _activeVehicles.Cast<IInventoryItem>().Concat(_equipment).ToList().AsReadOnly();

    /// <summary>История всех попыток приёмки, включая отклонённые.</summary>
    public IReadOnlyList<AcceptanceResult> InspectionLog => _inspectionLog.AsReadOnly();

    /// <summary>
    /// Подаёт транспорт на приёмку. Сначала проверяется номер (зачем осматривать то,
    /// что всё равно нельзя поставить на баланс), затем проводится техосмотр.
    /// Отклонённый транспорт не попадает в парк и не занимает номер: после ремонта
    /// его можно подать повторно.
    /// </summary>
    /// <exception cref="DuplicateInventoryNumberException">Номер уже занят.</exception>
    public AcceptanceResult SubmitVehicle(Vehicle vehicle)
    {
        ArgumentNullException.ThrowIfNull(vehicle);
        EnsureInventoryNumberIsFree(vehicle.InventoryNumber);

        var inspection = _inspectionService.Inspect(vehicle)
            ?? throw new InvalidOperationException("Сервис техосмотра не вернул решение.");

        var result = new AcceptanceResult(vehicle, inspection);
        if (result.IsAccepted)
        {
            _activeVehicles.Add(vehicle);
            _usedInventoryNumbers.Add(vehicle.InventoryNumber);
        }

        _inspectionLog.Add(result);
        return result;
    }

    /// <summary>Ставит вещь на баланс. Техосмотр для вещей не требуется.</summary>
    /// <exception cref="DuplicateInventoryNumberException">Номер уже занят.</exception>
    public void AddEquipment(Equipment item)
    {
        ArgumentNullException.ThrowIfNull(item);
        EnsureInventoryNumberIsFree(item.InventoryNumber);

        _equipment.Add(item);
        _usedInventoryNumbers.Add(item.InventoryNumber);
    }

    public bool IsInventoryNumberTaken(string inventoryNumber) =>
        !string.IsNullOrWhiteSpace(inventoryNumber) && _usedInventoryNumbers.Contains(inventoryNumber.Trim());

    private void EnsureInventoryNumberIsFree(string inventoryNumber)
    {
        if (_usedInventoryNumbers.Contains(inventoryNumber))
        {
            throw new DuplicateInventoryNumberException(inventoryNumber);
        }
    }
}
