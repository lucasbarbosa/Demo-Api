using DemoApi.Api.Configuration;
using DemoApi.Application.Automapper;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddAutoMapper(typeof(AutomapperConfig));

builder.Services.AddDependencyInjectionConfig();

builder.Services.AddApiConfig();

builder.Services.AddSwaggerConfig();


var app = builder.Build();

app.UseApiConfig(app.Environment);

app.UseSwaggerConfig();

app.MapControllers();

app.Run();
