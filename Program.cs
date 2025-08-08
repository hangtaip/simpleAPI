using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using TransactionApi;

var builder = WebApplication.CreateBuilder(new WebApplicationOptions
{
    EnvironmentName = Environments.Development
});
builder.Services.AddValidatorsFromAssemblyContaining<Program>();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddOpenApiDocument(config =>
{
    config.DocumentName = "TransactionAPI";
    config.Title = "TransactionAPI v1";
    config.Version = "v1";
});

// builder.Logging.ClearProviders();
builder.Services.AddLogging(loggingBuilder =>
{
    loggingBuilder.AddLog4Net("TransactionApi.config");
});
builder.Services.AddSingleton<IAppLogger, AppLogger>();
builder.Services.AddSingleton<Endpoints>();
builder.Services.AddSingleton<PrimeCheckService>();

builder.Services.Configure<JsonOptions>(options =>
{
    options.JsonSerializerOptions.PropertyNameCaseInsensitive = true;
});

var app = builder.Build();
if (app.Environment.IsDevelopment())
{
    app.UseOpenApi();
    app.UseSwaggerUi(config =>
    {
        config.DocumentTitle = "TransactionAPI";
        config.Path = "/swagger";
        config.DocumentPath = "/swagger/{documentName}/swagger.json";
        config.DocExpansion = "list";
    });
}

app.UseMiddleware<Validating>();

// app.UseHttpsRedirection();
var endpoints = app.Services.GetRequiredService<Endpoints>();
endpoints.Map(app);

app.Run();
