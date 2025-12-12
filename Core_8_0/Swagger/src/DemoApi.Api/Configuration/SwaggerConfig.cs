using Microsoft.OpenApi.Models;

namespace DemoApi.Api.Configuration
{
    public static class SwaggerConfig
    {
        #region Public Methods

        public static IServiceCollection AddSwaggerConfig(this IServiceCollection services)
        {
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { 
                    Title = "Demo API", 
                    Version = "v1",
                    Description = "Example of an API with versioning."
                });
            });

            return services;
        }

        public static IApplicationBuilder UseSwaggerConfig(this IApplicationBuilder app)
        {
            app.UseSwagger();

            app.UseSwaggerUI(options =>
            {
                options.SwaggerEndpoint("/swagger/v1/swagger.json", "V1");
            });

            return app;
        }

        #endregion
    }
}