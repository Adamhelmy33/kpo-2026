# VyshKat — Scooter-Sharing Fleet Management

> HSE Software Design, Homework #1: SOLID, DI container, unit tests.
> The console application's user interface is in Russian (as the assignment requires); the documentation is in English.

## 1. Purpose and functionality

A console application that manages the fleet of the "VyshKat" scooter-sharing service.

| Assignment requirement | Where it is implemented | Menu item |
|---|---|---|
| Add new vehicles to the fleet | `FleetService.SubmitVehicle` | 3 |
| Technical inspection before acceptance (approve / reject) | `IInspectionService` → `ServiceCenter` | 3, 7 |
| Count of units on the balance | `FleetReportService.GetSummary` | 1 |
| Total daily energy consumption | `FleetReportService.GetDailyEnergyReport` | 6 |
| Beginner-friendly vehicles (ease of use ≥ 6) | `EaseOfUseBeginnerRule` + `GetBeginnerFriendlyVehicles` | 5 |
| Track inventory items; show names and inventory numbers of vehicles and items | `FleetService.AddEquipment`, `GetInventoryList` | 4, 2 |
| Demonstrate every scenario | `DemoScenario` | 8 or `--demo` |

On start the fleet is seeded (`DemoDataSeeder`): two scooters, a bicycle, an e-bike, one faulty
scooter (rejected by the inspection), two helmets, a charging station and a charging cabinet.

## 2. Prerequisites

- **.NET SDK 10.0** (`global.json` requires 10.0.100 or newer in the 10.0.x line).
  Check with `dotnet --version`. If you only have .NET 8/9, either install .NET 10 or change
  `TargetFramework` in `Directory.Build.props` (and the `Microsoft.Extensions.*` package versions) and delete `global.json`.
- Internet access for the first `dotnet restore` (packages come from nuget.org).
- No database, no external API, no paid services.

Packages (all free):

| Project | Package | Version |
|---|---|---|
| VyshKat.Core | Microsoft.Extensions.DependencyInjection.Abstractions | 10.0.0 |
| VyshKat.Console | Microsoft.Extensions.DependencyInjection | 10.0.0 |
| VyshKat.Tests | xunit | 2.9.3 |
| VyshKat.Tests | xunit.runner.visualstudio | 3.1.4 |
| VyshKat.Tests | Microsoft.NET.Test.Sdk | 17.14.1 |
| VyshKat.Tests | coverlet.collector | 6.0.4 |
| VyshKat.Tests | Microsoft.Extensions.DependencyInjection | 10.0.0 |

## 3. Build, run, test, coverage

```bash
dotnet restore
dotnet build

# interactive menu
dotnet run --project src/VyshKat.Console

# automatic run of every scenario, then exit
dotnet run --project src/VyshKat.Console -- --demo

# tests
dotnet test

# coverage (Coverlet, Cobertura format)
dotnet test --collect:"XPlat Code Coverage"

# HTML report and text summary (free ReportGenerator tool)
dotnet tool install -g dotnet-reportgenerator-globaltool
reportgenerator -reports:"tests/**/TestResults/*/coverage.cobertura.xml" -targetdir:coveragereport -reporttypes:"Html;TextSummary"
cat coveragereport/Summary.txt          # Windows: type coveragereport\Summary.txt
```

Coverage is measured on `VyshKat.Core` (the only assembly the test project references). The console layer
has no business logic and is checked by running `--demo`.

**Verification results** (measured on 11 Oct 2026, Windows, .NET SDK 10.0.401, xUnit adapter v3.1.4):

| Check | Result |
|---|---|
| `dotnet restore` / `dotnet build` | succeeded, all 3 projects (Core, Console, Tests) |
| `dotnet test` | **62 tests: 62 passed, 0 failed, 0 skipped** |
| Line coverage of `VyshKat.Core` | **97.7 %** (255 of 261 lines) |
| Branch coverage | 96.1 % (50 of 52) |
| Method coverage | 93.1 % (68 of 73) |
| `dotnet run --project src/VyshKat.Console -- --demo` | ran to completion; the output matches the sample at the end of this file |

The coverage requirement of the assignment (at least 60 %) is met. Coverage was measured with Coverlet
(`coverlet.collector`) and summarised with ReportGenerator. Only `VyshKat.Core` is measured; the console project
contains no business logic and is checked by the `--demo` run.

## 4. Project structure

