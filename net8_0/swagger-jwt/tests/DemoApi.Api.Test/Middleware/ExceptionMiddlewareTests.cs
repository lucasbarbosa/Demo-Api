using DemoApi.Api.Extensions;
using DemoApi.Application.Models;
using DemoApi.Domain.Handlers;
using DemoApi.Domain.Interfaces;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Moq;
using Newtonsoft.Json;
using ILogger = DemoApi.Infra.CrossCutting.Interfaces.ILogger;

namespace DemoApi.Api.Test.Middleware
{
    public class ExceptionMiddlewareTests
    {
        [Fact]
        public async Task InvokeAsync_ShouldCallNextDelegate_WhenNoExceptionOccurs()
        {
            // Arrange
            var nextDelegateCalled = false;
            RequestDelegate next = (HttpContext hc) =>
            {
                nextDelegateCalled = true;
                return Task.CompletedTask;
            };

            var middleware = new ExceptionMiddleware(next);
            var context = new DefaultHttpContext();
            var logger = new Mock<ILogger>();
            var notificator = new Mock<INotificatorHandler>();

            // Act
            await middleware.InvokeAsync(context, logger.Object, notificator.Object);

            // Assert
            nextDelegateCalled.Should().BeTrue();
            logger.Verify(l => l.LogException(It.IsAny<Exception>()), Times.Never);
        }

        [Fact]
        public async Task InvokeAsync_ShouldReturn500_WhenUnhandledExceptionOccurs()
        {
            // Arrange
            var expectedException = new InvalidOperationException("Test exception");
            RequestDelegate next = (HttpContext hc) => throw expectedException;

            var middleware = new ExceptionMiddleware(next);
            var context = new DefaultHttpContext();
            context.Response.Body = new MemoryStream();

            var logger = new Mock<ILogger>();
            var notificator = new Mock<INotificatorHandler>();
            notificator.Setup(n => n.GetErrors()).Returns(new List<Notification>());

            // Act
            await middleware.InvokeAsync(context, logger.Object, notificator.Object);

            // Assert
            context.Response.StatusCode.Should().Be(500);
            context.Response.ContentType.Should().Be("application/json");
        }

        [Fact]
        public async Task InvokeAsync_ShouldLogException_WhenExceptionThrown()
        {
            // Arrange
            var expectedException = new InvalidOperationException("Test exception");
            RequestDelegate next = (HttpContext hc) => throw expectedException;

            var middleware = new ExceptionMiddleware(next);
            var context = new DefaultHttpContext();
            context.Response.Body = new MemoryStream();

            var logger = new Mock<ILogger>();
            var notificator = new Mock<INotificatorHandler>();
            notificator.Setup(n => n.GetErrors()).Returns(new List<Notification>());

            // Act
            await middleware.InvokeAsync(context, logger.Object, notificator.Object);

            // Assert
            logger.Verify(l => l.LogException(expectedException), Times.Once);
        }

        [Fact]
        public async Task InvokeAsync_ShouldReturnStandardizedResponse_OnError()
        {
            // Arrange
            var expectedException = new InvalidOperationException("Test exception message");
            RequestDelegate next = (HttpContext hc) => throw expectedException;

            var middleware = new ExceptionMiddleware(next);
            var context = new DefaultHttpContext();
            context.Response.Body = new MemoryStream();

            var logger = new Mock<ILogger>();
            var notificator = new Mock<INotificatorHandler>();
            notificator.Setup(n => n.GetErrors()).Returns(new List<Notification>
            {
                new Notification("Domain error 1")
            });

            // Act
            await middleware.InvokeAsync(context, logger.Object, notificator.Object);

            // Assert
            context.Response.Body.Seek(0, SeekOrigin.Begin);
            var responseBody = await new StreamReader(context.Response.Body).ReadToEndAsync();
            var response = JsonConvert.DeserializeObject<ResponseViewModel>(responseBody);

            response.Should().NotBeNull();
            response!.Success.Should().BeFalse();
            response.Errors.Should().Contain("Domain error 1");
            response.Errors.Should().Contain("Test exception message");
        }

        [Fact]
        public async Task InvokeAsync_ShouldIncludeDomainErrors_InResponse()
        {
            // Arrange
            var expectedException = new Exception("Exception message");
            RequestDelegate next = (HttpContext hc) => throw expectedException;

            var middleware = new ExceptionMiddleware(next);
            var context = new DefaultHttpContext();
            context.Response.Body = new MemoryStream();

            var logger = new Mock<ILogger>();
            var notificator = new Mock<INotificatorHandler>();
            notificator.Setup(n => n.GetErrors()).Returns(new List<Notification>
            {
                new Notification("Validation error 1"),
                new Notification("Validation error 2")
            });

            // Act
            await middleware.InvokeAsync(context, logger.Object, notificator.Object);

            // Assert
            context.Response.Body.Seek(0, SeekOrigin.Begin);
            var responseBody = await new StreamReader(context.Response.Body).ReadToEndAsync();
            var response = JsonConvert.DeserializeObject<ResponseViewModel>(responseBody);

            response!.Errors.Should().HaveCount(3);
            response.Errors.Should().Contain("Validation error 1");
            response.Errors.Should().Contain("Validation error 2");
            response.Errors.Should().Contain("Exception message");
        }
    }
}