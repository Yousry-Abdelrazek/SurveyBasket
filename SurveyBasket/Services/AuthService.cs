
using Microsoft.Extensions.Logging.Abstractions;
using System.Security.Cryptography;

namespace SurveyBasket.Services;

public class AuthService(UserManager<ApplicationUser> userManager, IJwtProvider jwtProvider) : IAuthService
{
    private readonly UserManager<ApplicationUser> _userManager = userManager;
    private readonly IJwtProvider _jwtProvider = jwtProvider;

    private readonly int _refreshTokenExpireInDays = 14; // Set the refresh token expiration time (e.g., 7 days)


    public async Task<AuthResponse?> GetTokenAsync(string email, string password, CancellationToken cancellationToken = default)
    {
        // Check User? 
        var user = await _userManager.FindByEmailAsync(email);
        if (user is null)
            return null;

        // Check Password
        var isValidPassword = await _userManager.CheckPasswordAsync(user, password);
        
        if(!isValidPassword)
            return null;

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
        return new AuthResponse(user.Id , user.Email , user.FirstName , user.LastName , Token : token , ExpiresIn: expiresIn * 60 , refreshToken, refreshTokenExpiration);

    }
    public async Task<AuthResponse?> GetRefreshTokenAsync(string token, string refreshToken, CancellationToken cancellationToken = default)
    {
        var userId = _jwtProvider.ValidateToken(token);

        if (userId is null)
            return null; 
        
        var user = await _userManager.FindByIdAsync(userId);

        if (user is null)
            return null;

        var userRefreshToken = user.RefreshTokens.SingleOrDefault(rt => rt.Token == refreshToken && rt.IsActive);

        if (userRefreshToken is null)
            return null;

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
        return new AuthResponse(user.Id, user.Email, user.FirstName, user.LastName, Token: newtoken, ExpiresIn: expiresIn * 60, newrefreshToken, refreshTokenExpiration);



    }

    public async Task<bool> RevokeRefreshTokenAsync(string token, string refreshToken, CancellationToken cancellationToken = default)
    {
        var userId = _jwtProvider.ValidateToken(token);

        if (userId is null)
            return false;

        var user = await _userManager.FindByIdAsync(userId);

        if (user is null)
            return false;

        var userRefreshToken = user.RefreshTokens.SingleOrDefault(rt => rt.Token == refreshToken && rt.IsActive);

        if (userRefreshToken is null)
            return false;

        userRefreshToken.RevokedOn = DateTime.UtcNow;

        await _userManager.UpdateAsync(user);

        return true;
    }
    private static string GenerateRefreshToken()
    {
        return Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));
    }


}
