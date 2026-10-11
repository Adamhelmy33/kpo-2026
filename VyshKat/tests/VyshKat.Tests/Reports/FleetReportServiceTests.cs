using VyshKat.Core.Fleet;
using VyshKat.Core.Inspection;
using VyshKat.Core.Reports;
using VyshKat.Core.Rules;
using VyshKat.Tests.Fakes;

namespace VyshKat.Tests.Reports;

public sealed class FleetReportServiceTests
{
    private static (FleetService Fleet, FleetReportService Reports) Create(IInspectionService inspection)
    {
        var fleet = new FleetService(inspection);
        return (fleet, new FleetReportService(fleet, new EaseOfUseBeginnerRule()));
    }

    [Fact]
    public void DailyEnergy_SumsAllEnergyConsumersOnBalance()
    {
        var (fleet, reports) = Create(FakeInspectionService.AlwaysApprove());
        fleet.SubmitVehicle(TestData.Scooter("SC-1", kwh: 0.45m));
        fleet.SubmitVehicle(TestData.EBike("EB-1", kwh: 0.80m));
        fleet.AddEquipment(TestData.Station("CS-1", kwh: 12m));

        var report = reports.GetDailyEnergyReport();

        Assert.Equal(1.25m, report.VehiclesTotalKwh);
        Assert.Equal(12m, report.EquipmentTotalKwh);
        Assert.Equal(13.25m, report.TotalKwh);
        Assert.Equal(3, report.Entries.Count);
    }

    [Fact]
    public void DailyEnergy_IgnoresHelmetsAndOrdinaryBicycles()
    {
        var (fleet, reports) = Create(FakeInspectionService.AlwaysApprove());
        fleet.SubmitVehicle(TestData.Scooter("SC-1", kwh: 0.5m));
        var before = reports.GetDailyEnergyReport().TotalKwh;

        fleet.SubmitVehicle(TestData.Bicycle("BK-1"));
        fleet.AddEquipment(TestData.Helmet("HL-1"));
        var after = reports.GetDailyEnergyReport();

        Assert.Equal(before, after.TotalKwh);
        Assert.DoesNotContain(after.Entries, e => e.InventoryNumber is "BK-1" or "HL-1");
    }

    [Fact]
    public void DailyEnergy_ZeroConsumptionDevice_IsListedButAddsNothing()
    {
        var (fleet, reports) = Create(FakeInspectionService.AlwaysApprove());
        fleet.AddEquipment(TestData.Station("CS-0", kwh: 0m));

        var report = reports.GetDailyEnergyReport();

        Assert.Equal("CS-0", Assert.Single(report.Entries).InventoryNumber);
        Assert.Equal(0m, report.TotalKwh);
    }

    [Fact]
    public void DailyEnergy_EmptyFleet_IsZero()
    {
        var (_, reports) = Create(FakeInspectionService.AlwaysApprove());

        var report = reports.GetDailyEnergyReport();

        Assert.Empty(report.Entries);
        Assert.Equal(0m, report.TotalKwh);
    }

    [Fact]
    public void RejectedVehicles_DoNotAppearInAnyActiveFleetReport()
    {
        var (fleet, reports) = Create(FakeInspectionService.ApproveOnly("SC-OK"));
        fleet.SubmitVehicle(TestData.Scooter("SC-OK", easeOfUse: 7, kwh: 0.5m));
        fleet.SubmitVehicle(TestData.Scooter("SC-BAD", easeOfUse: 9, kwh: 100m));

        Assert.Equal(0.5m, reports.GetDailyEnergyReport().TotalKwh);
        Assert.Equal("SC-OK", Assert.Single(reports.GetBeginnerFriendlyVehicles()).InventoryNumber);
        Assert.Equal("SC-OK", Assert.Single(reports.GetInventoryList()).InventoryNumber);

        var summary = reports.GetSummary();
        Assert.Equal(1, summary.ActiveVehicles);
        Assert.Equal(1, summary.RejectedSubmissions);
    }

    [Fact]
    public void BeginnerFriendly_ContainsOnlyVehiclesPassingRule_SortedByEaseDescending()
    {
        var (fleet, reports) = Create(FakeInspectionService.AlwaysApprove());
        fleet.SubmitVehicle(TestData.Scooter("SC-5", easeOfUse: 5));
        fleet.SubmitVehicle(TestData.Scooter("SC-6", easeOfUse: 6));
        fleet.SubmitVehicle(TestData.Bicycle("BK-9", easeOfUse: 9));

        var numbers = reports.GetBeginnerFriendlyVehicles().Select(v => v.InventoryNumber);

        Assert.Equal(new[] { "BK-9", "SC-6" }, numbers);
    }

    [Fact]
    public void BeginnerFriendly_UsesInjectedRule()
    {
        var fleet = new FleetService(FakeInspectionService.AlwaysApprove());
        var reports = new FleetReportService(fleet, new EaseOfUseBeginnerRule(minimumEaseOfUse: 9));
        fleet.SubmitVehicle(TestData.Scooter("SC-8", easeOfUse: 8));

        Assert.Empty(reports.GetBeginnerFriendlyVehicles());
    }

    [Fact]
    public void InventoryList_ShowsVehiclesFirstThenEquipment_WithNumbersAndNames()
    {
        var (fleet, reports) = Create(FakeInspectionService.AlwaysApprove());
        fleet.AddEquipment(TestData.Helmet("HL-1"));
        fleet.SubmitVehicle(TestData.Scooter("SC-2"));
        fleet.SubmitVehicle(TestData.Bicycle("BK-1"));

        var list = reports.GetInventoryList();

        Assert.Equal(new[] { "BK-1", "SC-2", "HL-1" }, list.Select(i => i.InventoryNumber));
        Assert.All(list, item => Assert.False(string.IsNullOrWhiteSpace(item.Name)));
    }

    [Fact]
    public void Summary_CountsUnitsByKind()
    {
        var (fleet, reports) = Create(FakeInspectionService.AlwaysApprove());
        fleet.SubmitVehicle(TestData.Scooter("SC-1"));
        fleet.SubmitVehicle(TestData.Scooter("SC-2"));
        fleet.SubmitVehicle(TestData.Bicycle("BK-1"));
        fleet.AddEquipment(TestData.Helmet("HL-1"));

        var summary = reports.GetSummary();

        Assert.Equal(3, summary.ActiveVehicles);
        Assert.Equal(1, summary.EquipmentItems);
        Assert.Equal(4, summary.TotalOnBalance);
        Assert.Equal(new KindCount("Электросамокат", 2), summary.ByKind[0]);
        Assert.Contains(new KindCount("Шлем", 1), summary.ByKind);
    }

    [Fact]
    public void Constructor_RejectsNullDependencies()
    {
        var fleet = new FleetService(FakeInspectionService.AlwaysApprove());

        Assert.Throws<ArgumentNullException>(() => new FleetReportService(null!, new EaseOfUseBeginnerRule()));
        Assert.Throws<ArgumentNullException>(() => new FleetReportService(fleet, null!));
    }
}
