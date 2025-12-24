using DemoApi.Api.Test.Configuration;
using DemoApi.Api.Test.Factories;
using DemoApi.Api.Test.Helpers;
using FluentAssertions;
using System.Net;

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
            var url = "/api/v1/products/999999";

            // Act
            var (result, response) = await HttpClientHelper.DeleteAndReturnResponseAsync(client, url);

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
            var url = "/api/v1/products/XYZ";

            // Act
            var (result, response) = await HttpClientHelper.DeleteAndReturnResponseAsync(client, url);

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
            var product = await GetLastCreatedProduct();
            var url = $"/api/v1/products/{product.Id}";

            // Act
            var (result, _) = await HttpClientHelper.DeleteAndReturnResponseAsync(client, url);

            // Assert
            result.StatusCode.Should().Be(HttpStatusCode.NoContent);
        }

        #endregion
    }
}