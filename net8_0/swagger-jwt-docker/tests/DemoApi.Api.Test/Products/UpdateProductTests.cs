using DemoApi.Api.Test.Configuration;
using DemoApi.Api.Test.Factories;
using DemoApi.Api.Test.Helpers;
using DemoApi.Application.Models;
using DemoApi.Application.Models.Products;
using FluentAssertions;
using System.Net;
using System.Net.Http.Json;
using System.Text.Json;

namespace DemoApi.Api.Test.Products
{
    [TestCaseOrderer("DemoApi.Api.Test.Configuration.PriorityOrderer", "DemoApi.Api.Test")]
    public class UpdateProductTests(CustomWebApplicationFactory factory) : ProductApiTests(factory)
    {
        #region Public Methods

        [Fact, TestPriority(300)]
        public async Task Update_ShouldReturnNoContent_WhenProductIsValid()
        {
            // Arrange
            HttpClient client = await GetAuthenticatedClient();
            string url = "/api/v1/products";
            ProductViewModel createdProduct = await GetLastCreatedProduct();

            createdProduct.Should().NotBeNull("Product creation should succeed");
            createdProduct.Id.Should().BeGreaterThan(0u, "Created product should have a valid ID");
            createdProduct.Name.Should().NotBeNullOrEmpty("Created product should have a name");

            ProductViewModel productToUpdate = ProductToUpdate(createdProduct!);
            productToUpdate.Id.Should().Be(createdProduct.Id, "Update product ID should match created product ID");

            // Act
            HttpResponseMessage result = await client.PutAsJsonAsync(url, productToUpdate);

            // Assert
            result.StatusCode.Should().Be(HttpStatusCode.NoContent);
        }

        [Fact, TestPriority(301)]
        public async Task Update_ShouldReturnPreconditionFailed_WhenProductNameIsEmpty()
        {
            // Arrange
            HttpClient client = await GetAuthenticatedClient();
            string url = "/api/v1/products";
            ProductViewModel productFake = ProductWithEmptyName();

            // Act
            (HttpResponseMessage result, ResponseViewModel? response) = await HttpClientHelper.PutAndReturnResponseAsync(client, url, productFake);

            // Assert
            result.StatusCode.Should().Be(HttpStatusCode.PreconditionFailed);
            response!.Should().NotBeNull();
            response!.Success.Should().BeFalse();
            response!.Errors.Should().Contain("Name is required");
        }

        [Fact, TestPriority(302)]
        public async Task Update_ShouldReturnPreconditionFailed_WhenProductNameIsNull()
        {
            // Arrange
            HttpClient client = await GetAuthenticatedClient();
            string url = "/api/v1/products";
            ProductViewModel productFake = ProductWithNullName();

            // Act
            (HttpResponseMessage result, ResponseViewModel? response) = await HttpClientHelper.PutAndReturnResponseAsync(client, url, productFake);

            // Assert
            result.StatusCode.Should().Be(HttpStatusCode.PreconditionFailed);
            response!.Should().NotBeNull();
            response!.Success.Should().BeFalse();
            response!.Errors.Should().Contain("Name is required");
        }

        [Fact, TestPriority(303)]
        public async Task Update_ShouldReturnPreconditionFailed_WhenProductWeightIsZero()
        {
            // Arrange
            HttpClient client = await GetAuthenticatedClient();
            string url = "/api/v1/products";
            ProductViewModel productFake = ProductWithZeroWeight();

            // Act
            (HttpResponseMessage result, ResponseViewModel? response) = await HttpClientHelper.PutAndReturnResponseAsync(client, url, productFake);

            // Assert
            result.StatusCode.Should().Be(HttpStatusCode.PreconditionFailed);
            response!.Should().NotBeNull();
            response!.Success.Should().BeFalse();
            response!.Errors.Should().Contain("Weight must be greater than 0");
        }

        [Fact, TestPriority(304)]
        public async Task Update_ShouldReturnPreconditionFailed_WhenProductWeightIsNegative()
        {
            // Arrange
            HttpClient client = await GetAuthenticatedClient();
            string url = "/api/v1/products";
            ProductViewModel productFake = ProductWithNegativeWeight();

            // Act
            (HttpResponseMessage result, ResponseViewModel? response) = await HttpClientHelper.PutAndReturnResponseAsync(client, url, productFake);

            // Assert
            result.StatusCode.Should().Be(HttpStatusCode.PreconditionFailed);
            response!.Should().NotBeNull();
            response!.Success.Should().BeFalse();
            response!.Errors.Should().Contain("Weight must be greater than 0");
        }

        [Fact, TestPriority(305)]
        public async Task Update_ShouldReturnNotFound_WhenProductDoesNotExist()
        {
            // Arrange
            HttpClient client = await GetAuthenticatedClient();
            string url = "/api/v1/products";
            ProductViewModel productFake = NonExistentProduct();

            // Act
            (HttpResponseMessage result, ResponseViewModel? response) = await HttpClientHelper.PutAndReturnResponseAsync(client, url, productFake);

            // Assert
            result.StatusCode.Should().Be(HttpStatusCode.NotFound);
            response?.Should().NotBeNull();
            response?.Success.Should().BeFalse();
        }

        #endregion
    }
}