```
VyshKat.sln
Directory.Build.props        shared settings: net10.0, nullable, implicit usings
global.json                  SDK version pin
src/
  VyshKat.Core/              domain model and business logic, no console code
    Domain/
      IInventoryItem.cs      contract "tracked by inventory number"
      IEnergyConsumer.cs     contract "consumes energy"
      InventoryAsset.cs      base class: validates inventory number and name
      Vehicles/              Vehicle, ElectricVehicle, ElectricScooter, ElectricBicycle, Bicycle, TechnicalCondition
      Items/                 Equipment, Helmet, ChargingStation, ChargingCabinet
    Inspection/              IInspectionService, ServiceCenter, InspectionResult
    Fleet/                   FleetService, AcceptanceResult, DuplicateInventoryNumberException
    Rules/                   IBeginnerSuitabilityRule, EaseOfUseBeginnerRule
    Reports/                 FleetReportService, FleetSummary, EnergyReport
    ServiceCollectionExtensions.cs   AddVyshKatCore() — DI registration
  VyshKat.Console/           input/output
    Program.cs               composition root (DI)
    ConsoleApplication.cs    menu and dialogs
    ConsoleUi.cs             reading numbers/strings with retry, coloured output
    ReportPrinter.cs         prints reports
    Catalogs.cs              which vehicle/item kinds can be entered from the keyboard
    DemoDataSeeder.cs        start-up data
    DemoScenario.cs          automatic demonstration of all scenarios
    UserMessage.cs           cleans exception text for users
tests/
  VyshKat.Tests/             xUnit, hand-written fake inspection service
docs/DEFENSE_GUIDE.md        oral-defense study guide
```

Two projects under `src` ensure the core physically cannot depend on the console, and the tests
reference only the core. More projects would be unnecessary.

### Responsibilities of the main classes

- **`FleetService`** — accepts vehicles and puts equipment on the balance. Guards two invariants:
  only vehicles approved by the inspection join the active fleet; inventory numbers are unique.
- **`ServiceCenter`** — the real inspection: brakes, frame, lights. Returns an `InspectionResult`.
- **`FleetReportService`** — read-only: summary, beginner-friendly list, energy, inventory list.
- **`EaseOfUseBeginnerRule`** — the single place where the threshold "≥ 6" is defined.
- **`ConsoleApplication` / `ReportPrinter` / `ConsoleUi`** — user dialog, no calculations.

## 5. Domain model

```
IInventoryItem ◄─── InventoryAsset (abstract) ──┬── Vehicle (abstract): EaseOfUse 1–10, Condition
                                                │     ├── Bicycle
                                                │     └── ElectricVehicle (abstract) ── IEnergyConsumer
                                                │           ├── ElectricScooter
                                                │           └── ElectricBicycle
                                                └── Equipment (abstract)
                                                      ├── Helmet
                                                      ├── ChargingStation ── IEnergyConsumer
                                                      └── ChargingCabinet ── IEnergyConsumer
```

- **`IInventoryItem`** (`InventoryNumber`, `Name`, `Kind`) — everything on the balance. The
  "names and inventory numbers" report works only with this interface.
- **`IEnergyConsumer`** (`DailyEnergyConsumptionKwh`) — only things that really consume energy.
  A helmet and an ordinary bicycle do **not** implement it.
- **`InventoryAsset`** — shared implementation of `IInventoryItem`: the number is trimmed and upper-cased
  (`sc-001` equals `SC-001`); letters, digits and hyphen only; at most 32 characters.
- **`Vehicle`** — rentable transport; ease of use outside 1–10 throws `ArgumentOutOfRangeException`.
- **`ElectricVehicle`** — shared check "consumption ≥ 0" for electric transport.
- **`Equipment`** — items put on the balance without inspection and not rented out.
- **`TechnicalCondition`** — the data the inspection examines.

Design decisions:

- **Ease of use exists only on vehicles.** The assignment needs the rule for rentable equipment that can be given to
  beginners; nobody rents out a charging cabinet. If items ever need a score, the property moves into its own interface.
- **Employees are not modelled.** `FleetService` methods accept only `Vehicle` and `Equipment`, so an
  employee cannot be put on the balance even by accident — it is a compile error.
- **`decimal` for kWh** — sums like 0.45 + 0.8 are exact, with no `double` rounding artefacts.
- **The energy report covers every energy consumer on the balance**: active vehicles **and** items
  (stations, cabinets), with separate subtotals. The assignment asks for the fleet's energy consumption, and
  charging stations are as much part of the fleet as scooters. A vehicle rejected by the inspection is not on the balance
  and is not counted. A device with zero consumption appears in the list but does not change the total.

