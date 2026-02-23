
namespace SurveyBasket.Services;

public interface IAuthService
{
    Task<AuthResponse?> GetTokenAsync(string token , string password , CancellationToken cancellationToken = default) ;
}  
