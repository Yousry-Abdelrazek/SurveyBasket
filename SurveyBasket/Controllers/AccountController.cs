using Microsoft.AspNetCore.Authorization;
using SurveyBasket.Contracts.Users;
using SurveyBasket.Extensions;
namespace SurveyBasket.Controllers;

[Route("me")]
[ApiController]
[Authorize]
public class AccountController(IUserService userService) : ControllerBase
{
    private readonly IUserService _userService = userService;

    [HttpGet]
    public async Task<IActionResult> Info()
    {

        var result = await _userService.GetProfileAsync(User.GetUserId()!);

        return Ok(result.Value);
    }

    [HttpPut("update-profile")]
    public async Task<IActionResult> UpdateProfile(UpdateProfileRequest request)
    {
        var result = await _userService.UpdateProfileAsync(User.GetUserId()!, request);

        return NoContent();
    }

    [HttpPut("change-pass")]
    public async Task<IActionResult> ChangePassword (ChangePasswordRequest request)
    {
        var result = await _userService.ChangePasswordAsync(User.GetUserId()!, request);

        return result.IsSuccess ? NoContent() : result.ToProblem();
    }

}
