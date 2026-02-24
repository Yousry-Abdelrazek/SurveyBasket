using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace SurveyBasket.Controllers;

[Route("[controller]")]
[ApiController]
public class AuthController(IAuthService authService, IOptions<JwtOptions> jwtOptions) : ControllerBase
{
    private readonly IAuthService _authService = authService;
    private readonly JwtOptions _jwtOptions = jwtOptions.Value;

    [HttpPost]
    public async Task<IActionResult> LoginAsync(LoginRequest loginRequest , CancellationToken cancellationToken = default)
    {
        var authResult = await _authService.GetTokenAsync(loginRequest.Email, loginRequest.Password, cancellationToken);

        return authResult is null ? BadRequest("Invalid Cradentials") : Ok(authResult);
    }


    [HttpGet("Test")]
    public IActionResult Test()
    {
        return Ok(_jwtOptions.Audience);
    }
}
