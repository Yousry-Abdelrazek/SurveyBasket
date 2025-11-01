using SurveyBasket.Contracts.Response;

namespace SurveyBasket.Mapping;

public class MappingConfiguration : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        //config.NewConfig<Poll, PollResponse>()
        //.Map(dest => dest.Notes, src => src.Description);

        config.NewConfig<Student, StudentResponse>()
            .Map(dest => dest.FullName, src => $"{src.FirstName} {src.MiddleName} {src.LastName}")
            .Map(dest => dest.Age, src => DateTime.Now.Year - src.DateOfBirth!.Value.Year, srcCond => srcCond.DateOfBirth.HasValue);
            // .TwoWays(); // => mapping from StudentResponse to Student 

    }
}
