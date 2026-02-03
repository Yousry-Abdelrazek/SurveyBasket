namespace SurveyBasket.Mapping;

public class MappingConfiguration : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        config.NewConfig<Poll, PollResponse>()
            .Map(dest => dest.Notes , src => src.Description); // Custom mapping from Description to Notes
    }
}
