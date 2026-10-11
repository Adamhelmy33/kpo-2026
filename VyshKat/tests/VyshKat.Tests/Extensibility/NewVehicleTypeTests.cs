using VyshKat.Core.Domain.Vehicles;
using VyshKat.Core.Fleet;
using VyshKat.Core.Inspection;
using VyshKat.Core.Reports;
using VyshKat.Core.Rules;

namespace VyshKat.Tests.Extensibility;

/// <summary>
/// Новый вид транспорта, объявленный ТОЛЬКО в тестовом проекте.
/// Если с ним работают приёмка, техосмотр и отчёты, значит ядро не пришлось менять (OCP)
/// и наследник корректно подставляется вместо Vehicle (LSP).
/// </summary>
internal sealed class Monowheel : ElectricVehicle
{
    public Monowheel(string inventoryNumber, int easeOfUse, decimal kwh, TechnicalCondition? condition = null)
        : base(inventoryNumber, "Моноколесо", easeOfUse, kwh, condition)
    {
    }

    public override string Kind => "Моноколесо";
}

public sealed class NewVehicleTypeTests
{
    [Fact]
    public void NewVehicleType_IsAcceptedInspectedAndReported_WithoutChangingCore()
    {
        var fleet = new FleetService(new ServiceCenter());
        var reports = new FleetReportService(fleet, new EaseOfUseBeginnerRule());

        var accepted = fleet.SubmitVehicle(new Monowheel("MW-1", easeOfUse: 2, kwh: 0.3m));
        var rejected = fleet.SubmitVehicle(new Monowheel("MW-2", easeOfUse: 2, kwh: 0.3m,
            new TechnicalCondition(BrakesWorking: true, FrameIntact: false, LightsWorking: true)));

        Assert.True(accepted.IsAccepted);
        Assert.False(rejected.IsAccepted);
        Assert.Equal(0.3m, reports.GetDailyEnergyReport().TotalKwh);
        Assert.Empty(reports.GetBeginnerFriendlyVehicles()); // простота 2 < 6
        Assert.Contains(new KindCount("Моноколесо", 1), reports.GetSummary().ByKind);
        Assert.Equal("MW-1", Assert.Single(reports.GetInventoryList()).InventoryNumber);
    }

    [Fact]
    public void NewVehicleType_InheritsCommonValidation()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => new Monowheel("MW-1", easeOfUse: 0, kwh: 0.3m));
        Assert.Throws<ArgumentOutOfRangeException>(() => new Monowheel("MW-1", easeOfUse: 5, kwh: -0.3m));
        Assert.Throws<ArgumentException>(() => new Monowheel("", easeOfUse: 5, kwh: 0.3m));
    }
}
