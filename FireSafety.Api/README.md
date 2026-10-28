# Пожарна безопасност — бекенд

ASP.NET Core Web API за учебния проект „Пожарна безопасност“. Системата е предназначена за клиенти, специалисти по пожарна безопасност и администратор.

**Технологии:** ASP.NET Core Web API (.NET 10) · Entity Framework Core · SQL Server LocalDB · OpenAPI / Swagger UI

## Какво има в тази папка

| Път | Съдържание |
|---|---|
| `Model/Entities/` | Модели за потребители, услуги, заявки, оферти и отзиви; статуси и модели за SQL изгледите |
| `Model/Data/AppDbContext.cs` | EF Core конфигурация на таблиците и начални видове услуги |
| `Model/Data/Migrations/` | Миграции за създаване на схемата и SQL обектите |
| `Model/Data/Sql/SqlObjects.cs` | SQL дефиниции на процедурите, изгледите и табличния тип |
| `Database/02_objects.sql` | SQL скрипт за обектите от стъпка 8 |
| `Program.cs` | Регистрация на базата и настройка на API и Swagger UI |

## Стартиране

Нужно е .NET 10 SDK и SQL Server LocalDB (включен например с Visual Studio).

1. Отворете `FireSafety.sln` в Visual Studio или терминал в главната папка на хранилището.
2. Проверете `appsettings.json` за връзката `ConnectionStrings:Default`. Текущата настройка използва LocalDB и базата `FireSafety`.
3. Създайте или актуализирайте базата от Package Manager Console:

   ```powershell
   Update-Database -Project FireSafety.Api -StartupProject FireSafety.Api
   ```

   Или с .NET CLI:

   ```powershell
   dotnet ef database update --project FireSafety.Api --startup-project FireSafety.Api
   ```

4. Стартирайте проекта `FireSafety.Api` с профил `https`. В Development Swagger UI е достъпен на `/swagger`.

## База данни

Миграцията `InitialCreate` създава шестте таблици от заданието: `Users`, `FireServiceTypes`, `SpecialistFireServiceTypes`, `SafetyRequests`, `Offers` и `Reviews`. Миграцията `SqlObjects` добавя `dbo.IdListType`, процедурите `usp_AcceptOffer`, `usp_AddReview`, `usp_SetFireSpecialistServiceTypes` и изгледите `vw_OpenSafetyRequests`, `vw_FireSpecialistProfiles`.

Изгледите не връщат телефон или точен адрес. EF Core поддържа допълнителната системна таблица `__EFMigrationsHistory` за приложените миграции.

## Състояние

Проектът е подготвен до стъпка 8: модел на данните, миграции и SQL обекти. Контролери, вход/регистрация и останалите API функции са следващ етап от разработката.
