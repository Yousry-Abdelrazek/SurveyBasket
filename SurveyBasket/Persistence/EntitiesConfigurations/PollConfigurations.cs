namespace SurveyBasket.Persistence.EntitiesConfigurations;

public class PollConfigurations : IEntityTypeConfiguration<Poll>
{
    public void Configure(EntityTypeBuilder<Poll> builder)
    {
    //    modelBuilder.Entity<Poll>()
    //.Property(x => x.Title)
    //.HasMaxLength(100);

        builder.HasIndex(x => x.Title).IsUnique();
        builder.Property(x => x.Title).HasMaxLength(100);
        builder.Property(x => x.Summary).HasMaxLength(1500);

    }
}
