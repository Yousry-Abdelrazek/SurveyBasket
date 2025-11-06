using MapsterMapper;
using SharpGrip.FluentValidation.AutoValidation.Mvc.Extensions;
using SurveyBasket.Persistence;

namespace SurveyBasket;

public static class DependencyInjection
{
    public static IServiceCollection AddDependencies (this IServiceCollection services , IConfiguration configuration)
    {
        // Add services to the container.

        services.AddControllers();
        var connectionString = configuration.GetConnectionString("DefaultConnection") ??
            throw new InvalidOperationException("Connection string 'DefaultConnection' not found. ");

        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseSqlServer(connectionString));
        


        services
            .AddswaggerServices()
            .AddFluentValidation()
            .AddMapsterServices();

        services.AddScoped<IPollService, PollService>();

        return services;
    }

    public static IServiceCollection AddswaggerServices(this IServiceCollection services)
    {
        services.AddOpenApi();
        return services;
    }
    public static IServiceCollection AddMapsterServices(this IServiceCollection services)
    {

        var mappingConfig = TypeAdapterConfig.GlobalSettings;
        mappingConfig.Scan(Assembly.GetExecutingAssembly());
        services.AddSingleton<IMapper>(new Mapper(mappingConfig));

        return services;
    }
    public static IServiceCollection AddFluentValidation(this IServiceCollection services)
    {
        //services.AddScoped<IValidator<CreatePollRequest>, CreatePollRequestValidator>();
        services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());
        services.AddFluentValidationAutoValidation();

        return services;

    }
}
