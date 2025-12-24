using DemoApi.Api.Test.Configuration;
using DemoApi.Api.Test.Factories;
using DemoApi.Api.Test.Helpers;
using FluentAssertions;
using System.Net;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.JwtBearer;

namespace DemoApi.Api.Test.Products
{
    [TestCaseOrderer("DemoApi.Api.Test.Configuration.PriorityOrderer", "DemoApi.Api.Test")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class CreateProductTests(CustomWebApplicationFactory factory) : ProductApiTests(factory)
    {
        #region Public Methods

        [Fact, TestPriority(100)]
        public async Task Create_ShouldReturnCreated_WithValidResponseObject_WhenProductIsValid()
        {
            // Arrange
            HttpClient client = await GetAuthenticatedClient();
            var url = "/api/v1/products";
            var productFake = NewProductWithRandomId();

            // Act
            var (result, response) = await HttpClientHelper.PostAndReturnResponseAsync(client, url, productFake);

            // Assert
            result.StatusCode.Should().Be(HttpStatusCode.Created);
            response!.Should().NotBeNull();
            response!.Success.Should().BeTrue();
            response!.Data.Should().NotBeNull();
        }

        [Fact, TestPriority(101)]
        public async Task Create_ShouldReturnPreconditionFailed_WithValidErrorResponse_WhenProductNameIsEmpty()
        {
            // Arrange
            HttpClient client = await GetAuthenticatedClient();
            var url = "/api/v1/products";
            var productFake = ProductWithEmptyName();

            // Act
            var (result, response) = await HttpClientHelper.PostAndReturnResponseAsync(client, url, productFake);

            // Assert
            result.StatusCode.Should().Be(HttpStatusCode.PreconditionFailed);
            response.Should().NotBeNull();
            response!.Success.Should().BeFalse();
            response!.Errors.Should().Contain("Name is required");
        }

        [Fact, TestPriority(102)]
        public async Task Create_ShouldReturnPreconditionFailed_WithValidErrorResponse_WhenProductNameIsNull()
        {
            // Arrange
            HttpClient client = await GetAuthenticatedClient();
            var url = "/api/v1/products";
            var productFake = ProductWithNullName();

            // Act
            var (result, response) = await HttpClientHelper.PostAndReturnResponseAsync(client, url, productFake);

            // Assert
            result.StatusCode.Should().Be(HttpStatusCode.PreconditionFailed);
            response.Should().NotBeNull();
            response!.Success.Should().BeFalse();
            response!.Errors.Should().Contain("Name is required");
        }

        [Fact, TestPriority(103)]
        public async Task Create_ShouldReturnPreconditionFailed_WithValidErrorResponse_WhenProductWeightIsZero()
        {
            // Arrange
            HttpClient client = await GetAuthenticatedClient();
            var url = "/api/v1/products";
            var productFake = ProductWithZeroWeight();

            // Act
            var (result, response) = await HttpClientHelper.PostAndReturnResponseAsync(client, url, productFake);

            // Assert
            result.StatusCode.Should().Be(HttpStatusCode.PreconditionFailed);
            response.Should().NotBeNull();
            response!.Success.Should().BeFalse();
            response!.Errors.Should().Contain("Weight must be greater than 0");
        }

        [Fact, TestPriority(104)]
        public async Task Create_ShouldReturnPreconditionFailed_WhenProductWeightIsNegative()
        {
            // Arrange
            HttpClient client = await GetAuthenticatedClient();
            var url = "/api/v1/products";
            var productFake = ProductWithNegativeWeight();

            // Act
            var (result, response) = await HttpClientHelper.PostAndReturnResponseAsync(client, url, productFake);

            // Assert
            result.StatusCode.Should().Be(HttpStatusCode.PreconditionFailed);
            response!.Should().NotBeNull();
            response!.Success.Should().BeFalse();
            response!.Errors.Should().Contain("Weight must be greater than 0");
        }

        #endregion
    }
}