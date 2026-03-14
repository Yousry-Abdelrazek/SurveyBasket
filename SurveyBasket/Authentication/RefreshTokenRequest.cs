namespace SurveyBasket.Authentication;

public record RefreshTokenRequest(
    string Token,
    string RefreshToken

    );
