using Microsoft.AspNetCore.Mvc;

namespace TalentShowCase.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class HomeController : ControllerBase
{
    [HttpGet]
    public IActionResult Get()
    {
        return Ok(new { message = "Welcome to Talent Show Case API" });
    }

    [HttpGet("health")]
    public IActionResult HealthCheck()
    {
        return Ok(new { status = "Healthy", timestamp = DateTime.UtcNow });
    }
} 