
using Hangfire;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Logging.Abstractions;
using SurveyBasket.Contracts.Users;
using SurveyBasket.Helpers;
using System.Security.Cryptography;
using System.Text;
using static Org.BouncyCastle.Crypto.Engines.SM2Engine;

namespace SurveyBasket.Services;

public class AuthService(
    UserManager<ApplicationUser> userManager,
    IJwtProvider jwtProvider, 
    SignInManager<ApplicationUser> signInManager,
    ILogger<AuthService> logger,
    IEmailSender emailSender, 
    IHttpContextAccessor httpContextAccessor) : IAuthService
{
    private readonly UserManager<ApplicationUser> _userManager = userManager;
    private readonly IJwtProvider _jwtProvider = jwtProvider;
    private readonly SignInManager<ApplicationUser> _signInManager = signInManager;
    private readonly ILogger<AuthService> _logger = logger;
    private readonly IEmailSender _emailSender = emailSender;
    private readonly IHttpContextAccessor _httpContextAccessor = httpContextAccessor;
    private readonly int _refreshTokenExpireInDays = 14; // Set the refresh token expiration time (e.g., 7 days)


    public async Task<Result<AuthResponse>> GetTokenAsync(string email, string password, CancellationToken cancellationToken = default)
    {
        // Check User? 
        if (await _userManager.FindByEmailAsync(email) is not { } user)
            return Result.Failure<AuthResponse>(UserErrors.InvalidCredentials);

        var result = await _signInManager.PasswordSignInAsync(user, password, isPersistent: false, lockoutOnFailure: false);

        // Another way 
        //var result = await _userManager.CheckPasswordAsync(user, password);

        //if (!user.EmailConfirmed)
        //    return Result.Failure<AuthResponse>(UserErrors.EmailNotConfirmed);

        if (result.Succeeded)
        {

            // Generate Jwt Token 
            var (token, expiresIn) = _jwtProvider.GenerateToken(user);

            var refreshToken = GenerateRefreshToken();
            var refreshTokenExpiration = DateTime.UtcNow.AddDays(_refreshTokenExpireInDays);

            user.RefreshTokens.Add(new RefreshToken
            {
                Token = refreshToken,
                ExpiresOn = refreshTokenExpiration
            });

            await _userManager.UpdateAsync(user);
            // return token 
            var response = new AuthResponse(user.Id, user.Email, user.FirstName, user.LastName, Token: token, ExpiresIn: expiresIn * 60, refreshToken, refreshTokenExpiration);

            return Result.Success<AuthResponse>(response);
        }

        return Result.Failure<AuthResponse>(result.IsNotAllowed ? UserErrors.EmailNotConfirmed : UserErrors.InvalidCredentials);

    }
    public async Task<Result<AuthResponse>> GetRefreshTokenAsync(string token, string refreshToken, CancellationToken cancellationToken = default)
    {
        var userId = _jwtProvider.ValidateToken(token);

        if (userId is null)
            return Result.Failure<AuthResponse>(UserErrors.InvalidJwtToken); 
        
        var user = await _userManager.FindByIdAsync(userId);

        if (user is null)
            return Result.Failure<AuthResponse>(UserErrors.InvalidJwtToken);

        var userRefreshToken = user.RefreshTokens.SingleOrDefault(rt => rt.Token == refreshToken && rt.IsActive);

        if (userRefreshToken is null)
            return Result.Failure<AuthResponse>(UserErrors.InvalidJwtToken);

        userRefreshToken.RevokedOn = DateTime.UtcNow;

        // Generate Jwt Token 
        var (newtoken, expiresIn) = _jwtProvider.GenerateToken(user);

        var newrefreshToken = GenerateRefreshToken();
        var refreshTokenExpiration = DateTime.UtcNow.AddDays(_refreshTokenExpireInDays);

        user.RefreshTokens.Add(new RefreshToken
        {
            Token = newrefreshToken,
            ExpiresOn = refreshTokenExpiration
        });

        await _userManager.UpdateAsync(user);
        // return token 
        var response = 
            new AuthResponse(user.Id, user.Email, user.FirstName, user.LastName, Token: newtoken, ExpiresIn: expiresIn * 60, newrefreshToken, refreshTokenExpiration);

        return Result.Success(response);


    }

    public async Task<Result> RevokeRefreshTokenAsync(string token, string refreshToken, CancellationToken cancellationToken = default)
    {
        var userId = _jwtProvider.ValidateToken(token);

        if (userId is null)
            return Result.Failure(UserErrors.InvalidJwtToken);

        var user = await _userManager.FindByIdAsync(userId);

        if (user is null)
            return Result.Failure(UserErrors.InvalidJwtToken); 

        var userRefreshToken = user.RefreshTokens.SingleOrDefault(rt => rt.Token == refreshToken && rt.IsActive);

        if (userRefreshToken is null)
            return Result.Failure(UserErrors.InvalidJwtToken);

        userRefreshToken.RevokedOn = DateTime.UtcNow;

        await _userManager.UpdateAsync(user);

        return Result.Success();
    }
    public async Task<Result> RegisterAsync(RegisterRequest request, CancellationToken cancellationToken = default)
    {


        // If email already exists, return error

        if (await _userManager.FindByEmailAsync(request.Email) is not null)
            return Result.Failure<AuthResponse>(UserErrors.DuplicatedEmail);

        var user = request.Adapt<ApplicationUser>();

        var result = await _userManager.CreateAsync(user, request.Password);

        if(result.Succeeded)
        {
            var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));

            _logger.LogInformation("Confirmation code for {Email}: {Code}", user.Email, code);

            // TODO : Send Email 
            await SendConfirmationEmail(user, code);



            return Result.Success();
        }



        var error = result.Errors.First();
        
        return Result.Failure<AuthResponse>(new Error(error.Code, error.Description , StatusCodes.Status404NotFound));

    }
    public async Task<Result> ConfirmEmailAsync(ConfirmEmailRequest request)
    {
        if(await _userManager.FindByIdAsync(request.UserId) is not { } user)
            return Result.Failure(UserErrors.InvalidCode);

        if (user.EmailConfirmed)
            return Result.Failure(UserErrors.DuplicatedEmailConfirmation);

        var code = request.Code;
        try
        {
            code = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(code));
            
        }
        catch (FormatException)
        {
            return Result.Failure(UserErrors.InvalidCode);
        }

        var result = await _userManager.ConfirmEmailAsync(user, code);

        if (result.Succeeded) 
            return Result.Success();

        var error = result.Errors.First();
        return Result.Failure(new Error(error.Code, error.Description, StatusCodes.Status400BadRequest));

    }
    public async Task<Result> ResendConfirmationEmailAsync (ResendConfirmationEmailRequest request)
    {
        if (await _userManager.FindByEmailAsync(request.Email) is not { } user)
            return Result.Success(); // Don't reveal that the email doesn't exist

        if(user.EmailConfirmed)
            return Result.Failure(UserErrors.DuplicatedEmailConfirmation);

        var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
        code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));

        _logger.LogInformation("Confirmation code for {Email}: {Code}", user.Email, code);

        // Send TO Email 
        await SendConfirmationEmail(user, code);

        return Result.Success();
    }

    public async Task<Result> SendResetPasswordCodeAsync(string email)
    {
        if(await _userManager.FindByEmailAsync(email) is not { } user)
            return Result.Success(); // Don't reveal that the email doesn't exist) is not { } user)


        var code = await _userManager.GeneratePasswordResetTokenAsync(user);
        code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));

        
        _logger.LogInformation("Password reset code for {Email}: {Code}", user.Email, code);

        await SendResetPasswordCode(user, code);

        return Result.Success();



    }

    public async Task<Result> ResetPasswordAsync(ResetPasswordRequest request)
    {
        var user = await _userManager.FindByEmailAsync(request.Email);
        if (user is null)
            return Result.Failure(UserErrors.InvalidCode); // Don't reveal that the email doesn't exist

        if(!user.EmailConfirmed)
            return Result.Failure(UserErrors.EmailNotConfirmed);

        IdentityResult result;

        try
        {
            var code = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(request.Code));
            result = await _userManager.ResetPasswordAsync(user, code, request.NewPassword);
        }
        catch (FormatException)
        {
            result = IdentityResult.Failed(_userManager.ErrorDescriber.InvalidToken());
        }

        if(result.Succeeded)
            return Result.Success();

        var error = result.Errors.First();
        return Result.Failure(new Error(error.Code, error.Description, StatusCodes.Status401Unauthorized));
    }



    private static string GenerateRefreshToken()
    {
        return Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));
    }

    private async Task SendConfirmationEmail(ApplicationUser user , string code)
    {
        // Read Origin from request header 
        var origin = _httpContextAccessor.HttpContext?.Request.Headers.Origin;


        var emailBody = EmailBodyBuilder.GenerateEmailBody("EmailConfirmation",
               new Dictionary<string, string>
               {
                       {"{{name}}" , user.FirstName },
                       {"{{action_url}}" ,  $"{origin}/auth/emailConfirmation?userId={user.Id}&code={code}"}
               }
            );

        BackgroundJob.Enqueue(() => _emailSender.SendEmailAsync(user.Email!, "✅ Survey Basket : Confirm your email", emailBody));

        await Task.CompletedTask;

    }
    private async Task SendResetPasswordCode(ApplicationUser user, string code)
    {
        // Read Origin from request header
        var origin = _httpContextAccessor.HttpContext?.Request.Headers.Origin;

        var emailBody = EmailBodyBuilder.GenerateEmailBody("ForgetPassword",
               new Dictionary<string, string>
               {
                       {"{{name}}" , user.FirstName },
                       {"{{action_url}}" ,  $"{origin}/auth/resetPassword?email={user.Email}&code={code}"}
               }
            );

        BackgroundJob.Enqueue(() => _emailSender.SendEmailAsync(user.Email!, "✅ Survey Basket : Reset your password", emailBody));

        await Task.CompletedTask;


    }


}
