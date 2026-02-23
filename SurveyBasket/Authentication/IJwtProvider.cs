namespace SurveyBasket.Authentication;

public interface IJwtProvider
{
    // method : return 2 values : 1- token 2- expire date
    (string token , int expireIn) GenerateToken(ApplicationUser user);

}
