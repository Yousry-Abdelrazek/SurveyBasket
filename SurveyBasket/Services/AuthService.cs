
using Microsoft.Extensions.Logging.Abstractions;
using System.Security.Cryptography;

namespace SurveyBasket.Services;

public class AuthService(UserManager<ApplicationUser> userManager, IJwtProvider jwtProvider) : IAuthService
{
    private readonly UserManager<ApplicationUser> _userManager = userManager;
    private readonly IJwtProvider _jwtProvider = jwtProvider;

    private readonly int _refreshTokenExpireInDays = 14; // Set the refresh token expiration time (e.g., 7 days)


    public async Task<Result<AuthResponse>> GetTokenAsync(string email, string password, CancellationToken cancellationToken = default)
    {
        // Check User? 
        var user = await _userManager.FindByEmailAsync(email);
        if (user is null)
            return Result.Failure<AuthResponse>(UserErrors.InvalidCredentials);

        // Check Password
        var isValidPassword = await _userManager.CheckPasswordAsync(user, password);
        
        if(!isValidPassword)
            return Result.Failure<AuthResponse>(UserErrors.InvalidCredentials);

        // Generate Jwt Token 
        var (token , expiresIn) = _jwtProvider.GenerateToken(user);

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
    public async Task<Result<AuthResponse>> RegisterAsync(RegisterRequest request, CancellationToken cancellationToken = default)
    {
        var emailIsExists = await _userManager.FindByEmailAsync(request.Email) is not null ;

        if(emailIsExists)
            return Result.Failure<AuthResponse>(UserErrors.DuplicatedEmail);

        var user = request.Adapt<ApplicationUser>();

        var result = await _userManager.CreateAsync(user, request.Password);

        if(result.Succeeded)
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

        var error = result.Errors.First();
        
        return Result.Failure<AuthResponse>(new Error(error.Code, error.Description , StatusCodes.Status404NotFound));

    }


    private static string GenerateRefreshToken()
    {
        return Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));
    }


}
