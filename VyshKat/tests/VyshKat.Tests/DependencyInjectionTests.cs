using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using VyshKat.Core;
using VyshKat.Core.Domain.Vehicles;
using VyshKat.Core.Fleet;
using VyshKat.Core.Inspection;
using VyshKat.Core.Reports;
using VyshKat.Tests.Fakes;

namespace VyshKat.Tests;

/// <summary>
/// Проверяем ту же регистрацию, что использует Program.cs (AddVyshKatCore),
/// и подмену техосмотра фейком прямо в DI-контейнере.
/// </summary>
public sealed class DependencyInjectionTests
{
    [Fact]
    public void Container_ByDefault_UsesRealServiceCenter()
    {
        using var provider = new ServiceCollection().AddVyshKatCore().BuildServiceProvider(validateScopes: true);

        Assert.IsType<ServiceCenter>(provider.GetRequiredService<IInspectionService>());
    }

    [Fact]
    public void Container_WithReplacedInspection_InjectsFakeIntoFleetService()
    {
        var fake = FakeInspectionService.AlwaysReject("отказ из фейка");
        var services = new ServiceCollection().AddVyshKatCore();
        services.Replace(ServiceDescriptor.Singleton<IInspectionService>(fake));
        using var provider = services.BuildServiceProvider(validateScopes: true);

        var fleet = provider.GetRequiredService<FleetService>();
        var result = fleet.SubmitVehicle(new ElectricScooter("SC-1", "Исправный", 8, 0.5m));

        Assert.False(result.IsAccepted);                      // исправный самокат отклонён — решал фейк
        Assert.Equal("отказ из фейка", Assert.Single(result.Inspection.Remarks));
        Assert.Single(fake.InspectedVehicles);
        Assert.Empty(provider.GetRequiredService<FleetReportService>().GetInventoryList());
    }

    [Fact]
    public void Container_SharesOneFleetBetweenServiceAndReports()
    {
        var services = new ServiceCollection().AddVyshKatCore();
        services.Replace(ServiceDescriptor.Singleton<IInspectionService>(FakeInspectionService.AlwaysApprove()));
        using var provider = services.BuildServiceProvider(validateScopes: true);

        provider.GetRequiredService<FleetService>().SubmitVehicle(TestData.Scooter("SC-1"));

        var inventory = provider.GetRequiredService<FleetReportService>().GetInventoryList();
        Assert.Equal("SC-1", Assert.Single(inventory).InventoryNumber);
    }
}
