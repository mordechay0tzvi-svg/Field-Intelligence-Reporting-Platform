using Microsoft.AspNetCore.Mvc;
using Pipeline;
using Models;
namespace Controllers;

[ApiController]
[Route("api/[controller]")]
public class ReportsController : ControllerBase
{
    private readonly IElasticsearchPipline _pipeline;

    public ReportsController(IElasticsearchPipline pipeline)
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
        return Ok(await _pipeline.GetReportsAsync(theater,sector,location));
    }
    [HttpGet("reports/search")]
    public async Task<ActionResult<List<FieldReport>>> GetReportsByPriority([FromQuery] string[]? priorities = null,[FromQuery] DateTime? from = null,[FromQuery] DateTime? to = null)
    {
        return Ok(await _pipeline.GetReportsByPriorityAsync(priorities,from,to));
    }
    [HttpGet("search/reports")]
    public async Task<ActionResult<List<FieldReport>>> SearchCombinedAsync([FromQuery]string? text = null,[FromQuery]string? theater = null,[FromQuery]string? sector = null,[FromQuery]string? location = null,[FromQuery]string[]? priorities = null,[FromQuery]string? reportType = null,[FromQuery]DateTime? from = null,[FromQuery]DateTime? to = null)
    {
        return Ok(await _pipeline.SearchCombinedAsync(text, theater, sector, location, priorities, reportType, from, to));
    }
    [HttpGet("statistics")]
    public async Task<ActionResult<List<Dictionary<string, int>>>> GetStatsAsync()
    {
        return Ok(new List<Dictionary<string, int>>
        {
            await _pipeline.GetStatsForFieldAsync("priority"),
            await _pipeline.GetStatsForFieldAsync("reportType"),
            await _pipeline.GetStatsForFieldAsync("theater.keyword")
        });
    }
}