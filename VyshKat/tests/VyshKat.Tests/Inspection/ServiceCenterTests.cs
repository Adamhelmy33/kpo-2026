using VyshKat.Core.Domain.Vehicles;
using VyshKat.Core.Inspection;

namespace VyshKat.Tests.Inspection;

public sealed class ServiceCenterTests
{
    private readonly ServiceCenter _serviceCenter = new();

    [Fact]
    public void Inspect_FullyWorkingVehicle_IsApproved()
    {
        var result = _serviceCenter.Inspect(TestData.Scooter());

        Assert.True(result.IsApproved);
        Assert.Empty(result.Remarks);
    }

    [Fact]
    public void Inspect_BrokenBrakes_IsRejectedWithReason()
    {
        var bike = new Bicycle("BK-1", "Велосипед", 8, new TechnicalCondition(false, true, true));

        var result = _serviceCenter.Inspect(bike);

        Assert.False(result.IsApproved);
        Assert.Equal("неисправны тормоза", Assert.Single(result.Remarks));
    }

    [Fact]
    public void Inspect_SeveralDefects_ListsEveryReason()
    {
        var ebike = new ElectricBicycle("EB-1", "Электровелосипед", 6, 0.8m, new TechnicalCondition(false, false, false));

        var result = _serviceCenter.Inspect(ebike);

        Assert.False(result.IsApproved);
        Assert.Equal(3, result.Remarks.Count);
        Assert.Contains("повреждена рама", result.Remarks);
        Assert.Contains("не работает световое оборудование", result.Remarks);
    }

    [Fact]
    public void Inspect_Null_IsRejected()
    {
        Assert.Throws<ArgumentNullException>(() => _serviceCenter.Inspect(null!));
    }
}
