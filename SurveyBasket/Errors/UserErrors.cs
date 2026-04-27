namespace SurveyBasket.Errors;

public class UserErrors
{
    public static readonly Error InvalidCredentials = 
        new Error("User.InvalidCredentials", "Invalid email/Passowrd", StatusCodes.Status401Unauthorized);
    public static readonly Error InvalidJwtToken = 
        new Error("User.InvalidTokens", "Invalid Access/Refresh Token", StatusCodes.Status401Unauthorized);
    public static readonly Error InvalidRefreshToken = new Error("User.InvalidRefreshToken", "Invalid Refresh Token", StatusCodes.Status401Unauthorized);

    public static readonly Error DuplicatedEmail = 
        new Error("User.DuplicatedEmail", "A user with the same email already exists.", StatusCodes.Status409Conflict);

    public static readonly Error EmailNotConfirmed = 
        new Error("User.EmailNotConfirmed", "Email not confirmed.", StatusCodes.Status401Unauthorized);


    public static readonly Error InvalidCode = 
        new Error("User.InvalidCode", "Invalid confirmation code.", StatusCodes.Status401Unauthorized);

    public static readonly Error DuplicatedEmailConfirmation =
    new Error("User.DuplicatedEmailConfirmation", "Email is already confirmed.", StatusCodes.Status409Conflict);



}
