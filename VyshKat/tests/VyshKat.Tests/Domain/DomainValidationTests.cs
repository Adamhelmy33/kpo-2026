using VyshKat.Core.Domain;
using VyshKat.Core.Domain.Items;
using VyshKat.Core.Domain.Vehicles;

namespace VyshKat.Tests.Domain;

public sealed class DomainValidationTests
{
    [Theory]
    [InlineData(0)]
    [InlineData(-3)]
    [InlineData(11)]
    public void Vehicle_WithEaseOfUseOutsideOneToTen_IsRejected(int easeOfUse)
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => TestData.Scooter(easeOfUse: easeOfUse));
        Assert.Throws<ArgumentOutOfRangeException>(() => TestData.Bicycle(easeOfUse: easeOfUse));
    }

    [Theory]
    [InlineData(1)]
    [InlineData(10)]
    public void Vehicle_WithEaseOfUseOnScaleBoundary_IsCreated(int easeOfUse)
    {
        var scooter = TestData.Scooter(easeOfUse: easeOfUse);

        Assert.Equal(easeOfUse, scooter.EaseOfUse);
    }

    [Fact]
    public void EnergyConsumers_WithNegativeConsumption_AreRejected()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => TestData.Scooter(kwh: -0.1m));
        Assert.Throws<ArgumentOutOfRangeException>(() => TestData.EBike(kwh: -1m));
        Assert.Throws<ArgumentOutOfRangeException>(() => TestData.Station(kwh: -5m));
        Assert.Throws<ArgumentOutOfRangeException>(() => new ChargingCabinet("CC-1", "Шкаф", 4, -1m));
    }

    [Fact]
    public void EnergyConsumer_WithZeroConsumption_IsAllowed()
    {
        var station = TestData.Station(kwh: 0m);

        Assert.Equal(0m, station.DailyEnergyConsumptionKwh);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData("SC 001")]
    [InlineData("SC_001")]
    [InlineData("ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456")] // 33 символа
    public void InvalidInventoryNumber_IsRejected(string inventoryNumber)
    {
        Assert.Throws<ArgumentException>(() => TestData.Helmet(inventoryNumber));
    }

    [Fact]
    public void InventoryNumber_IsTrimmedAndUpperCased()
    {
        var helmet = TestData.Helmet("  hl-001 ");

        Assert.Equal("HL-001", helmet.InventoryNumber);
    }

    [Theory]
    [InlineData("")]
    [InlineData("  ")]
    public void BlankName_IsRejected(string name)
    {
        Assert.Throws<ArgumentException>(() => new Helmet("HL-1", name));
        Assert.Throws<ArgumentException>(() => new Bicycle("BK-1", name, 5));
    }

    [Fact]
    public void ChargingEquipment_WithoutPortsOrSlots_IsRejected()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => new ChargingStation("CS-1", "Станция", 0, 1m));
        Assert.Throws<ArgumentOutOfRangeException>(() => new ChargingCabinet("CC-1", "Шкаф", 0, 1m));
    }

    [Fact]
    public void OnlyRealConsumers_ImplementEnergyInterface()
    {
        // ISP: шлем и обычный велосипед не обязаны «потреблять энергию».
        Assert.False(typeof(IEnergyConsumer).IsAssignableFrom(typeof(Helmet)));
        Assert.False(typeof(IEnergyConsumer).IsAssignableFrom(typeof(Bicycle)));
        Assert.True(typeof(IEnergyConsumer).IsAssignableFrom(typeof(ElectricScooter)));
        Assert.True(typeof(IEnergyConsumer).IsAssignableFrom(typeof(ChargingStation)));
    }

    [Fact]
    public void Vehicle_WithoutExplicitCondition_IsConsideredWorking()
    {
        Assert.Equal(TechnicalCondition.Good, TestData.Bicycle().Condition);
    }
}
