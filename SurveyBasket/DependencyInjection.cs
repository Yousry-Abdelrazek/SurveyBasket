namespace SurveyBasket;

public static class DependencyInjection
{
    public static IServiceCollection AddDependencies (this IServiceCollection services)
    {
        services.AddControllers();

        services.AddSwaggerServices()
                .AddMapsterConf()
                .AddFluentValidationConf();

        services.AddScoped<IPollService, PollService>();

        return services;
    }
    public static IServiceCollection AddSwaggerServices(this IServiceCollection services)
    {
        services.AddOpenApi();
        services.AddSwaggerGen();
        return services;
    }
    public static IServiceCollection AddMapsterConf(this IServiceCollection services)
    {
        var mappingConfig = TypeAdapterConfig.GlobalSettings;
        mappingConfig.Scan(Assembly.GetExecutingAssembly());

        services.AddSingleton<IMapper>(new Mapper(mappingConfig));
        return services;
    }
    public static IServiceCollection AddFluentValidationConf(this IServiceCollection services)
    {
        services.AddFluentValidationAutoValidation()
                 .AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());
        return services;
    }



}
