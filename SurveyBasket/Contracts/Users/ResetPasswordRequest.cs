namespace SurveyBasket.Contracts.Users;

public record ResetPasswordRequest
(
    string Email,
    string NewPassword,
    string Code
);