using MapsterMapper;
using Microsoft.AspNetCore.Identity;
using SharpGrip.FluentValidation.AutoValidation.Mvc;
using SharpGrip.FluentValidation.AutoValidation.Mvc.Extensions;
using SurveyBasket;
using SurveyBasket.Contracts.Validations;
using SurveyBasket.Persistence;

var builder = WebApplication.CreateBuilder(args);



builder.Services.AddDependencies(builder.Configuration);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwaggerUI(options => options.SwaggerEndpoint("/openapi/v1.json","v1"));
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
