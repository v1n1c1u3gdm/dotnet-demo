using DotnetDemo.Domain.DTOs;
using DotnetDemo.Domain.Requests;
using DotnetDemo.Services;
using Microsoft.AspNetCore.Mvc;

namespace DotnetDemo.Controllers;

[ApiController]
[Route("socials")]
public class SocialsController : ControllerBase
{
    private readonly ISocialService _socialService;

    public SocialsController(ISocialService socialService)
    {
        _socialService = socialService;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<SocialDto>>> GetAll(CancellationToken cancellationToken)
    {
        var socials = await _socialService.GetAllAsync(cancellationToken);
        return Ok(socials);
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<SocialDto>> GetById(Guid id, CancellationToken cancellationToken)
    {
        var social = await _socialService.GetByIdAsync(id, cancellationToken);
        if (social is null)
        {
            return NotFound(new { message = "Social not found" });
        }

        return Ok(social);
    }

    [HttpPost]
    public async Task<ActionResult<SocialDto>> Create([FromBody] SocialRequest request, CancellationToken cancellationToken)
    {
        var social = await _socialService.CreateAsync(request, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id = social.Id }, social);
    }

    [HttpPatch("{id:guid}")]
    public async Task<ActionResult<SocialDto>> Update(Guid id, [FromBody] SocialRequest request, CancellationToken cancellationToken)
    {
        var social = await _socialService.UpdateAsync(id, request, cancellationToken);
        if (social is null)
        {
            return NotFound(new { message = "Social not found" });
        }

        return Ok(social);
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        var deleted = await _socialService.DeleteAsync(id, cancellationToken);
        if (!deleted)
        {
            return NotFound(new { message = "Social not found" });
        }

        return NoContent();
    }
}

