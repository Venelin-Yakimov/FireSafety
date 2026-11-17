using FireSafety.Api.Infrastructure.Errors;
using FireSafety.Api.View.DTOs;
using FireSafety.Model.Data;
using FireSafety.Model.Entities;
using Microsoft.EntityFrameworkCore;

namespace FireSafety.Model.Services;

/// <summary>Бизнес правила за каталога на услугите по пожарна безопасност.</summary>
public sealed class FireServiceTypeService(AppDbContext db)
{
    public async Task<IReadOnlyList<FireServiceTypeDto>> GetActiveAsync() =>
        await db.FireServiceTypes
            .AsNoTracking()
            .Where(type => type.IsActive)
            .OrderBy(type => type.Name)
            .Select(type => new FireServiceTypeDto(type.FireServiceTypeId, type.Name))
            .ToListAsync();

    public async Task<FireServiceTypeDto> CreateAsync(FireServiceTypeUpsertDto dto)
    {
        var name = NormalizeName(dto.Name);
        await EnsureNameIsFreeAsync(name, exceptId: null);

        var serviceType = new FireServiceType { Name = name };
        db.FireServiceTypes.Add(serviceType);
        await db.SaveChangesAsync();

        return ToDto(serviceType);
    }

    public async Task UpdateAsync(int id, FireServiceTypeUpsertDto dto)
    {
        var serviceType = await FindActiveAsync(id);
        var name = NormalizeName(dto.Name);
        await EnsureNameIsFreeAsync(name, exceptId: id);

        serviceType.Name = name;
        await db.SaveChangesAsync();
    }

    public async Task DeactivateAsync(int id)
    {
        var serviceType = await FindActiveAsync(id);
        serviceType.IsActive = false;
        await db.SaveChangesAsync();
    }

    private async Task<FireServiceType> FindActiveAsync(int id) =>
        await db.FireServiceTypes.SingleOrDefaultAsync(type =>
            type.FireServiceTypeId == id && type.IsActive)
        ?? throw new AppException(StatusCodes.Status404NotFound, "Видът услуга не съществува.");

    private async Task EnsureNameIsFreeAsync(string name, int? exceptId)
    {
        // Името остава заето и след деактивиране, както изисква уникалният индекс в базата.
        if (await db.FireServiceTypes.AnyAsync(type =>
                type.Name == name && type.FireServiceTypeId != exceptId))
        {
            throw new AppException(StatusCodes.Status409Conflict,
                "Вече съществува вид услуга с това име.");
        }
    }

    private static string NormalizeName(string name)
    {
        var normalized = name.Trim();
        if (normalized.Length < 2)
        {
            throw new AppException(StatusCodes.Status400BadRequest,
                "Името трябва да съдържа поне 2 символа.");
        }

        return normalized;
    }

    private static FireServiceTypeDto ToDto(FireServiceType serviceType) =>
        new(serviceType.FireServiceTypeId, serviceType.Name);
}
