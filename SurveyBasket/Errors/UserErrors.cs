namespace SurveyBasket.Errors;

public class UserErrors
{
    public static readonly Error InvalidCredentials = 
        new Error("User.InvalidCredentials", "Invalid email/Passowrd", StatusCodes.Status401Unauthorized);
    public static readonly Error InvalidJwtToken = 
        new Error("User.InvalidTokens", "Invalid Access/Refresh Token", StatusCodes.Status401Unauthorized);
    public static readonly Error InvalidRefreshToken = new Error("User.InvalidRefreshToken", "Invalid Refresh Token", StatusCodes.Status401Unauthorized);
     
}
