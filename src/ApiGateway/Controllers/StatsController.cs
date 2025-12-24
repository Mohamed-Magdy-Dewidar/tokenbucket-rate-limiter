using ApiGateway.Services;
using Microsoft.AspNetCore.Mvc;
namespace ApiGateway.Controllers;



[ApiController]
[Route("api/stats")]
public class StatsController : ControllerBase
{
    private readonly StatsService _stats;
    public StatsController(StatsService stats) => _stats = stats;

    [HttpGet]
    public IActionResult Get() => Ok(_stats.GetStats());
}