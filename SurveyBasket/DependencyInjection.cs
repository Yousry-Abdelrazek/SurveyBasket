using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace SurveyBasket;

public static class DependencyInjection
{
    public static IServiceCollection AddDependencies (this IServiceCollection services , 
        IConfiguration configuration)
    {
        services.AddControllers();

        services.AddAuthConfig(configuration);

        services.AddSwaggerServices()
                .AddMapsterConfig()
                .AddFluentValidationConfig();
                

        var connectionString = configuration.GetConnectionString("DefaultConnection") ??
            throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

        services.AddDbContext<ApplicationDbContext>(options =>
           options.UseSqlServer(connectionString));

        services.AddScoped<IPollService, PollService>();
        services.AddScoped<IAuthService, AuthService>();

        return services;
    }
    private static IServiceCollection AddSwaggerServices(this IServiceCollection services)
    {
        services.AddOpenApi();
        services.AddSwaggerGen();
        return services;
    }
    private static IServiceCollection AddMapsterConfig(this IServiceCollection services)
    {
        var mappingConfig = TypeAdapterConfig.GlobalSettings;
        mappingConfig.Scan(Assembly.GetExecutingAssembly());

        services.AddSingleton<IMapper>(new Mapper(mappingConfig));
        return services;
    }
    private static IServiceCollection AddFluentValidationConfig(this IServiceCollection services)
    {
        services.AddFluentValidationAutoValidation()
                 .AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());
        return services;
    }
    private static IServiceCollection AddAuthConfig(this IServiceCollection services
        , IConfiguration configuration )
    {
        services.AddSingleton<IJwtProvider, JwtProvider>();

        services.AddIdentity<ApplicationUser, IdentityRole>()
        .AddEntityFrameworkStores<ApplicationDbContext>();

        services.Configure<JwtOptions>(configuration.GetSection(JwtOptions.SectionName));

        var setting = configuration.GetSection(JwtOptions.SectionName).Get<JwtOptions>();


        services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme; 
            // to specify that the default authentication scheme is JWT Bearer, which means that the application will expect JWT tokens in the Authorization header of incoming requests for authentication.
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            // to specify that the default challenge scheme is JWT Bearer, which means that if an unauthenticated user tries to access a protected resource, the application will respond with a challenge that indicates that the client should provide a JWT token for authentication.
        })
            .AddJwtBearer(o =>
            {
                o.SaveToken = true; // Anytime while request come to the server with token in header , this token will be saved in the context of the request to be used later in the controllers
                o.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true, // to validate the signature of the token
                    ValidateIssuer = true, // to validate the issuer of the token
                    ValidateAudience = true, // to validate the audience of the token
                    ValidateLifetime = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(setting?.Key!)),
                    ValidIssuer = configuration[setting?.Issuer!],
                    ValidAudience = configuration[setting?.Audience!]

                };

            
            
            }
            );

        return services;
    }



}
