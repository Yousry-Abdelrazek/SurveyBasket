namespace SurveyBasket.Errors;

public class UserErrors
{
    public static readonly Error InvalidCredentials = new Error("User.InvalidCredentials", "Invalid email/Passowrd");
    public static readonly Error InvalidJwtToken = new Error("User.InvalidTokens", "Invalid Access/Refresh Token");
}