### Accepting a vehicle

```
SubmitVehicle(vehicle)
  ├─ number already taken? ── yes ──► DuplicateInventoryNumberException (inspection is not called)
  ├─ IInspectionService.Inspect(vehicle)
  ├─ approved ── yes ──► added to ActiveVehicles, number reserved
  │            └ no ───► does not join the fleet, number stays free (can be resubmitted after repair)
  └─ entry in InspectionLog, returns AcceptanceResult(Vehicle, Inspection)
```

Why two mechanisms: **an inspection rejection is a normal business outcome**, so it is returned as a result.
**A duplicate number or an invalid value is bad input**, so it is an exception. The console catches these and
shows a friendly message; the program does not crash.

## 6. The DI container

The standard `Microsoft.Extensions.DependencyInjection` container is used. Everything is wired in two places:

- `VyshKat.Core/ServiceCollectionExtensions.cs` — `AddVyshKatCore()` registers the core:
  `IInspectionService → ServiceCenter`, `IBeginnerSuitabilityRule → EaseOfUseBeginnerRule`,
  `FleetService`, `FleetReportService`.
- `VyshKat.Console/Program.cs` — the composition root: calls `AddVyshKatCore()`, adds the console classes,
  builds the container with `ValidateOnBuild = true` and resolves a single entry point, `ConsoleApplication`.
  Nothing else calls `GetService`; dependencies arrive through constructors (no service locator).

Every service is a `Singleton`: the fleet lives in memory for the whole run, and reports must see the same
`FleetService` instance that the acceptance workflow uses.

Replacing the inspection in a test (`DependencyInjectionTests`):

```csharp
var services = new ServiceCollection().AddVyshKatCore();          // same registration as Program.cs
services.Replace(ServiceDescriptor.Singleton<IInspectionService>(fake));
var fleet = services.BuildServiceProvider().GetRequiredService<FleetService>();
```

`FleetService` does not know which inspection it received, and its code does not change.

## 7. Test strategy

xUnit with no mocking library: the hand-written `FakeInspectionService` is clearer and offers
`AlwaysApprove()`, `AlwaysReject(reason)`, `ApproveOnly(numbers...)`, and it records which vehicles it was
asked about. Tests assert observable behaviour (what ended up in the fleet, what the report totals are), not internal fields.

| Requirement | Test |
|---|---|
| Approved vehicle is accepted | `FleetServiceTests.SubmitVehicle_WhenInspectionApproves_AddsVehicleToActiveFleet` |
| Rejected vehicle is not in the fleet | `FleetServiceTests.SubmitVehicle_WhenInspectionRejects_DoesNotAddVehicleToActiveFleet` |
| The substituted dependency is used | `FleetServiceTests.SubmitVehicle_AsksTheInjectedInspectionServiceExactlyOnce`, `..._DecisionComesFromInjectedService_NotFromVehicleCondition`, `DependencyInjectionTests.Container_WithReplacedInspection_InjectsFakeIntoFleetService` |
| Energy total | `FleetReportServiceTests.DailyEnergy_SumsAllEnergyConsumersOnBalance` |
| Non-consuming items do not affect it | `FleetReportServiceTests.DailyEnergy_IgnoresHelmetsAndOrdinaryBicycles` |
| Ease of use 5 / 6 / 7 | `EaseOfUseBeginnerRuleTests.IsSuitableForBeginners_UsesThresholdSixInclusive` (Theory) |
| Invalid ease of use | `DomainValidationTests.Vehicle_WithEaseOfUseOutsideOneToTen_IsRejected` |
| Duplicate numbers | `FleetServiceTests.SubmitVehicle_WithInventoryNumberOfActiveVehicle_...`, `InventoryNumbers_AreUniqueAcrossVehiclesAndEquipment`, `InventoryNumbers_AreComparedCaseInsensitively` |
| New vehicle type without changing the core | `NewVehicleTypeTests` (the `Monowheel` class exists only in the tests) |
| Rejected vehicles absent from reports | `FleetReportServiceTests.RejectedVehicles_DoNotAppearInAnyActiveFleetReport` |

Also covered: zero consumption, an empty fleet, resubmission after repair, a rejection without a reason, inventory-number
format and length, blank names, `null` arguments, summary by kind.

## 8. SOLID in the code

