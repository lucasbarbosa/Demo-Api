﻿using Demo.Application;
using Demo.Domain.Handlers;
using Demo.Domain.Interfaces;
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

            services.AddScoped<IUserApplication, UserApplication>();

            #endregion

            #region Repositories

            services.AddScoped<IUserRepository, UserRepository>();

            #endregion

            #region Others

            //services.AddScoped<ILogger, NLogLogger>();
            services.AddScoped<INotificator, NotificatorHandler>();

            #endregion

            return services;
        }

        #endregion
    }
}