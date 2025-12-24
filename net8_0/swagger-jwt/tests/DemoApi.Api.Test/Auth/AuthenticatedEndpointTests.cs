using DemoApi.Api.Test.Configuration;
using DemoApi.Api.Test.Factories;
using DemoApi.Application.Models;
using FluentAssertions;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;

namespace DemoApi.Api.Test.Auth
{
    [TestCaseOrderer("DemoApi.Api.Test.Configuration.PriorityOrderer", "DemoApi.Api.Test")]
    public class AuthenticatedEndpointTests(CustomWebApplicationFactory factory) : AuthTests(factory)
    {
        #region Public Methods

        [Fact, TestPriority(600)]
        public async Task GetProducts_ShouldReturnUnauthorized_WhenNoTokenProvided()
        {
            // Arrange
            var url = "/api/v1/products";

            // Act
            var result = await _client.GetAsync(url);

            // Assert
            result.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        }

        [Fact, TestPriority(601)]
        public async Task GetProducts_ShouldReturnOk_WhenValidTokenProvided()
        {
            // Arrange
            var token = await GetValidToken();
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var url = "/api/v1/products";

            // Act
            var result = await _client.GetAsync(url);

            // Assert
            result.StatusCode.Should().Be(HttpStatusCode.OK);
        }

        [Fact, TestPriority(602)]
        public async Task GetProducts_ShouldReturnUnauthorized_WhenTokenIsInvalid()
        {
            // Arrange
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", "INVALID_TOKEN");

            var url = "/api/v1/products";

            // Act
            var result = await _client.GetAsync(url);

            // Assert
            result.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        }

        [Fact, TestPriority(603)]
        public async Task GetProducts_ShouldReturnUnauthorized_WhenTokenIsMalformed()
        {
            // Arrange
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.invalid.signature");

            var url = "/api/v1/products";

            // Act
            var result = await _client.GetAsync(url);

            // Assert
            result.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        }

        [Fact, TestPriority(604)]
        public async Task GetProducts_ShouldReturnUnauthorized_WhenTokenHasWrongScheme()
        {
            // Arrange
            var token = await GetValidToken();
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", token);

            var url = "/api/v1/products";

            // Act
            var result = await _client.GetAsync(url);

            // Assert
            result.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        }

        [Fact, TestPriority(605)]
        public async Task GetProducts_ShouldReturnUnauthorized_WhenAuthorizationHeaderIsMissing()
        {
            // Arrange
            var url = "/api/v1/products";

            // Act
            var result = await _client.GetAsync(url);

            // Assert
            result.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        }

        [Fact, TestPriority(606)]
        public async Task CreateProduct_ShouldReturnUnauthorized_WhenNoTokenProvided()
        {
            // Arrange
            var url = "/api/v1/products";
            var product = new { name = "Test Product", weight = 1.5 };

            // Act
            var result = await _client.PostAsJsonAsync(url, product);

            // Assert
            result.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        }

        [Fact, TestPriority(607)]
        public async Task UpdateProduct_ShouldReturnUnauthorized_WhenNoTokenProvided()
        {
            // Arrange
            var url = "/api/v1/products";
            var product = new { id = 1, name = "Updated Product", weight = 2.5 };

            // Act
            var result = await _client.PutAsJsonAsync(url, product);

            // Assert
            result.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        }

        [Fact, TestPriority(608)]
        public async Task DeleteProduct_ShouldReturnUnauthorized_WhenNoTokenProvided()
        {
            // Arrange
            var url = "/api/v1/products/1";

            // Act
            var result = await _client.DeleteAsync(url);

            // Assert
            result.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        }

        [Fact, TestPriority(609)]
        public async Task GetProductById_ShouldReturnUnauthorized_WhenNoTokenProvided()
        {
            // Arrange
            var url = "/api/v1/products/1";

            // Act
            var result = await _client.GetAsync(url);

            // Assert
            result.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        }

        [Fact, TestPriority(610)]
        public async Task MultipleRequests_ShouldWork_WithSameToken()
        {
            // Arrange
            var token = await GetValidToken();
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var url = "/api/v1/products";

            // Act
            var result1 = await _client.GetAsync(url);
            var result2 = await _client.GetAsync(url);
            var result3 = await _client.GetAsync(url);

            // Assert
            result1.StatusCode.Should().Be(HttpStatusCode.OK);
            result2.StatusCode.Should().Be(HttpStatusCode.OK);
            result3.StatusCode.Should().Be(HttpStatusCode.OK);
        }

        #endregion

        #region Private Methods

        private async Task<string> GetValidToken()
        {
            var tokenClient = _factory.CreateClient();
            tokenClient.DefaultRequestHeaders.Add("X-Security-Key", ValidSecurityKey);

            var result = await tokenClient.PostAsync("/api/v1/auth/token", null);
            var response = await result.Content.ReadFromJsonAsync<ResponseViewModel>();

            var tokenJson = JsonSerializer.Deserialize<JsonElement>(
                response!.Data!.ToString()!);

            return tokenJson.GetProperty("accessToken").GetString()!;
        }

        #endregion
    }
}