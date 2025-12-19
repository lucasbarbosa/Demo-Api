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
    public class CreateProductTests(CustomWebApplicationFactory factory) : ProductTests(factory)
    {
        #region Public Methods

        [Fact, TestPriority(100)]
        public async Task Create_ShouldReturnCreated_WhenProductIsValid()
        {
            // Arrange
            var url = "/api/v1/products";
            var productFake = NewProduct();

            // Act
            var result = await HttpClientHelper.PostAndEnsureSuccessAsync(_client, url, productFake);
            var response = await result.Content.ReadFromJsonAsync<ResponseViewModel>();

            // Assert
            result.StatusCode.Should().Be(HttpStatusCode.Created);
            response!.Should().NotBeNull();
            response!.Success.Should().BeTrue();
            response!.Data.Should().NotBeNull();
        }

        [Fact, TestPriority(101)]
        public async Task Create_ShouldReturnPreconditionFailed_WhenProductNameIsEmpty()
        {
            // Arrange
            var url = "/api/v1/products";
            var productFake = ProductWithEmptyName();

            // Act
            var result = await _client.PostAsJsonAsync(url, productFake);
            var response = await result.Content.ReadFromJsonAsync<ResponseViewModel>();

            // Assert
            result.StatusCode.Should().Be(HttpStatusCode.PreconditionFailed);
            response!.Should().NotBeNull();
            response!.Success.Should().BeFalse();
            response!.Errors.Should().Contain("Name is required");
        }

        [Fact, TestPriority(102)]
        public async Task Create_ShouldReturnPreconditionFailed_WhenProductNameIsNull()
        {
            // Arrange
            var url = "/api/v1/products";
            var productFake = ProductWithNullName();

            // Act
            var result = await _client.PostAsJsonAsync(url, productFake);
            var response = await result.Content.ReadFromJsonAsync<ResponseViewModel>();

            // Assert
            result.StatusCode.Should().Be(HttpStatusCode.PreconditionFailed);
            response!.Should().NotBeNull();
            response!.Success.Should().BeFalse();
            response!.Errors.Should().Contain("Name is required");
        }

        [Fact, TestPriority(103)]
        public async Task Create_ShouldReturnPreconditionFailed_WhenProductWeightIsZero()
        {
            // Arrange
            var url = "/api/v1/products";
            var productFake = ProductWithZeroWeight();

            // Act
            var result = await _client.PostAsJsonAsync(url, productFake);
            var response = await result.Content.ReadFromJsonAsync<ResponseViewModel>();

            // Assert
            result.StatusCode.Should().Be(HttpStatusCode.PreconditionFailed);
            response!.Should().NotBeNull();
            response!.Success.Should().BeFalse();
            response!.Errors.Should().Contain("Weight must be greater than 0");
        }

        [Fact, TestPriority(104)]
        public async Task Create_ShouldReturnPreconditionFailed_WhenProductWeightIsNegative()
        {
            // Arrange
            var url = "/api/v1/products";
            var productFake = ProductWithNegativeWeight();

            // Act
            var result = await _client.PostAsJsonAsync(url, productFake);
            var response = await result.Content.ReadFromJsonAsync<ResponseViewModel>();

            // Assert
            result.StatusCode.Should().Be(HttpStatusCode.PreconditionFailed);
            response!.Should().NotBeNull();
            response!.Success.Should().BeFalse();
            response!.Errors.Should().Contain("Weight must be greater than 0");
        }

        #endregion
    }
}