### S — Single Responsibility
- **Where:** `FleetService` (acceptance and bookkeeping), `ServiceCenter` (inspection), `FleetReportService`
  (reports), `EaseOfUseBeginnerRule` (rule), `ConsoleApplication`/`ReportPrinter` (I/O).
- **Problem solved:** changing a report format does not touch acceptance; new inspection criteria do not
  touch bookkeeping; the console can be replaced without touching the core.
- **Extension:** CSV export of reports is a new class next to `ReportPrinter`.
- **Compromise:** `FleetService` also stores the data in memory. A separate repository for three lists
  would be an unnecessary abstraction; when a database appears, storage moves into one.

### O — Open/Closed
- **Where:** `FleetService`, `ServiceCenter` and `FleetReportService` work with `Vehicle`, `IInventoryItem`,
  `IEnergyConsumer` and never check for a concrete class such as `ElectricScooter`.
- **Problem solved:** a new vehicle type is a new class; the core is not edited. Proof: `NewVehicleTypeTests`
  with `Monowheel`, which the core has never heard of.
- **Extension:** the beginner rule changes through a new `IBeginnerSuitabilityRule` implementation and one registration line.
- **Limitations:** to enter a new kind **from the keyboard**, a line must be added to `Console/Catalogs.cs` — the UI
  has to know what to offer. The set of inspection checks (`TechnicalCondition`) is fixed: a new criterion
  requires editing `ServiceCenter`.

### L — Liskov Substitution
- **Where:** any `Vehicle` subclass passes through `SubmitVehicle`, the rule and the reports identically; all
  subclasses share the same invariants (ease 1–10, consumption ≥ 0), enforced in the base classes.
- **Problem solved:** no "special" types that break generic code. A bicycle does **not** implement `IEnergyConsumer`
  with zero or throw `NotSupportedException` — it simply is not a consumer. Otherwise code expecting a real
  consumption figure would get false data or crash.
- **Extension:** `Monowheel` in the tests inherits `ElectricVehicle` and works everywhere at once.
- **Limitation:** the compiler cannot prove LSP; it is maintained by tests and by `sealed` concrete classes.

### I — Interface Segregation
- **Where:** `IInventoryItem` and `IEnergyConsumer` are two small independent contracts. `Helmet` implements
  only the first, `ChargingStation` both.
- **Problem solved:** a helmet has no meaningless "energy consumption" property; the energy report takes exactly
  the types that implement `IEnergyConsumer`.
- **Extension:** "needs periodic maintenance" would become its own interface for only those who need it.
- **Limitation:** `Kind` lives in `IInventoryItem` although it is for display only. A separate interface for one string would be excessive.

### D — Dependency Inversion
- **Where:** `FleetService(IInspectionService inspection)` — the high-level service depends on the inspection
  abstraction; the container chooses the implementation (`ServiceCenter` or a fake).
  `FleetReportService` depends on `IBeginnerSuitabilityRule`.
- **Problem solved:** acceptance can be tested without a real inspection; the real service centre can be
  replaced (for example by an external API) without touching `FleetService`.
- **Limitation:** see section 10.

## 9. Which principle is demonstrated most clearly

**DIP** (together with DI). `Fleet/FleetService.cs` receives `IInspectionService` through its constructor,
`ServiceCollectionExtensions.cs` binds it to `ServiceCenter`, and `DependencyInjectionTests` replaces it with a fake
via `services.Replace(...)` — a perfectly sound scooter is then rejected because the fake said so. It is visible both
in the code and in the behaviour.

## 10. Which principle is intentionally limited, and why

**DIP is limited for `FleetService` itself.** `FleetReportService`, `ConsoleApplication` and `ReportPrinter` depend on the
**concrete** `FleetService`; there is no `IFleetService`/`IFleetRepository`. Reasons: there is one implementation, and
nothing needs to replace it, neither in the program nor in tests (tests use the real `FleetService` with a fake
inspection). An interface "just in case" is exactly the decorative SOLID that costs points. When a second
storage appears (a database), a repository will be extracted from `FleetService` and the interface will be justified.

A related SRP compromise: `FleetService` both stores data and performs acceptance (see section 8).

A borderline case in the other direction: `IBeginnerSuitabilityRule` has one implementation today. It stays because the
assignment explicitly allows the rule to become more complex, so it is a planned extension point, not decoration. If you
disagree, it can be collapsed into a plain class without changing any logic.

## 11. What happens when the model is extended

