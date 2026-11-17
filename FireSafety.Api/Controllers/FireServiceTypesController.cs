using FireSafety.Api.Infrastructure.Auth;
using FireSafety.Api.View.DTOs;
using FireSafety.Model.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FireSafety.Api.Controllers;

/// <summary>Всеки вижда активните услуги; само Admin може да ги управлява.</summary>
[ApiController]
[Route("api/v1/fire-service-types")]
[Authorize(Roles = AppRoles.Admin)]
public sealed class FireServiceTypesController(FireServiceTypeService service) : ControllerBase
{
    [HttpGet]
    [AllowAnonymous]
    [ProducesResponseType<IReadOnlyList<FireServiceTypeDto>>(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll() => Ok(await service.GetActiveAsync());

    [HttpPost]
    [ProducesResponseType<FireServiceTypeDto>(StatusCodes.Status201Created)]
    public async Task<IActionResult> Create(FireServiceTypeUpsertDto dto)
    {
        var serviceType = await service.CreateAsync(dto);
        return StatusCode(StatusCodes.Status201Created, serviceType);
    }

    [HttpPut("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> Update(int id, FireServiceTypeUpsertDto dto)
    {
        await service.UpdateAsync(id, dto);
        return NoContent();
    }

    [HttpDelete("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> Deactivate(int id)
    {
        await service.DeactivateAsync(id);
        return NoContent();
    }
}
