using DotnetDemo.Domain.DTOs;
using DotnetDemo.Domain.Requests;
using DotnetDemo.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DotnetDemo.Controllers;

[ApiController]
[Authorize]
[Route("authors")]
public class AuthorsController : ControllerBase
{
    private readonly IAuthorService _authorService;

    public AuthorsController(IAuthorService authorService)
    {
        _authorService = authorService;
    }

    [HttpGet]
    [AllowAnonymous]
    public async Task<ActionResult<IEnumerable<AuthorDto>>> GetAll(CancellationToken cancellationToken)
    {
        var result = await _authorService.GetAllAsync(cancellationToken);
        return Ok(result);
    }

    [HttpGet("{id:guid}")]
    [AllowAnonymous]
    public async Task<ActionResult<AuthorDto>> GetById(Guid id, CancellationToken cancellationToken)
    {
        var author = await _authorService.GetByIdAsync(id, cancellationToken);
        if (author is null)
        {
            return NotFound(new { message = "Author not found" });
        }

        return Ok(author);
    }

    [HttpPost]
    public async Task<ActionResult<AuthorDto>> Create([FromBody] AuthorRequest request, CancellationToken cancellationToken)
    {
        var author = await _authorService.CreateAsync(request, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id = author.Id }, author);
    }

    [HttpPatch("{id:guid}")]
    public async Task<ActionResult<AuthorDto>> Update(Guid id, [FromBody] AuthorRequest request, CancellationToken cancellationToken)
    {
        var author = await _authorService.UpdateAsync(id, request, cancellationToken);
        if (author is null)
        {
            return NotFound(new { message = "Author not found" });
        }

        return Ok(author);
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        var deleted = await _authorService.DeleteAsync(id, cancellationToken);
        if (!deleted)
        {
            return NotFound(new { message = "Author not found" });
        }

        return NoContent();
    }
}

