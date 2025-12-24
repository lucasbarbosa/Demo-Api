using DemoApi.Api.Test.Configuration;
using DemoApi.Api.Test.Factories;
using DemoApi.Api.Test.Helpers;
using DemoApi.Application.Models;
using DemoApi.Application.Models.Products;
using FluentAssertions;
using System.Net;

namespace DemoApi.Api.Test.Products
{
    [TestCaseOrderer("DemoApi.Api.Test.Configuration.PriorityOrderer", "DemoApi.Api.Test")]
    public class CreateProductTests(CustomWebApplicationFactory factory) : ProductApiTests(factory)
    {
        #region Public Methods

        [Fact, TestPriority(100)]
        public async Task Create_ShouldReturnCreated_WhenProductIsValid()
        {
            // Arrange
            HttpClient client = await GetAuthenticatedClient();
            string url = "/api/v1/products";
            ProductViewModel productFake = NewProductWithRandomId();

            // Act
            (HttpResponseMessage result, ResponseViewModel? response) = await HttpClientHelper.PostAndReturnResponseAsync(client, url, productFake);

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
            HttpClient client = await GetAuthenticatedClient();
            string url = "/api/v1/products";
            ProductViewModel productFake = ProductWithEmptyName();

            // Act
            (HttpResponseMessage result, ResponseViewModel? response) = await HttpClientHelper.PostAndReturnResponseAsync(client, url, productFake);

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
            HttpClient client = await GetAuthenticatedClient();
            string url = "/api/v1/products";
            ProductViewModel productFake = ProductWithNullName();

            // Act
            (HttpResponseMessage result, ResponseViewModel? response) = await HttpClientHelper.PostAndReturnResponseAsync(client, url, productFake);

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
            HttpClient client = await GetAuthenticatedClient();
            string url = "/api/v1/products";
            ProductViewModel productFake = ProductWithZeroWeight();

            // Act
            (HttpResponseMessage result, ResponseViewModel? response) = await HttpClientHelper.PostAndReturnResponseAsync(client, url, productFake);

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
            HttpClient client = await GetAuthenticatedClient();
            string url = "/api/v1/products";
            ProductViewModel productFake = ProductWithNegativeWeight();

            // Act
            (HttpResponseMessage result, ResponseViewModel? response) = await HttpClientHelper.PostAndReturnResponseAsync(client, url, productFake);

            // Assert
            result.StatusCode.Should().Be(HttpStatusCode.PreconditionFailed);
            response!.Should().NotBeNull();
            response!.Success.Should().BeFalse();
            response!.Errors.Should().Contain("Weight must be greater than 0");
        }

        #endregion
    }
}