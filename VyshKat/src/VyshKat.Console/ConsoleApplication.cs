using VyshKat.Core.Domain.Vehicles;
using VyshKat.Core.Fleet;

namespace VyshKat.ConsoleApp;

/// <summary>
/// Точка входа приложения, получаемая из DI-контейнера.
/// Отвечает только за меню и диалог с пользователем; решения принимает ядро.
/// </summary>
public sealed class ConsoleApplication
{
    private readonly FleetService _fleet;
    private readonly ReportPrinter _printer;
    private readonly DemoDataSeeder _seeder;
    private readonly DemoScenario _demo;
    private readonly ConsoleUi _ui;

    public ConsoleApplication(
        FleetService fleet,
        ReportPrinter printer,
        DemoDataSeeder seeder,
        DemoScenario demo,
        ConsoleUi ui)
    {
        _fleet = fleet;
        _printer = printer;
        _seeder = seeder;
        _demo = demo;
        _ui = ui;
    }

    public void Run(bool demoOnly)
    {
        _ui.Info("ВышКат — учёт парка кикшеринга. «Доедем до дедлайна»");
        _ui.Header("Загрузка стартовых данных (транспорт проходит техосмотр)");
        foreach (var result in _seeder.Seed())
        {
            _printer.PrintAcceptance(result);
        }

        if (demoOnly)
        {
            _demo.Run();
            return;
        }

        try
        {
            MenuLoop();
        }
        catch (ConsoleUi.InputClosedException)
        {
            _ui.Info(string.Empty);
            _ui.Info("Ввод завершён. До свидания!");
        }
    }

    private void MenuLoop()
    {
        while (true)
        {
            _ui.Header("Главное меню");
            _ui.Info("1. Сводка парка (число единиц)");
            _ui.Info("2. Наименования и инвентарные номера всей техники и вещей");
            _ui.Info("3. Принять транспорт в парк (с техосмотром)");
            _ui.Info("4. Поставить вещь на баланс");
            _ui.Info("5. Транспорт для новичков");
            _ui.Info("6. Суммарное суточное энергопотребление");
            _ui.Info("7. Журнал техосмотров");
            _ui.Info("8. Автоматическая демонстрация всех сценариев");
            _ui.Info("0. Выход");

            var choice = _ui.ReadInt("Выберите пункт", 0, 8);
            switch (choice)
            {
                case 0:
                    _ui.Info("До свидания!");
                    return;
                case 1:
                    _printer.PrintSummary();
                    break;
                case 2:
                    _printer.PrintInventory();
                    break;
                case 3:
                    AcceptVehicle();
                    break;
                case 4:
                    AddEquipment();
                    break;
                case 5:
                    _printer.PrintBeginnerFriendly();
                    break;
                case 6:
                    _printer.PrintEnergy();
                    break;
                case 7:
                    _printer.PrintInspectionLog();
                    break;
                case 8:
                    _demo.Run();
                    break;
            }

            _ui.Pause();
        }
    }

    private void AcceptVehicle()
    {
        _ui.Header("Приёмка транспорта");
        var kind = Choose(Catalogs.VehicleKinds.Select(k => k.Title).ToList());
        var option = Catalogs.VehicleKinds[kind];

        var inventoryNumber = ReadFreeInventoryNumber();
        if (inventoryNumber is null)
        {
            return;
        }

        var name = _ui.ReadText("Наименование: ");
        var easeOfUse = _ui.ReadAnyInt($"Простота для новичка ({Vehicle.MinEaseOfUse}–{Vehicle.MaxEaseOfUse}): ");
        var energy = option.ConsumesEnergy ? _ui.ReadDecimal("Суточный расход энергии, кВт·ч: ") : 0m;

        _ui.Info("Данные для техосмотра:");
        var condition = new TechnicalCondition(
            BrakesWorking: _ui.ReadYesNo("  Тормоза исправны?"),
            FrameIntact: _ui.ReadYesNo("  Рама цела?"),
            LightsWorking: _ui.ReadYesNo("  Фары/световозвращатели работают?"));

        TryDomainAction(() =>
        {
            var vehicle = option.Create(new VehicleDraft(inventoryNumber, name, easeOfUse, energy, condition));
            _printer.PrintAcceptance(_fleet.SubmitVehicle(vehicle));
        });
    }

    private void AddEquipment()
    {
        _ui.Header("Постановка вещи на баланс");
        var kind = Choose(Catalogs.EquipmentKinds.Select(k => k.Title).ToList());
        var option = Catalogs.EquipmentKinds[kind];

        var inventoryNumber = ReadFreeInventoryNumber();
        if (inventoryNumber is null)
        {
            return;
        }

        var name = _ui.ReadText("Наименование: ");
        var capacity = option.CapacityPrompt is null ? 0 : _ui.ReadAnyInt(option.CapacityPrompt);
        var energy = option.ConsumesEnergy ? _ui.ReadDecimal("Суточный расход энергии, кВт·ч: ") : 0m;

        TryDomainAction(() =>
        {
            var item = option.Create(new EquipmentDraft(inventoryNumber, name, capacity, energy));
            _fleet.AddEquipment(item);
            _ui.Success($"{item} поставлен на баланс.");
        });
    }

    /// <summary>Ранняя проверка номера, чтобы не заставлять вводить остальные поля зря.</summary>
    private string? ReadFreeInventoryNumber()
    {
        var number = _ui.ReadText("Инвентарный номер (буквы, цифры, дефис): ");
        if (_fleet.IsInventoryNumberTaken(number))
        {
            _ui.Error($"инвентарный номер {number.ToUpperInvariant()} уже занят.");
            return null;
        }

        return number;
    }

    private int Choose(IReadOnlyList<string> titles)
    {
        for (var i = 0; i < titles.Count; i++)
        {
            _ui.Info($"{i + 1}. {titles[i]}");
        }

        return _ui.ReadInt("Вид", 1, titles.Count) - 1;
    }

    /// <summary>Ошибки валидации доменной модели показываем пользователю, а не роняем программу.</summary>
    private void TryDomainAction(Action action)
    {
        try
        {
            action();
        }
        catch (ArgumentException ex)
        {
            _ui.Error(UserMessage.From(ex));
        }
        catch (DuplicateInventoryNumberException ex)
        {
            _ui.Error(ex.Message);
        }
    }
}
