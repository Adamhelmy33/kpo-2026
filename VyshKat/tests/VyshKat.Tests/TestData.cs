using VyshKat.Core.Domain.Items;
using VyshKat.Core.Domain.Vehicles;

namespace VyshKat.Tests;

/// <summary>Короткие фабрики тестовых объектов, чтобы тесты читались по смыслу.</summary>
internal static class TestData
{
    public static ElectricScooter Scooter(string number = "SC-1", int easeOfUse = 7, decimal kwh = 0.5m) =>
        new(number, "Тестовый самокат", easeOfUse, kwh);

    public static Bicycle Bicycle(string number = "BK-1", int easeOfUse = 8) =>
        new(number, "Тестовый велосипед", easeOfUse);

    public static ElectricBicycle EBike(string number = "EB-1", int easeOfUse = 6, decimal kwh = 0.8m) =>
        new(number, "Тестовый электровелосипед", easeOfUse, kwh);

    public static Helmet Helmet(string number = "HL-1") => new(number, "Тестовый шлем");

    public static ChargingStation Station(string number = "CS-1", decimal kwh = 10m) =>
        new(number, "Тестовая станция", ports: 4, kwh);
}
