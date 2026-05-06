using SurveyBasket.Abstractions.Consts;

namespace SurveyBasket.Contracts.Users;

public class ChangePasswordRequestValidator : AbstractValidator<ChangePasswordRequest>
{
    public ChangePasswordRequestValidator()
    {
        RuleFor(x => x.CurrentPassword)
            .NotEmpty()
            .WithMessage("Current password is required.");

        RuleFor(x => x.NewPassword)
           .NotEmpty().WithMessage("New password is required.")
           .NotEqual(x => x.CurrentPassword).WithMessage("New password must be different from the current password.")
           .Matches(RegexPatterns.PasswordPattern).WithMessage("New password must be at least 8 characters long and contain at least one uppercase letter, one lowercase letter, one number, and one special character.");

    }
}
