using DemoApi.Api.Configuration;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddApiConfig();

builder.Services.AddSwaggerConfig();

var app = builder.Build();

app.UseApiConfig(app.Environment);

app.UseSwaggerConfig();

app.MapControllers();

app.Run();
