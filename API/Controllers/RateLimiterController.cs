using Microsoft.AspNetCore.Mvc;
using RateLimiter.Application;

namespace API.Controllers;

/// <summary>
/// RateLimiter Controller
/// </summary>
[Route("api/")]
[ApiController]
public class RateLimiterController : ControllerBase
{
    private readonly RateLimiterService _rateLimiterService;

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="rateLimiterService"></param>
    public RateLimiterController(RateLimiterService rateLimiterService)
    {
        _rateLimiterService = rateLimiterService;
    }

    /// <summary>
    /// POST api/request
    /// </summary>
    /// <param name="accessToken"></param>
    /// <returns></returns>
    [HttpPost("request")]
    public IActionResult CheckRules(string accessToken)
    {
        var requestTime = DateTime.UtcNow;

        // Check if the request is allowed based on the rate-limiting logic
        bool isAllowed = _rateLimiterService.IsRequestAllowed(accessToken, requestTime);

        if (isAllowed)
        {
            return Ok("Request allowed");
        }
        else
        {
            return StatusCode(429, "Rate limit exceeded");
        }
    }
}

