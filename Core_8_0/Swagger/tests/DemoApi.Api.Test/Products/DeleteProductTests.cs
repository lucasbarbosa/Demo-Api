using DemoApi.Api.Test.Configuration;
using DemoApi.Api.Test.Factories;
using DemoApi.Application.Models;
using FluentAssertions;
using System.Net;
using System.Net.Http.Json;

namespace DemoApi.Api.Test.Products
{
    [TestCaseOrderer("DemoApi.Api.Test.Configuration.PriorityOrderer", "DemoApi.Api.Test")]
    public class DeleteProductTests(CustomWebApplicationFactory factory) : ProductTests(factory)
    {
        #region Public Methods

        [Fact, TestPriority(400)]
        public async Task Delete_ShouldReturnNotFound_WhenProductDoesNotExist()
        {
            // Arrange
            var url = "/api/v1/products/999999";

            // Act
            var result = await _client.DeleteAsync(url);
            var response = await result.Content.ReadFromJsonAsync<ResponseViewModel>();

            // Assert
            result.StatusCode.Should().Be(HttpStatusCode.NotFound);
            response?.Should().NotBeNull();
            response?.Success.Should().BeFalse();
        }

        [Fact, TestPriority(401)]
        public async Task Delete_ShouldReturnBadRequest_WhenIdIsNotNumeric()
        {
            // Arrange
            var url = "/api/v1/products/XYZ";

            // Act
            var result = await _client.DeleteAsync(url);
            var response = await result.Content.ReadFromJsonAsync<ResponseViewModel>();

            // Assert
            result.StatusCode.Should().Be(HttpStatusCode.BadRequest);
            response?.Should().NotBeNull();
            response?.Success.Should().BeFalse();
            response?.Errors.Should().NotBeEmpty();
        }

        [Fact, TestPriority(402)]
        public async Task Delete_ShouldReturnNoContent_WhenProductExists()
        {
            // Arrange
            var url = "/api/v1/products/1";

            // Act
            var result = await _client.DeleteAsync(url);

            // Assert
            result.StatusCode.Should().Be(HttpStatusCode.NoContent);
        }

        #endregion
    }
}