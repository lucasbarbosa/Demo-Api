using DemoApi.Api.Test.Configuration;
using DemoApi.Api.Test.Factories;
using DemoApi.Application.Models;
using FluentAssertions;
using System.Net;
using System.Net.Http.Json;

namespace DemoApi.Api.Test.Auth
{
    [TestCaseOrderer("DemoApi.Api.Test.Configuration.PriorityOrderer", "DemoApi.Api.Test")]
    public class GenerateTokenTests(CustomWebApplicationFactory factory) : AuthTests(factory)
    {
        #region Public Methods

        [Fact, TestPriority(500)]
        public async Task GenerateToken_ShouldReturnOk_WhenSecurityKeyIsValid()
        {
            // Arrange
            var url = "/api/v1/auth/token";
            _client.DefaultRequestHeaders.Add("X-Security-Key", ValidSecurityKey);

            // Act
            var result = await _client.PostAsync(url, null);
            var response = await result.Content.ReadFromJsonAsync<ResponseViewModel>();

            // Assert
            result.StatusCode.Should().Be(HttpStatusCode.OK);
            response.Should().NotBeNull();
            response!.Success.Should().BeTrue();
            response.Data.Should().NotBeNull();
        }

        [Fact, TestPriority(501)]
        public async Task GenerateToken_ShouldReturnTokenViewModel_WithAllRequiredFields()
        {
            // Arrange
            var url = "/api/v1/auth/token";
            _client.DefaultRequestHeaders.Add("X-Security-Key", ValidSecurityKey);

            // Act
            var result = await _client.PostAsync(url, null);
            var response = await result.Content.ReadFromJsonAsync<ResponseViewModel>();

            // Assert
            response.Should().NotBeNull();
            response!.Data.Should().NotBeNull();
            
            var tokenData = response.Data.ToString();
            tokenData.Should().Contain("accessToken");
            tokenData.Should().Contain("tokenType");
            tokenData.Should().Contain("expiresIn");
            tokenData.Should().Contain("created");
            tokenData.Should().Contain("expires");
        }

        [Fact, TestPriority(502)]
        public async Task GenerateToken_ShouldReturnUnauthorized_WhenSecurityKeyIsInvalid()
        {
            // Arrange
            var url = "/api/v1/auth/token";
            _client.DefaultRequestHeaders.Add("X-Security-Key", "INVALID_KEY");

            // Act
            var result = await _client.PostAsync(url, null);
            var response = await result.Content.ReadFromJsonAsync<ResponseViewModel>();

            // Assert
            result.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
            response.Should().NotBeNull();
            response!.Success.Should().BeFalse();
            response.Errors.Should().Contain("Invalid security key. Authentication failed.");
        }

        [Fact, TestPriority(503)]
        public async Task GenerateToken_ShouldReturnPreconditionFailed_WhenSecurityKeyIsEmpty()
        {
            // Arrange
            var url = "/api/v1/auth/token";
            _client.DefaultRequestHeaders.Add("X-Security-Key", string.Empty);

            // Act
            var result = await _client.PostAsync(url, null);
            var response = await result.Content.ReadFromJsonAsync<ResponseViewModel>();

            // Assert
            result.StatusCode.Should().Be(HttpStatusCode.PreconditionFailed);
            response.Should().NotBeNull();
            response!.Success.Should().BeFalse();
        }

        [Fact, TestPriority(504)]
        public async Task GenerateToken_ShouldReturnPreconditionFailed_WhenSecurityKeyIsWhitespace()
        {
            // Arrange
            var url = "/api/v1/auth/token";
            _client.DefaultRequestHeaders.Add("X-Security-Key", "   ");

            // Act
            var result = await _client.PostAsync(url, null);
            var response = await result.Content.ReadFromJsonAsync<ResponseViewModel>();

            // Assert
            result.StatusCode.Should().Be(HttpStatusCode.PreconditionFailed);
            response.Should().NotBeNull();
            response!.Success.Should().BeFalse();
        }

