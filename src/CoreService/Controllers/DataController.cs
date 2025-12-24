using Microsoft.AspNetCore.Mvc;

namespace CoreService.Controllers;

[ApiController]
[Route("api/content")]
public class DataController : ControllerBase
{
    private readonly HttpClient _httpClient;

    public DataController(IHttpClientFactory httpClientFactory)
    {
        _httpClient = httpClientFactory.CreateClient();
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetContent(int id)
    {
        // Simulate 50ms processing time (CPU work)
        await Task.Delay(50);
        return Ok(new
        {
            Source = "Simulation Mode (Safe)",
            Id = id,
            Title = $"Generated Diploma #{id}",
            Status = "Verified"
        });        
    }
}
