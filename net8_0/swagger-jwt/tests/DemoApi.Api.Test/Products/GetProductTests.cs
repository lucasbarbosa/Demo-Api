using DemoApi.Api.Test.Configuration;
using DemoApi.Api.Test.Factories;
using DemoApi.Api.Test.Helpers;
using FluentAssertions;
using System.Net;

namespace DemoApi.Api.Test.Products
{
    [TestCaseOrderer("DemoApi.Api.Test.Configuration.PriorityOrderer", "DemoApi.Api.Test")]
    public class GetProductTests(CustomWebApplicationFactory factory) : ProductApiTests(factory)
    {
        #region Public Methods

        [Fact, TestPriority(200)]
        public async Task GetAll_ShouldReturnOk_WhenProductsExist()
        {
            // Arrange
            HttpClient client = await GetAuthenticatedClient();
            var url = "/api/v1/products";

            // Act
            var (result, response) = await HttpClientHelper.GetAndReturnResponseAsync(client, url);

            // Assert
            result.StatusCode.Should().Be(HttpStatusCode.OK);
            response?.Should().NotBeNull();
            response?.Success.Should().BeTrue();
            response?.Data.Should().NotBeNull();
        }

        [Fact, TestPriority(201)]
        public async Task GetById_ShouldReturnOk_WhenProductExists()
        {
            // Arrange
            HttpClient client = await GetAuthenticatedClient();
            var createdProduct = await GetLastCreatedProduct();
            var url = $"/api/v1/products/{createdProduct.Id}";

            // Act
            var (result, response) = await HttpClientHelper.GetAndReturnResponseAsync(client, url);

            // Assert
            result.StatusCode.Should().Be(HttpStatusCode.OK);
            response?.Should().NotBeNull();
            response?.Success.Should().BeTrue();
            response?.Data.Should().NotBeNull();
        }

        [Fact, TestPriority(202)]
        public async Task GetById_ShouldReturnNotFound_WhenProductDoesNotExist()
        {
            // Arrange
            HttpClient client = await GetAuthenticatedClient();
            var url = "/api/v1/products/999999";

            // Act
            var (result, response) = await HttpClientHelper.GetAndReturnResponseAsync(client, url);

            // Assert
            result.StatusCode.Should().Be(HttpStatusCode.NotFound);
            response?.Should().NotBeNull();
            response?.Success.Should().BeFalse();
        }

        [Fact, TestPriority(203)]
        public async Task GetById_ShouldReturnBadRequest_WhenIdIsNotNumeric()
        {
            // Arrange
            HttpClient client = await GetAuthenticatedClient();
            var url = "/api/v1/products/ABC";

            // Act
            var (result, response) = await HttpClientHelper.GetAndReturnResponseAsync(client, url);

            // Assert
            result.StatusCode.Should().Be(HttpStatusCode.BadRequest);
            response?.Should().NotBeNull();
            response?.Success.Should().BeFalse();
            response?.Errors.Should().NotBeEmpty();
            response?.Errors.Should().Contain(e => e.Contains("The value 'ABC' is not valid"));
        }

        [Fact, TestPriority(204)]
        public async Task GetById_ShouldReturnBadRequest_WhenIdIsNegative()
        {
            // Arrange
            HttpClient client = await GetAuthenticatedClient();
            var url = "/api/v1/products/-1";

            // Act
            var (result, response) = await HttpClientHelper.GetAndReturnResponseAsync(client, url);

            // Assert
            result.StatusCode.Should().Be(HttpStatusCode.BadRequest);
            response?.Should().NotBeNull();
            response?.Success.Should().BeFalse();
            response?.Errors.Should().NotBeEmpty();
        }

        [Fact, TestPriority(205)]
        public async Task GetById_ShouldReturnBadRequest_WhenIdIsDecimal()
        {
            // Arrange
            HttpClient client = await GetAuthenticatedClient();
            var url = "/api/v1/products/1.5";

            // Act
            var (result, response) = await HttpClientHelper.GetAndReturnResponseAsync(client, url);

            // Assert
            result.StatusCode.Should().Be(HttpStatusCode.BadRequest);
            response?.Should().NotBeNull();
            response?.Success.Should().BeFalse();
            response?.Errors.Should().NotBeEmpty();
        }

        [Fact, TestPriority(206)]
        public async Task GetById_ShouldReturnBadRequest_WhenIdContainsSpecialCharacters()
        {
            // Arrange
            HttpClient client = await GetAuthenticatedClient();
            var url = "/api/v1/products/@#$";

            // Act
            var (result, response) = await HttpClientHelper.GetAndReturnResponseAsync(client, url);

            // Assert
            result.StatusCode.Should().Be(HttpStatusCode.BadRequest);
            response?.Should().NotBeNull();
            response?.Success.Should().BeFalse();
            response?.Errors.Should().NotBeEmpty();
        }

        #endregion
    }
}