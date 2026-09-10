using Microsoft.AspNetCore.Mvc;
using Pipeline;
using Models;
using Elastic.Clients.Elasticsearch.IndexManagement;
using System.Threading.Tasks;
namespace Controllers;

[ApiController]
[Route("api/[controller]")]
public class SubjectsController : ControllerBase
{
    private readonly IElasticsearchPipline _pipeline;

    public SubjectsController(IElasticsearchPipline pipeline)
    {
        _pipeline = pipeline;
    }
    [HttpGet("{subjectId}/reports")]
    public async Task<ActionResult<FieldReport>> GetSubjectByIdAsync(string subjectId)
    {
        var report = await _pipeline.GetBySubjetIdAsync(subjectId);
        if(report == null)
        {
            return NotFound("report not found");
        }
        return Ok(report);
    }
}