- **A new vehicle type** (say, an electric moped): a class `: ElectricVehicle` (or `: Vehicle`) with `Kind` and a
  constructor, plus a line in `Catalogs.VehicleKinds`. `FleetService`, `ServiceCenter`, the reports and the rule do not
  change. **Nothing breaks** — `NewVehicleTypeTests` proves it.
- **An employee:** a separate class outside `Domain` (for example `Staff/Employee`) **without** `IInventoryItem`, with its
  own service. It never reaches the asset inventory, because `FleetService` methods accept only `Vehicle`/`Equipment`.
  Nothing breaks. If an employee is assigned to an inspection, they are passed to `ServiceCenter` as data, not as an asset.
- **A spare-parts warehouse:** if parts are tracked one by one with a number, `SparePart : Equipment` is enough and they
  show up in the inventory list with no other change. If parts are tracked in batches with quantities, a separate
  `WarehouseService` is needed, and the **inventory-number uniqueness check** (currently inside `FleetService`) must
  become shared — extracted into a registry of numbers used by both services. That is the only place that would need editing.
- **A new beginner rule** (for example, no electric vehicle above 1 kWh/day for beginners): a new
  `IBeginnerSuitabilityRule` implementation and one line in `AddVyshKatCore`.

## 12. Which tests verify the inspection and the beginner rule

- **Inspection:** `FleetServiceTests.SubmitVehicle_WhenInspectionRejects_DoesNotAddVehicleToActiveFleet` and
  `SubmitVehicle_WhenInspectionApproves_...`. The dependency is replaced through the constructor:
  `new FleetService(FakeInspectionService.AlwaysReject(...))`. Through the container:
  `DependencyInjectionTests.Container_WithReplacedInspection_InjectsFakeIntoFleetService` with
  `services.Replace(ServiceDescriptor.Singleton<IInspectionService>(fake))`. The real `ServiceCenter` is tested
  separately in `ServiceCenterTests`.
- **Beginner rule:** `EaseOfUseBeginnerRuleTests.IsSuitableForBeginners_UsesThresholdSixInclusive`
  (values 1, 5, 6, 7, 10) and `FleetReportServiceTests.BeginnerFriendly_...`.

## 13. AI usage


- **Generated by AI (Claude):** the architecture, the source code of all three projects, the unit tests, this README
  . The AI compiled the core and the console app and ran `--demo` in its own environment.
  It could not download xUnit there, so it ran the test sources through its own simplified stand-in (62 of 62 passed);
  it never ran the real `dotnet test` or measured coverage.
- **Tested by me on my own machine:** I installed .NET SDK 10.0.401 and ran `dotnet restore`, `dotnet build`,
  `dotnet test` (62 of 62 passed with the real xUnit) and the coverage commands (97.7 % line coverage of `VyshKat.Core`).
  I ran the application with `--demo` and the output matched the expected scenarios.

## Sample output (`--demo`, excerpt; the application's own output is in Russian)

```
=== Загрузка стартовых данных (транспорт проходит техосмотр) ===
✔ Электросамокат «ВышКат Lite» [SC-001] — техосмотр пройден, принят в активный парк.
✘ Электросамокат «ВышКат Lite (б/у)» [SC-003] — отклонён техосмотром: неисправны тормоза.

Шаг 2. Электровелосипед с повреждённой рамой и без света → отклонён, в парк не попадает.
✘ Электровелосипед «ВышКат E-Bike (после ДТП)» [D1-EB] — отклонён техосмотром: повреждена рама; не работает световое оборудование.
   В активном парке: нет; номер D1-EB занят: нет.
Шаг 4. Шлем ставится на баланс без техосмотра и не влияет на энергопотребление.
✔ Шлем «Шлем детский, размер S» [D1-HL] поставлен на баланс. Энергия до: 20,85 кВт·ч, после: 20,85 кВт·ч.
Шаг 5. Повторное использование номера SC-001 (в любом регистре) → отказ.
Ошибка: Инвентарный номер SC-001 уже занят другим объектом на балансе.

=== Транспорт для новичков (правило: простота ≥ 6) ===
BK-001       Велосипед          Городской велосипед «Покровка» — простота 9/10
D1-SC        Электросамокат     ВышКат Lite (демо) — простота 8/10
SC-001       Электросамокат     ВышКат Lite — простота 7/10
EB-001       Электровелосипед   ВышКат E-Bike — простота 6/10

=== Суточное энергопотребление ===
Транспорт: 2,35 кВт·ч
Вещи:      18,50 кВт·ч
ИТОГО:     20,85 кВт·ч в сутки
```

