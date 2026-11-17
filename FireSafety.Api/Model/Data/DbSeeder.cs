using FireSafety.Model.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace FireSafety.Model.Data;

/// <summary>Създава примерни акаунти само в Development и само при зададена Seed:DemoPassword.</summary>
public static class DbSeeder
{
    private const string Domain = "@firesafety.local";

    public static async Task SeedAsync(IServiceProvider services)
    {
        using var scope = services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var passwordHasher = scope.ServiceProvider.GetRequiredService<IPasswordHasher<User>>();
        var configuration = scope.ServiceProvider.GetRequiredService<IConfiguration>();
        var logger = scope.ServiceProvider.GetRequiredService<ILoggerFactory>().CreateLogger("DbSeeder");
        var demoPassword = configuration["Seed:DemoPassword"];

        if (string.IsNullOrWhiteSpace(demoPassword))
        {
            logger.LogWarning("Seed:DemoPassword липсва — примерните акаунти не са създадени.");
            return;
        }

        var demoUsers = new[]
        {
            (Email: "admin" + Domain, FullName: "Администратор", Role: UserRole.Admin),
            (Email: "client" + Domain, FullName: "Демо клиент", Role: UserRole.Client),
            (Email: "specialist" + Domain, FullName: "Демо специалист", Role: UserRole.FireSpecialist)
        };

        foreach (var demoUser in demoUsers)
        {
            if (await db.Users.AnyAsync(user => user.Email == demoUser.Email))
                continue;

            var user = new User
            {
                Email = demoUser.Email,
                FullName = demoUser.FullName,
                Role = demoUser.Role
            };
            user.PasswordHash = passwordHasher.HashPassword(user, demoPassword);
            db.Users.Add(user);
        }

        await db.SaveChangesAsync();

        var specialist = await db.Users.SingleAsync(user => user.Email == "specialist" + Domain);
        var firstActiveType = await db.FireServiceTypes
            .Where(type => type.IsActive)
            .OrderBy(type => type.FireServiceTypeId)
            .FirstOrDefaultAsync();

        if (firstActiveType is not null && !await db.SpecialistFireServiceTypes.AnyAsync(link =>
                link.SpecialistId == specialist.UserId && link.FireServiceTypeId == firstActiveType.FireServiceTypeId))
        {
            db.SpecialistFireServiceTypes.Add(new SpecialistFireServiceType
            {
                SpecialistId = specialist.UserId,
                FireServiceTypeId = firstActiveType.FireServiceTypeId
            });
            await db.SaveChangesAsync();
        }

        logger.LogInformation("Проверени са примерните акаунти за FireSafety.");
    }
}
