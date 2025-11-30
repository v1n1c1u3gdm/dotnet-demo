using DotnetDemo.Domain.DTOs;
using DotnetDemo.Domain.Requests;
using DotnetDemo.Services;
using Microsoft.AspNetCore.Mvc;

namespace DotnetDemo.Controllers;

[ApiController]
[Route("articles")]
public class ArticlesController : ControllerBase
{
    private readonly IArticleService _articleService;

    public ArticlesController(IArticleService articleService)
    {
        _articleService = articleService;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<ArticleDto>>> GetAll(CancellationToken cancellationToken)
    {
        var result = await _articleService.GetAllAsync(cancellationToken);
        return Ok(result);
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<ArticleDto>> GetById(Guid id, CancellationToken cancellationToken)
    {
        var article = await _articleService.GetByIdAsync(id, cancellationToken);
        if (article is null)
        {
            return NotFound(new { message = "Article not found" });
        }

        return Ok(article);
    }

    [HttpPost]
    public async Task<ActionResult<ArticleDto>> Create([FromBody] ArticleRequest request, CancellationToken cancellationToken)
    {
        var article = await _articleService.CreateAsync(request, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id = article.Id }, article);
    }

    [HttpPatch("{id:guid}")]
    public async Task<ActionResult<ArticleDto>> Update(Guid id, [FromBody] ArticleRequest request, CancellationToken cancellationToken)
    {
        var article = await _articleService.UpdateAsync(id, request, cancellationToken);
        if (article is null)
        {
            return NotFound(new { message = "Article not found" });
        }

        return Ok(article);
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        var deleted = await _articleService.DeleteAsync(id, cancellationToken);
        if (!deleted)
        {
            return NotFound(new { message = "Article not found" });
        }

        return NoContent();
    }

    [HttpGet("count_by_author")]
    public async Task<ActionResult<IEnumerable<ArticlesCountDto>>> CountByAuthor(CancellationToken cancellationToken)
    {
        var result = await _articleService.CountByAuthorAsync(cancellationToken);
        return Ok(result);
    }
}

