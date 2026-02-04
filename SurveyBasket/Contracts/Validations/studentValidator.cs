namespace SurveyBasket.Contracts.Validations;

public class StudentValidator : AbstractValidator<Student>
{
    public StudentValidator()
    {
        RuleFor(x => x.DateOfBirth)
            .Must(BeMoreThan18Years)
            .When(x => x.DateOfBirth.HasValue)
            .WithMessage("Student must be at least 18 years old.");

    }
    private bool BeMoreThan18Years(DateTime? dateOfBirth) => DateTime.Today > dateOfBirth!.Value.AddYears(18);

}
