using System.Globalization;
using System.Text;
using Microsoft.Extensions.DependencyInjection;
using VyshKat.ConsoleApp;
using VyshKat.Core;

// ---- Корень композиции: единственное место, где собирается граф объектов. ----

ConfigureConsole();

var services = new ServiceCollection();
services.AddVyshKatCore();                 // доменные сервисы (FleetService, ServiceCenter, отчёты)
services.AddSingleton<ConsoleUi>();        // ввод/вывод
services.AddSingleton<ReportPrinter>();    // форматирование отчётов
services.AddSingleton<DemoDataSeeder>();   // стартовые данные
services.AddSingleton<DemoScenario>();     // автоматический прогон всех сценариев
services.AddSingleton<ConsoleApplication>();

using var provider = services.BuildServiceProvider(new ServiceProviderOptions
{
    ValidateOnBuild = true,   // ошибка регистрации обнаружится сразу при старте
    ValidateScopes = true,
});

var app = provider.GetRequiredService<ConsoleApplication>();
var demoOnly = args.Contains("--demo", StringComparer.OrdinalIgnoreCase);
app.Run(demoOnly);

static void ConfigureConsole()
{
    Console.OutputEncoding = Encoding.UTF8;
    try
    {
        Console.InputEncoding = Encoding.UTF8;
    }
    catch (IOException)
    {
        // Некоторые терминалы не позволяют сменить кодировку ввода — это не критично.
    }

    var culture = CultureInfo.GetCultureInfo("ru-RU");
    CultureInfo.DefaultThreadCurrentCulture = culture;
    CultureInfo.DefaultThreadCurrentUICulture = culture;
    CultureInfo.CurrentCulture = culture;
    CultureInfo.CurrentUICulture = culture;
}
