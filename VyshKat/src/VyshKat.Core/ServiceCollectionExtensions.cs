using Microsoft.Extensions.DependencyInjection;
using VyshKat.Core.Fleet;
using VyshKat.Core.Inspection;
using VyshKat.Core.Reports;
using VyshKat.Core.Rules;

namespace VyshKat.Core;

/// <summary>
/// Регистрация сервисов ядра в DI-контейнере. Вызывается из корня композиции
/// (Program.cs) и из тестов, где затем подменяется IInspectionService.
/// </summary>
public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddVyshKatCore(this IServiceCollection services)
    {
        ArgumentNullException.ThrowIfNull(services);

        // Реальный техосмотр. В тестах заменяется фейком через Replace(...).
        services.AddSingleton<IInspectionService, ServiceCenter>();

        // Правило «для новичков» с порогом по умолчанию (≥ 6).
        services.AddSingleton<IBeginnerSuitabilityRule>(_ => new EaseOfUseBeginnerRule());

        // Singleton: парк хранится в памяти всё время работы приложения.
        services.AddSingleton<FleetService>();
        services.AddSingleton<FleetReportService>();

        return services;
    }
}
