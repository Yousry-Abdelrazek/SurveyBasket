
using SurveyBasket.Middlewares;
using SurveyBasket.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services.AddScoped<IOS, MacOsService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwaggerUI(options => options.SwaggerEndpoint("/openapi/v1.json", "v1"));
    //app.MapScalarApiReference();
}


//var logger = app.Logger;
//app.Use(async (context, next) =>
//{
//    logger.LogInformation("Processing request");
//    await next(context);
//    logger.LogInformation("Processing response");
//});

//app.UseMiddleware<CustomMiddleware>();

app.UseCustomMiddleware();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();  
