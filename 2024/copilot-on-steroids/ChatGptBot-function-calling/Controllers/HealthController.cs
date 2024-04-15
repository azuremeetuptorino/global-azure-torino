using Microsoft.AspNetCore.Mvc;

namespace ChatGptBot.Controllers;

[ApiController]
[Route("health")]
public class HealthController : ControllerBase
{
    [HttpGet]
    public string Get()
    {
        return "ok";
    }
}