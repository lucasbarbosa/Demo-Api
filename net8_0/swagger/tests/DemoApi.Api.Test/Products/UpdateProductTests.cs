using DemoApi.Api.Test.Configuration;
using DemoApi.Api.Test.Factories;
using DemoApi.Api.Test.Helpers;
using DemoApi.Application.Models;
using DemoApi.Application.Models.Products;
using FluentAssertions;
using System.Net;
using System.Net.Http.Json;

namespace DemoApi.Api.Test.Products
{
    [TestCaseOrderer("DemoApi.Api.Test.Configuration.PriorityOrderer", "DemoApi.Api.Test")]
    public class UpdateProductTests(CustomWebApplicationFactory factory) : ProductTests(factory)
    {
        #region Public Methods

        [Fact, TestPriority(300)]
        public async Task Update_ShouldReturnNoContent_WhenProductIsValid()
        {
            // Arrange - First create a product to update
            var url = "/api/v1/products";
            var newProduct = NewProduct();
            var createResult = await HttpClientHelper.PostAndEnsureSuccessAsync(_client, url, newProduct);
            var createResponse = await createResult.Content.ReadFromJsonAsync<ResponseViewModel>();

            createResponse.Should().NotBeNull();
            createResponse!.Data.Should().NotBeNull();

            var createdProduct = System.Text.Json.JsonSerializer.Deserialize<ProductViewModel>(
                createResponse.Data.ToString()!,
                new System.Text.Json.JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            createdProduct.Should().NotBeNull();

            // Update the created product
            var productToUpdate = ProductToUpdate(createdProduct);

            // Act
            var result = await _client.PutAsJsonAsync(url, productToUpdate);

            // Assert
            result.StatusCode.Should().Be(HttpStatusCode.NoContent);
        }

        [Fact, TestPriority(301)]
        public async Task Update_ShouldReturnPreconditionFailed_WhenProductNameIsEmpty()
        {
            // Arrange
            var url = "/api/v1/products";
            var productFake = ProductWithEmptyName();

            // Act
            var result = await _client.PutAsJsonAsync(url, productFake);
            var response = await result.Content.ReadFromJsonAsync<ResponseViewModel>();

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
            var url = "/api/v1/products";
            var productFake = ProductWithNullName();

            // Act
            var result = await _client.PutAsJsonAsync(url, productFake);
            var response = await result.Content.ReadFromJsonAsync<ResponseViewModel>();

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
            var url = "/api/v1/products";
            var productFake = ProductWithZeroWeight();

            // Act
            var result = await _client.PutAsJsonAsync(url, productFake);
            var response = await result.Content.ReadFromJsonAsync<ResponseViewModel>();

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
            var url = "/api/v1/products";
            var productFake = ProductWithNegativeWeight();

            // Act
            var result = await _client.PutAsJsonAsync(url, productFake);
            var response = await result.Content.ReadFromJsonAsync<ResponseViewModel>();

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
            var url = "/api/v1/products";
            var productFake = NonExistentProduct();

            // Act
            var result = await _client.PutAsJsonAsync(url, productFake);
            var response = await result.Content.ReadFromJsonAsync<ResponseViewModel>();

            // Assert
            result.StatusCode.Should().Be(HttpStatusCode.NotFound);
            response?.Should().NotBeNull();
            response?.Success.Should().BeFalse();
        }

        #endregion
    }
}