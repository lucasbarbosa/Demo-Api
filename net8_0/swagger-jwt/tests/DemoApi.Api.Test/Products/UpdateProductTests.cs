using DemoApi.Api.Test.Configuration;
using DemoApi.Api.Test.Factories;
using DemoApi.Api.Test.Helpers;
using FluentAssertions;
using System.Net;

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
            var url = "/api/v1/products";
            var createdProduct = await GetLastCreatedProduct();
            
            createdProduct.Should().NotBeNull("Product creation should succeed");
            createdProduct.Id.Should().BeGreaterThan(0u, "Created product should have a valid ID");
            createdProduct.Name.Should().NotBeNullOrEmpty("Created product should have a name");
            
            var productToUpdate = ProductToUpdate(createdProduct);
            productToUpdate.Id.Should().Be(createdProduct.Id, "Update product ID should match created product ID");

            // Act
            var (result, _) = await HttpClientHelper.PutAndReturnResponseAsync(client, url, productToUpdate);

            // Assert
            result.StatusCode.Should().Be(HttpStatusCode.NoContent, 
                $"Update should succeed for product with ID {productToUpdate.Id}");
        }

        [Fact, TestPriority(301)]
        public async Task Update_ShouldReturnPreconditionFailed_WhenProductNameIsEmpty()
        {
            // Arrange
            HttpClient client = await GetAuthenticatedClient();
            var url = "/api/v1/products";
            var productFake = ProductWithEmptyName();

            // Act
            var (result, response) = await HttpClientHelper.PutAndReturnResponseAsync(client, url, productFake);

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
            var url = "/api/v1/products";
            var productFake = ProductWithNullName();

            // Act
            var (result, response) = await HttpClientHelper.PutAndReturnResponseAsync(client, url, productFake);

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
            var url = "/api/v1/products";
            var productFake = ProductWithZeroWeight();

            // Act
            var (result, response) = await HttpClientHelper.PutAndReturnResponseAsync(client, url, productFake);

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
            var url = "/api/v1/products";
            var productFake = ProductWithNegativeWeight();

            // Act
            var (result, response) = await HttpClientHelper.PutAndReturnResponseAsync(client, url, productFake);

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
            var url = "/api/v1/products";
            var productFake = NonExistentProduct();

            // Act
            var (result, response) = await HttpClientHelper.PutAndReturnResponseAsync(client, url, productFake);

            // Assert
            result.StatusCode.Should().Be(HttpStatusCode.NotFound);
            response?.Should().NotBeNull();
            response?.Success.Should().BeFalse();
        }

        #endregion
    }
}