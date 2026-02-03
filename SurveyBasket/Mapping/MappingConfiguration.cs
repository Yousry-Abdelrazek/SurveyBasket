namespace SurveyBasket.Mapping;

public class MappingConfiguration : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        //config.NewConfig<Student, StudentResponse>()
        //    .Map(dest => dest.FullName,
        //         src => $"{src.FirstName} {src.MiddleName} {src.LastName}")
        //    .Map(dest => dest.Age,
        //            src => DateTime.Now.Year - 
        //                (src.DateOfBirth.HasValue ? src.DateOfBirth.Value.Year : DateTime.Now.Year));

        // mapping Department Name is automatic because of the same name
        // If you want to ignore it, you can use the following line:
        //    .Ignore(dest => dest.DepartmentName);

    }
}
