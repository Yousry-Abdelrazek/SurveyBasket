namespace SurveyBasket.Contracts.Questions;

public class QuestionRequestValidator : AbstractValidator<QuestionRequest>
{
    public QuestionRequestValidator()
    {
        RuleFor(x => x.Content)
            .NotEmpty()
            .Length(3, 1000);

        RuleFor(x => x.Answers)
            .NotNull()
            .WithMessage("Question should have answers");

        RuleFor(x => x.Answers)
            .Must(x => x.Count > 1)
            .WithMessage("Question should has at least two answers")
            .When(x => x.Answers != null);
            

        RuleFor(x => x.Answers)
            .Must(x => x.Distinct().Count() == x.Count)
            .WithMessage("Question should not have duplicate answers for the same question")
            .When(x => x.Answers != null);

    }
}
