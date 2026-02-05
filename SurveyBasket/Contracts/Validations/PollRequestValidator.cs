
namespace SurveyBasket.Contracts.Validations;

public class PollRequestValidator : AbstractValidator<PollRequest>
{
    public PollRequestValidator()
    {
        RuleFor(x => x.Title)
            .NotEmpty().WithMessage("{PropertyName} is required.")
            .Length(3, 100);


        RuleFor(x => x.Summary)
            .NotEmpty().WithMessage("{PropertyName} is required.")
            .Length(3, 1500);

        RuleFor(x => x.StartsAt)
            .NotEmpty().WithMessage("{PropertyName} is required.")
            .LessThan(x => x.EndsAt).WithMessage("{PropertyName} must be before {ComparisonValue}.");
        //.LessThan(DateOnly.FromDateTime(DateTime.Now)).WithMessage("{PropertyName} must be in the future.");
        RuleFor(x => x.EndsAt)
            .NotEmpty().WithMessage("{PropertyName} is required.")
            .GreaterThan(x => x.StartsAt).WithMessage("{PropertyName} must be after {ComparisonValue}.");
            //.GreaterThan(DateOnly.FromDateTime(DateTime.Now)).WithMessage("{PropertyName} must be in the future."); 
    }

    // PlaceHolders for future validation rules
    // ex => {PropertyName} , {PropertyValue}, {MinLength}, {MaxLength}
}
