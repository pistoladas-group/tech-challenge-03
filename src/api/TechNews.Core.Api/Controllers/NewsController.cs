using System.Net;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TechNews.Common.Library.Models;
using TechNews.Core.Api.Data;

namespace TechNews.Core.Api.Controllers;

[ApiController]
[Route("api/news")]
public class NewsController : ControllerBase
{
    public ApplicationDbContext _context { get; set; }
    public NewsController(ApplicationDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Get all news
    /// </summary>
    /// <response code="200">Returns the resource data</response>
    /// <response code="500">There was an internal problem</response>
    [Authorize]
    [HttpGet("")]
    [Produces("application/json")]
    [ProducesResponseType(typeof(ApiResponse), (int)HttpStatusCode.OK)]
    [ProducesResponseType(typeof(ApiResponse), (int)HttpStatusCode.InternalServerError)]
    public async Task<IActionResult> GetAllNewsAsync()
    {
        var news = await _context.News.Include(x => x.Author).AsNoTracking().ToListAsync();
        return Ok(new ApiResponse(data: news));
    }

    /// <summary>
    /// Get news by id
    /// </summary>
    /// <response code="200">Returns the resource data</response>
    /// <response code="400">There is a problem with the request</response>
    /// <response code="404">The resource was not found</response>
    /// <response code="500">There was an internal problem</response>
    [Authorize]
    [HttpGet("{newsId:guid}")]
    [Produces("application/json")]
    [ProducesResponseType(typeof(ApiResponse), (int)HttpStatusCode.OK)]
    [ProducesResponseType(typeof(ApiResponse), (int)HttpStatusCode.BadRequest)]
    [ProducesResponseType(typeof(ApiResponse), (int)HttpStatusCode.NotFound)]
    [ProducesResponseType(typeof(ApiResponse), (int)HttpStatusCode.InternalServerError)]
    public async Task<IActionResult> GetNewsById([FromRoute] Guid newsId)
    {
        if (newsId == Guid.Empty)
        {
            return BadRequest(new ApiResponse());
        }

        var news = await _context.News.Include(x => x.Author).AsNoTracking().FirstOrDefaultAsync(x => x.Id == newsId);

        if (news is null)
        {
            return NotFound(new ApiResponse());
        }

        return Ok(new ApiResponse(data: news));
    }
}
