using FireSafety.Model.Entities;
using Microsoft.EntityFrameworkCore;
using System.Reflection.Emit;

namespace FireSafety.Model.Data;

public class AppDbContext(DbContextOptions<AppDbContext> options)
    : DbContext(options)
{
    public DbSet<User> Users => Set<User>();
    public DbSet<FireServiceType> FireServiceTypes => Set<FireServiceType>();
    public DbSet<SpecialistFireServiceType> SpecialistFireServiceTypes
        => Set<SpecialistFireServiceType>();
    public DbSet<SafetyRequest> SafetyRequests => Set<SafetyRequest>();
    public DbSet<Offer> Offers => Set<Offer>();
    public DbSet<Review> Reviews => Set<Review>();
    public DbSet<OpenSafetyRequestView> OpenSafetyRequests => Set<OpenSafetyRequestView>();
    public DbSet<FireSpecialistProfileView> FireSpecialistProfiles => Set<FireSpecialistProfileView>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        ConfigureUsers(modelBuilder);
        ConfigureFireServiceTypes(modelBuilder);
        SeedFireServiceTypes(modelBuilder);
        ConfigureSpecialistFireServiceTypes(modelBuilder);
        ConfigureSafetyRequests(modelBuilder);
        ConfigureOffers(modelBuilder);
        ConfigureReviews(modelBuilder);
        ConfigureViews(modelBuilder);
    }

    private static void ConfigureViews(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<OpenSafetyRequestView>(entity =>
        {
            entity.HasNoKey();
            entity.ToView("vw_OpenSafetyRequests");
        });

        modelBuilder.Entity<FireSpecialistProfileView>(entity =>
        {
            entity.HasNoKey();
            entity.ToView("vw_FireSpecialistProfiles");
            entity.Property(x => x.AverageRating).HasColumnType("decimal(38,6)");
        });
    }

    private static void ConfigureUsers(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>(e =>
        {
            e.ToTable("Users", table =>
            {
                table.HasCheckConstraint(
                    "CK_Users_Role",
                    "[Role] IN (N'Client', N'FireSpecialist', N'Admin')");
            });

            e.HasKey(u => u.UserId);

            e.Property(u => u.Email)
                .IsRequired()
                .HasMaxLength(256);

            e.HasIndex(u => u.Email).IsUnique();

            e.Property(u => u.PasswordHash)
                .IsRequired()
                .HasMaxLength(500);

            e.Property(u => u.FullName)
                .IsRequired()
                .HasMaxLength(120);

            e.Property(u => u.Phone)
                .HasMaxLength(30);

            e.Property(u => u.Role)
                .HasConversion<string>()
                .HasMaxLength(30);

            e.Property(u => u.CreatedAt)
                .HasDefaultValueSql("SYSUTCDATETIME()");
        });
    }

    private static void ConfigureFireServiceTypes(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<FireServiceType>(e =>
        {
            e.ToTable("FireServiceTypes");

            e.HasKey(t => t.FireServiceTypeId);

            e.Property(t => t.Name)
                .IsRequired()
                .HasMaxLength(100);

            e.HasIndex(t => t.Name).IsUnique();

            e.Property(t => t.IsActive)
                .HasDefaultValue(true);
        });
    }

    private static void ConfigureSpecialistFireServiceTypes(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<SpecialistFireServiceType>(e =>
        {
            e.ToTable("SpecialistFireServiceTypes");

            e.HasKey(x => new { x.SpecialistId, x.FireServiceTypeId });

            e.HasOne(x => x.Specialist)
                .WithMany(u => u.SpecialistFireServiceTypes)
                .HasForeignKey(x => x.SpecialistId)
                .OnDelete(DeleteBehavior.Restrict);

            e.HasOne(x => x.FireServiceType)
                .WithMany(t => t.SpecialistFireServiceTypes)
                .HasForeignKey(x => x.FireServiceTypeId)
                .OnDelete(DeleteBehavior.Restrict);
        });
    }

    private static void SeedFireServiceTypes(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<FireServiceType>().HasData(
            new FireServiceType { FireServiceTypeId = 1, Name = "Проверка и презареждане на пожарогасители" },
            new FireServiceType { FireServiceTypeId = 2, Name = "Монтаж на пожарогасители" },
            new FireServiceType { FireServiceTypeId = 3, Name = "Пожароизвестителни системи" },
            new FireServiceType { FireServiceTypeId = 4, Name = "Пожарогасителни системи" },
            new FireServiceType { FireServiceTypeId = 5, Name = "Пожарна безопасност и оценка на риска" },
            new FireServiceType { FireServiceTypeId = 6, Name = "Инструктаж и евакуационни планове" });
    }

    private static void ConfigureSafetyRequests(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<SafetyRequest>(e =>
        {
            e.ToTable("SafetyRequests", table =>
            {
                table.HasCheckConstraint(
                    "CK_SafetyRequests_Status",
                    "[Status] IN (N'Open', N'Assigned', N'Completed', N'Cancelled')");
            });

            e.HasKey(r => r.SafetyRequestId);

            e.Property(r => r.Title)
                .IsRequired()
                .HasMaxLength(150);

            e.Property(r => r.Description)
                .IsRequired()
                .HasMaxLength(3000);

            e.Property(r => r.ExactAddress)
                .IsRequired()
                .HasMaxLength(300);

            e.Property(r => r.Status)
                .HasConversion<string>()
                .HasMaxLength(20)
                .HasDefaultValue(SafetyRequestStatus.Open);

            e.Property(r => r.CreatedAt)
                .HasDefaultValueSql("SYSUTCDATETIME()");

            e.HasIndex(r => new { r.FireServiceTypeId, r.Status });

            e.HasOne(r => r.Client)
                .WithMany(u => u.SafetyRequests)
                .HasForeignKey(r => r.ClientId)
                .OnDelete(DeleteBehavior.Restrict);

            e.HasOne(r => r.FireServiceType)
                .WithMany(t => t.SafetyRequests)
                .HasForeignKey(r => r.FireServiceTypeId)
                .OnDelete(DeleteBehavior.Restrict);
        });
    }

    private static void ConfigureOffers(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Offer>(e =>
        {
            e.ToTable("Offers", table =>
            {
                table.HasCheckConstraint("CK_Offers_Price", "[Price] > 0");

                table.HasCheckConstraint(
                    "CK_Offers_Status",
                    "[Status] IN (N'Pending', N'Accepted', N'Rejected')");
            });

            e.HasKey(o => o.OfferId);

            e.Property(o => o.Price)
                .HasPrecision(10, 2);

            e.Property(o => o.Message)
                .HasMaxLength(1000);

            e.Property(o => o.Status)
                .HasConversion<string>()
                .HasMaxLength(20)
                .HasDefaultValue(OfferStatus.Pending);

            e.Property(o => o.CreatedAt)
                .HasDefaultValueSql("SYSUTCDATETIME()");

            e.HasIndex(o => new { o.SafetyRequestId, o.FireSpecialistId })
                .IsUnique();

            e.HasIndex(o => o.SafetyRequestId)
                .HasDatabaseName("UX_Offers_OneAccepted")
                .IsUnique()
                .HasFilter("[Status] = N'Accepted'");

            e.HasOne(o => o.SafetyRequest)
                .WithMany(r => r.Offers)
                .HasForeignKey(o => o.SafetyRequestId)
                .OnDelete(DeleteBehavior.Restrict);

            e.HasOne(o => o.FireSpecialist)
                .WithMany(u => u.Offers)
                .HasForeignKey(o => o.FireSpecialistId)
                .OnDelete(DeleteBehavior.Restrict);
        });
    }

    private static void ConfigureReviews(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Review>(e =>
        {
            e.ToTable("Reviews", table =>
            {
                table.HasCheckConstraint(
                    "CK_Reviews_Rating",
                    "[Rating] BETWEEN 1 AND 5");
            });

            e.HasKey(r => r.ReviewId);

            e.Property(r => r.Comment)
                .HasMaxLength(1000);

            e.Property(r => r.CreatedAt)
                .HasDefaultValueSql("SYSUTCDATETIME()");

            e.HasOne(r => r.SafetyRequest)
                .WithOne(s => s.Review)
                .HasForeignKey<Review>(r => r.SafetyRequestId)
                .OnDelete(DeleteBehavior.Restrict);
        });
    }
}
