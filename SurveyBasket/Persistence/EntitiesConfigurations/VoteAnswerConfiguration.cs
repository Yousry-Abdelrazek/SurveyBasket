namespace SurveyBasket.Persistence.EntitiesConfigurations;

public class VoteAnswerConfiguration : IEntityTypeConfiguration<VoteAnswer>
{
    public void Configure(EntityTypeBuilder<VoteAnswer> builder)
    {
        // Each vote can have only one answer per question
        builder.HasIndex(x => new {x.VoteId, x.QuestionId }).IsUnique();
    }
}
