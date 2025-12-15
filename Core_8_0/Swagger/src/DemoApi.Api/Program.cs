using DemoApi.Api.Configuration;
using DemoApi.Application.Automapper;

var builder = WebApplication.CreateBuilder(args);

var logger = builder.AddNLogConfig();

try
{
    builder.Services.AddAutoMapper(typeof(AutomapperConfig));

    builder.Services.AddDependencyInjectionConfig();

    builder.Services.AddApiConfig();


    var app = builder.Build();

    app.UseApiConfig(app.Environment);

    app.MapControllers();

    app.Run();
}
catch (Exception ex)
{
    logger.Error(ex, "Stopped program because of exception");
    throw;
}
finally
{
    NLogConfig.Shutdown();
}