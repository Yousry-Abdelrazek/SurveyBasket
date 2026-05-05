using Hangfire;
using Hangfire.Dashboard;
using HangfireBasicAuthenticationFilter;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddDependencies(builder.Configuration);




builder.Host.UseSerilog((context, configuration) =>
    configuration.ReadFrom.Configuration(context.Configuration)
);


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseSerilogRequestLogging();

app.UseHttpsRedirection();

app.UseHangfireDashboard("/Jobs", new DashboardOptions
{
    Authorization =
    [
        new HangfireCustomBasicAuthenticationFilter
        {
            User = app.Configuration.GetValue<string>("HangfireSettings:UserName"),
            Pass = app.Configuration.GetValue<string>("HangfireSettings:Password")
        }
    ],
    DashboardTitle = "SurveyBasket - Hangfire Dashboard", 
    //IsReadOnlyFunc = (DashboardContext context) => true // Set the dashboard to read-only mode
});

var scopeFactory = app.Services.GetRequiredService<IServiceScopeFactory>();
using var scope = scopeFactory.CreateScope();
var notificationService = scope.ServiceProvider.GetRequiredService<INotificationService>();

RecurringJob.AddOrUpdate("SendNewPollNotification", () => notificationService.SendNewPollNotification(null), Cron.Daily);

//RecurringJob.AddOrUpdate<INotificationService>(recurringJobId: "SendNewPollNotification", x => x.SendNewPollNotification(null), Cron.Daily);

app.UseAuthorization();

app.MapControllers();

app.UseExceptionHandler();

app.Run();
