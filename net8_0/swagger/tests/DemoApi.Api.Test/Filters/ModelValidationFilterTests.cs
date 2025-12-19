using DemoApi.Api.Filters;
using DemoApi.Application.Models;
using DemoApi.Domain.Handlers;
using DemoApi.Domain.Interfaces;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Routing;
using Moq;

namespace DemoApi.Api.Test.Filters
{
    public class ModelValidationFilterTests
    {
        [Fact]
        public void OnActionExecuting_ShouldNotModifyContext_WhenModelStateIsValid()
        {
            // Arrange
            var notificator = new Mock<INotificatorHandler>();
            var filter = new ModelValidationFilter(notificator.Object);

            var actionContext = new ActionContext(
                new DefaultHttpContext(),
                new RouteData(),
                new ActionDescriptor(),
                new ModelStateDictionary()
            );

            var context = new ActionExecutingContext(
                actionContext,
                new List<IFilterMetadata>(),
                new Dictionary<string, object?>(),
                new Mock<Controller>().Object
            );

            // Act
            filter.OnActionExecuting(context);

            // Assert
            context.Result.Should().BeNull();
            notificator.Verify(n => n.AddError(It.IsAny<string>()), Times.Never);
        }

        [Fact]
        public void OnActionExecuting_ShouldReturn412_WhenDataAnnotationError()
        {
            // Arrange
            var notificator = new NotificatorHandler();
            var filter = new ModelValidationFilter(notificator);

            var modelState = new ModelStateDictionary();
            modelState.AddModelError("Name", "Name is required");

            var actionContext = new ActionContext(
                new DefaultHttpContext(),
                new RouteData(),
                new ActionDescriptor(),
                modelState
            );

            var context = new ActionExecutingContext(
                actionContext,
                new List<IFilterMetadata>(),
                new Dictionary<string, object?>(),
                new Mock<Controller>().Object
            );

            // Act
            filter.OnActionExecuting(context);

            // Assert
            context.Result.Should().BeOfType<ObjectResult>();
            var objectResult = context.Result as ObjectResult;
            objectResult!.StatusCode.Should().Be(412);
            
            var response = objectResult.Value as ResponseViewModel;
            response.Should().NotBeNull();
            response!.Success.Should().BeFalse();
            response.Errors.Should().Contain("Name is required");
        }

        [Fact]
        public void OnActionExecuting_ShouldReturn400_WhenModelBindingError()
        {
            // Arrange
            var notificator = new NotificatorHandler();
            var filter = new ModelValidationFilter(notificator);

            var modelState = new ModelStateDictionary();
            modelState.AddModelError("Id", "The value 'ABC' is not valid.");

            var actionContext = new ActionContext(
                new DefaultHttpContext(),
                new RouteData(),
                new ActionDescriptor(),
                modelState
            );

            var context = new ActionExecutingContext(
                actionContext,
                new List<IFilterMetadata>(),
                new Dictionary<string, object?>(),
                new Mock<Controller>().Object
            );

            // Act
            filter.OnActionExecuting(context);

            // Assert
            context.Result.Should().BeOfType<ObjectResult>();
            var objectResult = context.Result as ObjectResult;
            objectResult!.StatusCode.Should().Be(400);
            
            var response = objectResult.Value as ResponseViewModel;
            response.Should().NotBeNull();
            response!.Success.Should().BeFalse();
            response.Errors.Should().Contain("The value 'ABC' is not valid.");
        }

        [Fact]
        public void OnActionExecuting_ShouldAddAllErrors_ToNotificator()
        {
            // Arrange
            var notificator = new NotificatorHandler();
            var filter = new ModelValidationFilter(notificator);

            var modelState = new ModelStateDictionary();
            modelState.AddModelError("Name", "Name is required");
            modelState.AddModelError("Weight", "Weight must be greater than 0");

            var actionContext = new ActionContext(
                new DefaultHttpContext(),
                new RouteData(),
                new ActionDescriptor(),
                modelState
            );

            var context = new ActionExecutingContext(
                actionContext,
                new List<IFilterMetadata>(),
                new Dictionary<string, object?>(),
                new Mock<Controller>().Object
            );

            // Act
            filter.OnActionExecuting(context);

            // Assert
            notificator.GetErrors().Should().HaveCount(2);
            notificator.GetErrors().Select(e => e.Message).Should().Contain(new[] 
            { 
                "Name is required", 
                "Weight must be greater than 0" 
            });
        }

        [Fact]
        public void OnActionExecuted_ShouldNotThrowException()
        {
            // Arrange
            var notificator = new Mock<INotificatorHandler>();
            var filter = new ModelValidationFilter(notificator.Object);

            var actionContext = new ActionContext(
                new DefaultHttpContext(),
                new RouteData(),
                new ActionDescriptor()
            );

            var context = new ActionExecutedContext(
                actionContext,
                new List<IFilterMetadata>(),
                new Mock<Controller>().Object
            );

            // Act & Assert
            var act = () => filter.OnActionExecuted(context);
            act.Should().NotThrow();
        }
    }
}