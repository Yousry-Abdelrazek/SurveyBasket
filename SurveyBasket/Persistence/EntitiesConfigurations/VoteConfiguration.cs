namespace SurveyBasket.Persistence.EntitiesConfigurations;

public class VoteConfiguration : IEntityTypeConfiguration<Vote>
{
    public void Configure(EntityTypeBuilder<Vote> builder)
    {
        // Each user can vote only once per poll 
        builder.HasIndex(x => new { x.PollId, x.UserId }).IsUnique(); 
       
    }
}
