using Microsoft.AspNetCore.Mvc;

namespace Chinook.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PingController : Controller
{
    [HttpGet]
    public IActionResult Get()
    {
        return Ok(new { message = "API is alive!", timestamp = DateTime.UtcNow });
    }
}
