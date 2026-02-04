
namespace SurveyBasket.Contracts.Validations;

public class CreatePollRequestValidator : AbstractValidator<CreatePollRequest>
{
    public CreatePollRequestValidator()
    {
        RuleFor(x => x.Title)
            .NotEmpty().WithMessage("{PropertyName} is required.")
            .Length(3, 100);


        RuleFor(x => x.Description)
            .NotEmpty().WithMessage("{PropertyName} is required.")
            .Length(3, 1000);
    }

    // PlaceHolders for future validation rules
    // ex => {PropertyName} , {PropertyValue}, {MinLength}, {MaxLength}
}
