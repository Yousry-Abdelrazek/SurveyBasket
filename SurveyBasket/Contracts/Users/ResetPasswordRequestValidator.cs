using SurveyBasket.Abstractions.Consts;

namespace SurveyBasket.Contracts.Users;

public class ResetPasswordRequestValidator : AbstractValidator<ResetPasswordRequest>
{
    public ResetPasswordRequestValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty()
            .EmailAddress();
        RuleFor(x => x.NewPassword)
            .NotEmpty()
            .Matches(RegexPatterns.PasswordPattern);
        RuleFor(x => x.Code)
            .NotEmpty();
    }
}
