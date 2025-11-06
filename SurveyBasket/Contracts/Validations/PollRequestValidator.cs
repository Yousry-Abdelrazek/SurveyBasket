
namespace SurveyBasket.Contracts.Validations;

public class PollRequestValidator : AbstractValidator<PollRequest>
{
    public PollRequestValidator()
    {
        RuleFor(x => x.Title)
            .NotEmpty()
            .Length(3, 100);
        RuleFor(x => x.Summary)
            .NotEmpty()
            .Length(3, 1500);

        RuleFor(x => x.StartsAt)
            .NotEmpty()
            .GreaterThanOrEqualTo(DateTime.Today);
        RuleFor(x => x.EndsAt)
            .NotEmpty();
        RuleFor(x => x)
            .Must(x => x.EndsAt > x.StartsAt)
            .WithMessage("EndsAt must be greater than StartsAt");

    }
}
