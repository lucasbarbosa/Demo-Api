using Demo.Application.ViewModels;
using Demo.Domain.Interfaces;
using Demo.Infra.CrossCutting.Interfaces;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using System;
using System.Linq;
using System.Net;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace Demo.Api.Extensions
{
    public class ExceptionMiddleware
    {
        #region Properties

        private readonly RequestDelegate _next;

        #endregion

        #region Constructors

        public ExceptionMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        #endregion

        #region Public Methods

        public async Task InvokeAsync(HttpContext httpContext, ILogger logger, INotificatorHandler notificator)
        {
            try
            {
                await _next(httpContext);
            }
            catch (Exception ex)
            {
                HandleExceptionAsync(httpContext, ex, logger, notificator);
            }
        }

        #endregion

        #region Private Methods

        private static void HandleExceptionAsync(HttpContext context, Exception exception, ILogger logger, INotificatorHandler notificator)
        {
            logger.LogException(exception);

            var responseBody = new ResponseViewModel
            {
                Success = false,
                Errors = notificator.GetErrors().Select(x => x.Message).ToList()
            };

            responseBody.Errors.Add(exception.Message);

            var responseJson = JsonConvert.SerializeObject(responseBody);
            context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
            context.Response.ContentType = new MediaTypeHeaderValue("application/json").ToString();
            context.Response.WriteAsync(responseJson, Encoding.UTF8);
        }

        #endregion
    }
}