        [Fact, TestPriority(505)]
        public async Task GenerateToken_ShouldReturnPreconditionFailed_WhenSecurityKeyHeaderIsMissing()
        {
            // Arrange
            var url = "/api/v1/auth/token";

            // Act
            var result = await _client.PostAsync(url, null);
            var response = await result.Content.ReadFromJsonAsync<ResponseViewModel>();

            // Assert
            result.StatusCode.Should().Be(HttpStatusCode.PreconditionFailed);
            response.Should().NotBeNull();
            response!.Success.Should().BeFalse();
        }

        [Fact, TestPriority(506)]
        public async Task GenerateToken_ShouldReturnDifferentTokens_WhenCalledMultipleTimes()
        {
            // Arrange
            var url = "/api/v1/auth/token";
            _client.DefaultRequestHeaders.Add("X-Security-Key", ValidSecurityKey);

            // Act
            var result1 = await _client.PostAsync(url, null);
            var response1 = await result1.Content.ReadFromJsonAsync<ResponseViewModel>();

            await Task.Delay(1000);

            var result2 = await _client.PostAsync(url, null);
            var response2 = await result2.Content.ReadFromJsonAsync<ResponseViewModel>();

            // Assert
            var token1 = response1!.Data!.ToString();
            var token2 = response2!.Data!.ToString();

            token1.Should().NotBe(token2);
        }

        [Fact, TestPriority(507)]
        public async Task GenerateToken_ShouldReturnUnauthorized_WhenSecurityKeyHasExtraCharacters()
        {
            // Arrange
            var url = "/api/v1/auth/token";
            _client.DefaultRequestHeaders.Add("X-Security-Key", ValidSecurityKey + "EXTRA");

            // Act
            var result = await _client.PostAsync(url, null);
            var response = await result.Content.ReadFromJsonAsync<ResponseViewModel>();

            // Assert
            result.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
            response.Should().NotBeNull();
            response!.Success.Should().BeFalse();
            response.Errors.Should().Contain("Invalid security key. Authentication failed.");
        }

        [Fact, TestPriority(508)]
        public async Task GenerateToken_ShouldReturnUnauthorized_WhenSecurityKeyIsTruncated()
        {
            // Arrange
            var url = "/api/v1/auth/token";
            var truncatedKey = ValidSecurityKey[..^5];
            _client.DefaultRequestHeaders.Add("X-Security-Key", truncatedKey);

            // Act
            var result = await _client.PostAsync(url, null);
            var response = await result.Content.ReadFromJsonAsync<ResponseViewModel>();

            // Assert
            result.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
            response.Should().NotBeNull();
            response!.Success.Should().BeFalse();
            response.Errors.Should().Contain("Invalid security key. Authentication failed.");
        }

        [Fact, TestPriority(509)]
        public async Task GenerateToken_ShouldBeCaseSensitive_ForSecurityKey()
        {
            // Arrange
            var url = "/api/v1/auth/token";
            _client.DefaultRequestHeaders.Add("X-Security-Key", ValidSecurityKey.ToLower());

            // Act
            var result = await _client.PostAsync(url, null);
            var response = await result.Content.ReadFromJsonAsync<ResponseViewModel>();

            // Assert
            result.StatusCode.Should().Be(HttpStatusCode.OK);
            response.Should().NotBeNull();
            response!.Success.Should().BeFalse();
        }

        [Fact, TestPriority(510)]
        public async Task GenerateToken_ShouldReturnToken_WithBearerTokenType()
        {
            // Arrange
            var url = "/api/v1/auth/token";
            _client.DefaultRequestHeaders.Add("X-Security-Key", ValidSecurityKey);

            // Act
            var result = await _client.PostAsync(url, null);
            var response = await result.Content.ReadFromJsonAsync<ResponseViewModel>();

            // Assert
            response.Should().NotBeNull();
            var tokenData = response!.Data!.ToString();
            tokenData.Should().Contain("Bearer");
        }

        #endregion
    }
}