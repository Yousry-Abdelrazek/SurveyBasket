namespace SurveyBasket.Contracts.Votes;

public class VoteRequestValidtor : AbstractValidator<VoteRequest>
{
    public VoteRequestValidtor()
    {
        RuleFor(x => x.Answers)
            .NotEmpty();

        RuleForEach(x => x.Answers)
            .SetInheritanceValidator(v => 
                v.Add(new VoteAnswerRequestValidator())
                );
    }
}
