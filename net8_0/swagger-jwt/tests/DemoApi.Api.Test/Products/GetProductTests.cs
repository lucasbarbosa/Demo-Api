using DemoApi.Api.Test.Configuration;
using DemoApi.Api.Test.Factories;
using DemoApi.Api.Test.Helpers;
using DemoApi.Application.Models;
using FluentAssertions;
using System.Net;
using System.Net.Http.Json;

namespace DemoApi.Api.Test.Products
{
    [TestCaseOrderer("DemoApi.Api.Test.Configuration.PriorityOrderer", "DemoApi.Api.Test")]
    public class GetProductTests(CustomWebApplicationFactory factory) : ProductTests(factory)
    {
        #region Public Methods

        [Fact, TestPriority(200)]
        public async Task GetAll_ShouldReturnOk_WhenProductsExist()
        {
            // Arrange
            var url = "/api/v1/products";

            // Act
            var result = await HttpClientHelper.GetAndEnsureSuccessAsync(_client, url);
            var response = await result.Content.ReadFromJsonAsync<ResponseViewModel>();

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
            var url = "/api/v1/products/1";

            // Act
            var result = await HttpClientHelper.GetAndEnsureSuccessAsync(_client, url);
            var response = await result.Content.ReadFromJsonAsync<ResponseViewModel>();

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
            var url = "/api/v1/products/999999";

            // Act
            var result = await _client.GetAsync(url);
            var response = await result.Content.ReadFromJsonAsync<ResponseViewModel>();

            // Assert
            result.StatusCode.Should().Be(HttpStatusCode.NotFound);
            response?.Should().NotBeNull();
            response?.Success.Should().BeFalse();
        }

        [Fact, TestPriority(203)]
        public async Task GetById_ShouldReturnBadRequest_WhenIdIsNotNumeric()
        {
            // Arrange
            var url = "/api/v1/products/ABC";

            // Act
            var result = await _client.GetAsync(url);
            var response = await result.Content.ReadFromJsonAsync<ResponseViewModel>();

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
            var url = "/api/v1/products/-1";

            // Act
            var result = await _client.GetAsync(url);
            var response = await result.Content.ReadFromJsonAsync<ResponseViewModel>();

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
            var url = "/api/v1/products/1.5";

            // Act
            var result = await _client.GetAsync(url);
            var response = await result.Content.ReadFromJsonAsync<ResponseViewModel>();

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
            var url = "/api/v1/products/@#$";

            // Act
            var result = await _client.GetAsync(url);
            var response = await result.Content.ReadFromJsonAsync<ResponseViewModel>();

            // Assert
            result.StatusCode.Should().Be(HttpStatusCode.BadRequest);
            response?.Should().NotBeNull();
            response?.Success.Should().BeFalse();
            response?.Errors.Should().NotBeEmpty();
        }

        #endregion
    }
}