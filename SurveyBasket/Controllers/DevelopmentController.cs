using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SurveyBasket.Services;

namespace SurveyBasket.Controllers;

[Route("api/[controller]")]
[ApiController]
public class DevelopmentController : ControllerBase
{
    private readonly IOS _osService;

    public DevelopmentController(IOS osService)
    {
        _osService = osService;
    }

    [HttpGet]
    public IActionResult Get()
    {
        var message = _osService.appRun();
        return Ok(message);
    }
}
