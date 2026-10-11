using VyshKat.Core.Domain.Vehicles;
using VyshKat.Core.Fleet;
using VyshKat.Tests.Fakes;

namespace VyshKat.Tests.Fleet;

public sealed class FleetServiceTests
{
    [Fact]
    public void SubmitVehicle_WhenInspectionApproves_AddsVehicleToActiveFleet()
    {
        var fleet = new FleetService(FakeInspectionService.AlwaysApprove());
        var scooter = TestData.Scooter("SC-1");

        var result = fleet.SubmitVehicle(scooter);

        Assert.True(result.IsAccepted);
        Assert.Same(scooter, result.Vehicle);
        Assert.Contains(scooter, fleet.ActiveVehicles);
        Assert.True(fleet.IsInventoryNumberTaken("SC-1"));
    }

    [Fact]
    public void SubmitVehicle_WhenInspectionRejects_DoesNotAddVehicleToActiveFleet()
    {
        var fleet = new FleetService(FakeInspectionService.AlwaysReject("неисправны тормоза"));
        var scooter = TestData.Scooter("SC-1");

        var result = fleet.SubmitVehicle(scooter);

        Assert.False(result.IsAccepted);
        Assert.Equal("неисправны тормоза", Assert.Single(result.Inspection.Remarks));
        Assert.Empty(fleet.ActiveVehicles);
        Assert.Empty(fleet.InventoryItems);
        Assert.False(fleet.IsInventoryNumberTaken("SC-1"));
    }

    [Fact]
    public void SubmitVehicle_AsksTheInjectedInspectionServiceExactlyOnce()
    {
        var fakeInspection = FakeInspectionService.AlwaysApprove();
        var fleet = new FleetService(fakeInspection);
        var bicycle = TestData.Bicycle();

        fleet.SubmitVehicle(bicycle);

        Assert.Same(bicycle, Assert.Single(fakeInspection.InspectedVehicles));
    }

    [Fact]
    public void SubmitVehicle_DecisionComesFromInjectedService_NotFromVehicleCondition()
    {
        // Исправное ТС, но фейк отказывает: решение принимает зависимость, а не FleetService.
        var fleet = new FleetService(FakeInspectionService.AlwaysReject());
        var perfectScooter = new ElectricScooter("SC-1", "Новый", 7, 0.5m, TechnicalCondition.Good);

        var result = fleet.SubmitVehicle(perfectScooter);

        Assert.False(result.IsAccepted);
    }

    [Fact]
    public void SubmitVehicle_RecordsBothAcceptedAndRejectedInInspectionLog()
    {
        var fleet = new FleetService(FakeInspectionService.ApproveOnly("SC-OK"));

        fleet.SubmitVehicle(TestData.Scooter("SC-OK"));
        fleet.SubmitVehicle(TestData.Scooter("SC-BAD"));

        Assert.Equal(2, fleet.InspectionLog.Count);
        Assert.True(fleet.InspectionLog[0].IsAccepted);
        Assert.False(fleet.InspectionLog[1].IsAccepted);
        Assert.Equal("SC-OK", Assert.Single(fleet.ActiveVehicles).InventoryNumber);
    }

    [Fact]
    public void SubmitVehicle_WithInventoryNumberOfActiveVehicle_ThrowsAndKeepsFleetUnchanged()
    {
        var fakeInspection = FakeInspectionService.AlwaysApprove();
        var fleet = new FleetService(fakeInspection);
        var original = TestData.Scooter("SC-1");
        fleet.SubmitVehicle(original);

        var ex = Assert.Throws<DuplicateInventoryNumberException>(
            () => fleet.SubmitVehicle(TestData.Bicycle("SC-1")));

        Assert.Equal("SC-1", ex.InventoryNumber);
        Assert.Same(original, Assert.Single(fleet.ActiveVehicles));
        Assert.Single(fakeInspection.InspectedVehicles); // дубликат даже не отправляли на техосмотр
    }

    [Fact]
    public void InventoryNumbers_AreComparedCaseInsensitively()
    {
        var fleet = new FleetService(FakeInspectionService.AlwaysApprove());
        fleet.SubmitVehicle(TestData.Scooter("sc-1"));

        Assert.Equal("SC-1", fleet.ActiveVehicles[0].InventoryNumber);
        Assert.Throws<DuplicateInventoryNumberException>(() => fleet.AddEquipment(TestData.Helmet("Sc-1")));
    }

    [Fact]
    public void InventoryNumbers_AreUniqueAcrossVehiclesAndEquipment()
    {
        var fleet = new FleetService(FakeInspectionService.AlwaysApprove());
        fleet.AddEquipment(TestData.Helmet("X-1"));

        Assert.Throws<DuplicateInventoryNumberException>(() => fleet.SubmitVehicle(TestData.Scooter("X-1")));
        Assert.Throws<DuplicateInventoryNumberException>(() => fleet.AddEquipment(TestData.Station("X-1")));
        Assert.Single(fleet.InventoryItems);
        Assert.Empty(fleet.InspectionLog);
    }

    [Fact]
    public void RejectedVehicle_DoesNotReserveInventoryNumber_AndCanBeResubmittedAfterRepair()
    {
        var fleet = new FleetService(new Core.Inspection.ServiceCenter());
        var broken = new ElectricScooter("SC-7", "Самокат", 7, 0.5m, new TechnicalCondition(false, true, true));
        var repaired = new ElectricScooter("SC-7", "Самокат", 7, 0.5m, TechnicalCondition.Good);

        var first = fleet.SubmitVehicle(broken);
        var second = fleet.SubmitVehicle(repaired);

        Assert.False(first.IsAccepted);
        Assert.True(second.IsAccepted);
        Assert.Same(repaired, Assert.Single(fleet.ActiveVehicles));
    }

    [Fact]
    public void AddEquipment_PutsItemOnBalanceWithoutInspection()
    {
        var fakeInspection = FakeInspectionService.AlwaysReject();
        var fleet = new FleetService(fakeInspection);
        var helmet = TestData.Helmet();

        fleet.AddEquipment(helmet);

        Assert.Contains(helmet, fleet.EquipmentItems);
        Assert.Contains(helmet, fleet.InventoryItems);
        Assert.Empty(fakeInspection.InspectedVehicles);
    }

    [Fact]
    public void NullArguments_AreRejected()
    {
        Assert.Throws<ArgumentNullException>(() => new FleetService(null!));

        var fleet = new FleetService(FakeInspectionService.AlwaysApprove());
        Assert.Throws<ArgumentNullException>(() => fleet.SubmitVehicle(null!));
        Assert.Throws<ArgumentNullException>(() => fleet.AddEquipment(null!));
    }

    [Fact]
    public void ActiveVehicles_CannotBeModifiedFromOutside()
    {
        var fleet = new FleetService(FakeInspectionService.AlwaysApprove());
        fleet.SubmitVehicle(TestData.Scooter());

        var list = Assert.IsAssignableFrom<IList<Vehicle>>(fleet.ActiveVehicles);

        Assert.True(list.IsReadOnly);
    }
}
