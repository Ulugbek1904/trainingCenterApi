using Microsoft.AspNetCore.Mvc;
using trainingCenter.Domain.Models;
using trainingCenter.Infrastructure.brokers.storage;

namespace trainingCenterApi.Presentation.Controllers;

[ApiController]
[Route("[controller]")]
public class Home : ControllerBase
{
    [HttpGet]
    public IActionResult Get()
    {
        return Ok("Welcome to the Training Center API!");
    }

}