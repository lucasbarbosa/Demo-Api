using Demo.Application.Interfaces;
using Demo.Application.Services;
using Demo.Domain.Handlers;
using Demo.Domain.Interfaces;
using Demo.Infra.CrossCutting.Interfaces;
using Demo.Infra.CrossCutting.Logging;
using Demo.Infra.Data.Interfaces;
using Demo.Infra.Data.Repositories;
using Microsoft.Extensions.DependencyInjection;

namespace Demo.Api.Configuration
{
    public static class DependencyInjectionConfig
    {
        #region Public Methods

        public static IServiceCollection AddDependencyInjectionConfig(this IServiceCollection services)
        {
            #region Applications

            services.AddScoped<IProductAppService, ProductAppService>();

            #endregion

            #region Repositories

            services.AddScoped<IProductRepository, ProductRepository>();

            #endregion

            #region Others

            services.AddScoped<ILogger, NLogLogger>();
            services.AddScoped<INotificatorHandler, NotificatorHandler>();

            #endregion

            return services;
        }

        #endregion
    }
}