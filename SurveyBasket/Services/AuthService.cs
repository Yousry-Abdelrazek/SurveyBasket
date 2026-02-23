
namespace SurveyBasket.Services;

public class AuthService(UserManager<ApplicationUser> userManager, IJwtProvider jwtProvider) : IAuthService
{
    private readonly UserManager<ApplicationUser> _userManager = userManager;
    private readonly IJwtProvider _jwtProvider = jwtProvider;

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

        // return token 
        return new AuthResponse(user.Id , user.Email , user.FirstName , user.LastName , Token : token , ExpiresIn: expiresIn * 60);

    }


}
