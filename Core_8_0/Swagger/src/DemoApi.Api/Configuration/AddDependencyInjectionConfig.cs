using DemoApi.Domain.Handlers;
using DemoApi.Domain.Interfaces;

namespace DemoApi.Api.Configuration
{
    public static class DependencyInjectionConfig
    {
        #region Public Methods

        public static IServiceCollection AddDependencyInjectionConfig(this IServiceCollection services)
        {
            #region Others

            services.AddScoped<INotificatorHandler, NotificatorHandler>();

            #endregion

            return services;
        }

        #endregion
    }
}