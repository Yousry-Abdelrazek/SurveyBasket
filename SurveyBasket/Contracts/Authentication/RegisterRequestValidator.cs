using SurveyBasket.Abstractions.Consts;

namespace SurveyBasket.Contracts.Authentication;

public class RegisterRequestValidator : AbstractValidator<RegisterRequest>
{
    public RegisterRequestValidator()
    {
        RuleFor(x => x.FirstName)
            .NotEmpty().WithMessage("First name is required.")
            .Length(3,max: 100).WithMessage("First name must be between 3 and 100 characters.");
        RuleFor(x => x.LastName)
            .NotEmpty().WithMessage("Last name is required.")
            .Length(3, 100).WithMessage("Last name must be between 3 and 100 characters.");
        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email is required.")
            .EmailAddress().WithMessage("Invalid email format.");
            
        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("Password is required.")
            .Matches(RegexPatterns.PasswordPattern).WithMessage("Password must be at least 8 characters long and contain at least one uppercase letter, one lowercase letter, one number, and one special character.");
    }
}

