using DemoApi.Api.Test.Configuration;
using DemoApi.Api.Test.Factories;
using DemoApi.Application.Models;
using FluentAssertions;
using System.Net;
using System.Net.Http.Json;

namespace DemoApi.Api.Test.Products
{
    [TestCaseOrderer("DemoApi.Api.Test.Configuration.PriorityOrderer", "DemoApi.Api.Test")]
    public class DeleteProductTests(CustomWebApplicationFactory factory) : ProductApiTests(factory)
    {
        #region Public Methods

        [Fact, TestPriority(400)]
        public async Task Delete_ShouldReturnNotFound_WhenProductDoesNotExist()
        {
            // Arrange
            HttpClient client = await GetAuthenticatedClient();
            string url = "/api/v1/products/999999";

            // Act
            HttpResponseMessage result = await client.DeleteAsync(url);
            ResponseViewModel? response = await result.Content.ReadFromJsonAsync<ResponseViewModel>();

            // Assert
            result.StatusCode.Should().Be(HttpStatusCode.NotFound);
            response?.Should().NotBeNull();
            response?.Success.Should().BeFalse();
        }

        [Fact, TestPriority(401)]
        public async Task Delete_ShouldReturnBadRequest_WhenIdIsNotNumeric()
        {
            // Arrange
            HttpClient client = await GetAuthenticatedClient();
            string url = "/api/v1/products/XYZ";

            // Act
            HttpResponseMessage result = await client.DeleteAsync(url);
            ResponseViewModel? response = await result.Content.ReadFromJsonAsync<ResponseViewModel>();

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
            HttpClient client = await GetAuthenticatedClient();
            string url = "/api/v1/products/1";

            // Act
            HttpResponseMessage result = await client.DeleteAsync(url);

            // Assert
            result.StatusCode.Should().Be(HttpStatusCode.NoContent);
        }

        #endregion
    }
}