using DemoApi.Domain.Handlers;
using FluentAssertions;

namespace DemoApi.Application.Test.Handlers
{
    public class NotificatorHandlerTests
    {
        [Fact]
        public void Constructor_ShouldInitializeEmptyErrorList()
        {
            // Arrange & Act
            var notificator = new NotificatorHandler();

            // Assert
            notificator.HasErrors().Should().BeFalse();
            notificator.GetErrors().Should().BeEmpty();
        }

        [Fact]
        public void AddError_ShouldAddNotificationToList()
        {
            // Arrange
            var notificator = new NotificatorHandler();
            var errorMessage = "Test error message";

            // Act
            notificator.AddError(errorMessage);

            // Assert
            notificator.HasErrors().Should().BeTrue();
            notificator.GetErrors().Should().HaveCount(1);
            notificator.GetErrors().First().Message.Should().Be(errorMessage);
        }

        [Fact]
        public void AddError_ShouldAddMultipleNotifications()
        {
            // Arrange
            var notificator = new NotificatorHandler();

            // Act
            notificator.AddError("Error 1");
            notificator.AddError("Error 2");
            notificator.AddError("Error 3");

            // Assert
            notificator.HasErrors().Should().BeTrue();
            notificator.GetErrors().Should().HaveCount(3);
        }

        [Fact]
        public void HasErrors_ShouldReturnTrue_WhenErrorsExist()
        {
            // Arrange
            var notificator = new NotificatorHandler();
            notificator.AddError("Some error");

            // Act
            var result = notificator.HasErrors();

            // Assert
            result.Should().BeTrue();
        }

        [Fact]
        public void HasErrors_ShouldReturnFalse_WhenNoErrors()
        {
            // Arrange
            var notificator = new NotificatorHandler();

            // Act
            var result = notificator.HasErrors();

            // Assert
            result.Should().BeFalse();
        }

        [Fact]
        public void GetErrors_ShouldReturnAllNotifications()
        {
            // Arrange
            var notificator = new NotificatorHandler();
            notificator.AddError("Error 1");
            notificator.AddError("Error 2");

            // Act
            var errors = notificator.GetErrors();

            // Assert
            errors.Should().HaveCount(2);
            errors.Select(e => e.Message).Should().Contain(new[] { "Error 1", "Error 2" });
        }

        [Fact]
        public void GetErrors_ShouldReturnEmptyList_WhenNoErrorsAdded()
        {
            // Arrange
            var notificator = new NotificatorHandler();

            // Act
            var errors = notificator.GetErrors();

            // Assert
            errors.Should().BeEmpty();
        }
    }
}