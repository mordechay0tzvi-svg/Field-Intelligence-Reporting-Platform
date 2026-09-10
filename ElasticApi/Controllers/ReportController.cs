using Microsoft.AspNetCore.Mvc;
using Pipeline;
using Models;
namespace Controllers;

[ApiController]
[Route("api/[controller]")]
public class ElasticsearchController : ControllerBase
{
    private readonly IElasticsearchPipline _pipeline;

    public ElasticsearchController(IElasticsearchPipline pipeline)
    {
        _pipeline = pipeline;
    }
    [HttpGet("search")]
    public async Task<ActionResult<IEnumerable<FieldReport>>> SearchTerm([FromQuery] string query)
    {
        return await _pipeline.SearchAsync(query);
    }
     [HttpGet("reports")]
    public async Task<ActionResult<List<FieldReport>>> GetReports([FromQuery] string? theater = null,[FromQuery] string? sector = null,[FromQuery] string? location = null)
    {
        var reports = await _pipeline.GetReportsAsync(theater,sector,location);
        return Ok(reports);
    }
    [HttpGet("priority")]
    public async Task<ActionResult<List<FieldReport>>> GetReportsByPriority([FromQuery] string[]? priorities = null,[FromQuery] DateTime? from = null,[FromQuery] DateTime? to = null)
    {
        var reports = await _pipeline.GetReportsByPriorityAsync(priorities,from,to);
        return Ok(reports);
    }
}