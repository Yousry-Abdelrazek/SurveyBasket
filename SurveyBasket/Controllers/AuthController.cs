using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace SurveyBasket.Controllers;

[Route("[controller]")]
[ApiController]
public class AuthController(IAuthService authService) : ControllerBase
{
    private readonly IAuthService _authService = authService;

    [HttpPost("")]
    public async Task<IActionResult> LoginAsync([FromBody] LoginRequest loginRequest , CancellationToken cancellationToken = default)
    {
        var authResult = await _authService.GetTokenAsync(loginRequest.Email, loginRequest.Password, cancellationToken);

        return authResult is null ? BadRequest("Invalid Email/Passowrd") : Ok(authResult);
    }

    [HttpPost("refresh")]
    public async Task<IActionResult> RefreshAsync([FromBody] RefreshTokenRequest request ,  CancellationToken cancellationToken = default)
    {
        var authResult = await _authService.GetRefreshTokenAsync(request.Token, request.RefreshToken, cancellationToken);

        return authResult is null ? BadRequest("Invalid Token/RefreshToken") : Ok(authResult);
    }

    [HttpPut("revoke-refresh-token")]
    public async Task<IActionResult> RevokeRefreshAsync([FromBody] RefreshTokenRequest request ,  CancellationToken cancellationToken = default)
    {
        var isRevoked = await _authService.RevokeRefreshTokenAsync(request.Token, request.RefreshToken, cancellationToken);

        return isRevoked ? Ok("Revoked Successfully") : BadRequest("Operation Failed");
    }


}
