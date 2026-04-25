using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace SurveyBasket.Controllers;

[Route("[controller]")]
[ApiController]
public class AuthController(IAuthService authService, ILogger<AuthController> logger) : ControllerBase
{
    private readonly IAuthService _authService = authService;
    private readonly ILogger<AuthController> _logger = logger;

    [HttpPost("")]
    public async Task<IActionResult> LoginAsync([FromBody] LoginRequest loginRequest , CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Login attempt for email: {Email} and password: {password}", loginRequest.Email,loginRequest.Password);


        var authResult = await _authService.GetTokenAsync(loginRequest.Email, loginRequest.Password, cancellationToken);
        //throw new Exception("Test exception for global handler");

        return authResult.IsSuccess
            ? Ok(authResult.Value)
            : authResult.ToProblem();

    }

    [HttpPost("refresh")]
    public async Task<IActionResult> RefreshAsync([FromBody] RefreshTokenRequest request ,  CancellationToken cancellationToken = default)
    {
        var authResult = await _authService.GetRefreshTokenAsync(request.Token, request.RefreshToken, cancellationToken);

        return authResult.IsSuccess
            ? Ok(authResult.Value)
            : authResult.ToProblem();
        //    : Problem(title: authResult.Error.Code, detail: authResult.Error.Description, statusCode: StatusCodes.Status400BadRequest);

    }
    [HttpPost("register")]
    public async Task<IActionResult> RegisterAsync([FromBody] RegisterRequest request ,  CancellationToken cancellationToken = default)
    {
        var authResult = await _authService.RegisterAsync(request, cancellationToken);

        return authResult.IsSuccess
            ? Ok(authResult.Value)
            : authResult.ToProblem();
        //    : Problem(title: authResult.Error.Code, detail: authResult.Error.Description, statusCode: StatusCodes.Status400BadRequest);

    }

    [HttpPut("revoke-refresh-token")]
    public async Task<IActionResult> RevokeRefreshAsync([FromBody] RefreshTokenRequest request ,  CancellationToken cancellationToken = default)
    {
        var authResult = await _authService.RevokeRefreshTokenAsync(request.Token, request.RefreshToken, cancellationToken);

        return authResult.IsSuccess 
            ? NoContent()
            : authResult.ToProblem();
    }


}
