namespace DemoApi.Api.Configuration
{
    public static class SwaggerConfig
    {
        #region Public Methods

        public static IServiceCollection AddSwaggerConfig(this IServiceCollection services)
        {
            services.AddSwaggerGen();

            return services;
        }

        public static IApplicationBuilder UseSwaggerConfig(this IApplicationBuilder app)
        {
            app.UseSwagger();

            app.UseSwaggerUI();

            return app;
        }

        #endregion
    }
}