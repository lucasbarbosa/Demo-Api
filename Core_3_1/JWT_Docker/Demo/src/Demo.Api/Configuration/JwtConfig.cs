using Demo.Api.Extensions;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace Demo.Api.Configuration
{
    public static class JwtConfig
    {
        #region Public Methods

        public static IServiceCollection AddJwtConfig(this IServiceCollection services, IConfiguration configuration)
        {
            #region Macoratti

            //services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options =>
            //options.TokenValidationParameters = new TokenValidationParameters
            //{
            //    ValidateIssuer = false,
            //    ValidateAudience = false,
            //    ValidateLifetime = true,
            //    ValidateIssuerSigningKey = true,
            //    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["Authorization:SecurityKey"])),
            //    ClockSkew = System.TimeSpan.Zero
            //});

            #endregion

            var authorizationSection = configuration.GetSection("Authorization");
            var authorization = authorizationSection.Get<AuthorizationSettings>();
            var key = Encoding.ASCII.GetBytes(authorization.SecurityKey);

            services.Configure<AuthorizationSettings>(authorizationSection);

            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(options =>
            {
                options.RequireHttpsMetadata = true;
                options.SaveToken = true;
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidAudience = authorization.ValidOn,
                    ValidIssuer = authorization.Sender
                };
            });

            return services;
        }

        public static IApplicationBuilder UseJwtConfig(this IApplicationBuilder app)
        {
            app.UseAuthorization();

            return app;
        }

        #endregion
    }
}