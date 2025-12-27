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
            // Arrange
            string url = "/api/v1/products";
            ProductViewModel productFake = await GetLastCreatedProduct();
            ProductViewModel productToUpdate = ProductToUpdate(productFake);

            // Act
            (HttpResponseMessage response, ResponseViewModel? viewModel) = await HttpClientHelper.PutAndReturnResponseAsync(_client, url, productToUpdate);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.NoContent);
        }

        [Fact, TestPriority(301)]
        public async Task Update_ShouldReturnPreconditionFailed_WhenProductNameIsEmpty()
        {
            // Arrange
            string url = "/api/v1/products";
            ProductViewModel productFake = ProductWithEmptyName();

            // Act
            (HttpResponseMessage response, ResponseViewModel? viewModel) = await HttpClientHelper.PutAndReturnResponseAsync(_client, url, productFake);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.PreconditionFailed);
            viewModel!.Should().NotBeNull();
            viewModel!.Success.Should().BeFalse();
            viewModel!.Errors.Should().Contain("Name is required");
        }

        [Fact, TestPriority(302)]
        public async Task Update_ShouldReturnPreconditionFailed_WhenProductNameIsNull()
        {
            // Arrange
            string url = "/api/v1/products";
            ProductViewModel productFake = ProductWithNullName();

            // Act
            (HttpResponseMessage response, ResponseViewModel? viewModel) = await HttpClientHelper.PutAndReturnResponseAsync(_client, url, productFake);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.PreconditionFailed);
            viewModel!.Should().NotBeNull();
            viewModel!.Success.Should().BeFalse();
            viewModel!.Errors.Should().Contain("Name is required");
        }

        [Fact, TestPriority(303)]
        public async Task Update_ShouldReturnPreconditionFailed_WhenProductWeightIsZero()
        {
            // Arrange
            string url = "/api/v1/products";
            ProductViewModel productFake = ProductWithZeroWeight();

            // Act
            (HttpResponseMessage response, ResponseViewModel? viewModel) = await HttpClientHelper.PutAndReturnResponseAsync(_client, url, productFake);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.PreconditionFailed);
            viewModel!.Should().NotBeNull();
            viewModel!.Success.Should().BeFalse();
            viewModel!.Errors.Should().Contain("Weight must be greater than 0");
        }

        [Fact, TestPriority(304)]
        public async Task Update_ShouldReturnPreconditionFailed_WhenProductWeightIsNegative()
        {
            // Arrange
            string url = "/api/v1/products";
            ProductViewModel productFake = ProductWithNegativeWeight();

            // Act
            (HttpResponseMessage response, ResponseViewModel? viewModel) = await HttpClientHelper.PutAndReturnResponseAsync(_client, url, productFake);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.PreconditionFailed);
            viewModel!.Should().NotBeNull();
            viewModel!.Success.Should().BeFalse();
            viewModel!.Errors.Should().Contain("Weight must be greater than 0");
        }

        [Fact, TestPriority(305)]
        public async Task Update_ShouldReturnNotFound_WhenProductDoesNotExist()
        {
            // Arrange
            string url = "/api/v1/products";
            ProductViewModel productFake = NonExistentProduct();

            // Act
            (HttpResponseMessage response, ResponseViewModel? viewModel) = await HttpClientHelper.PutAndReturnResponseAsync(_client, url, productFake);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
            viewModel?.Should().NotBeNull();
            viewModel?.Success.Should().BeFalse();
        }

        [Fact, TestPriority(306)]
        public async Task Update_ShouldReturnNotFound_WhenProductIdIsZero()
        {
            // Arrange
            string url = "/api/v1/products";
            ProductViewModel productFake = ProductWithIdZero();

            // Act
            (HttpResponseMessage response, ResponseViewModel? viewModel) = await HttpClientHelper.PutAndReturnResponseAsync(_client, url, productFake);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
            viewModel.Should().NotBeNull();
            viewModel!.Success.Should().BeFalse();
        }

        [Fact, TestPriority(307)]
        public async Task Update_ShouldAllowChangingWeightOnly()
        {
            // Arrange
            ProductViewModel product = await GetLastCreatedProduct();
            string url = "/api/v1/products";
            ProductViewModel updatedProduct = ProductToUpdateWeightOnly(product);

            // Act
            (HttpResponseMessage response, ResponseViewModel? viewModel) = await HttpClientHelper.PutAndReturnResponseAsync(_client, url, updatedProduct);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.NoContent);
        }

        [Fact, TestPriority(308)]
        public async Task Update_ShouldAllowChangingNameOnly()
        {
            // Arrange
            ProductViewModel product = await GetLastCreatedProduct();
            string url = "/api/v1/products";
            ProductViewModel updatedProduct = ProductToUpdateNameOnly(product);

            // Act
            (HttpResponseMessage response, ResponseViewModel? viewModel) = await HttpClientHelper.PutAndReturnResponseAsync(_client, url, updatedProduct);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.NoContent);
        }

        [Fact, TestPriority(309)]
        public async Task Update_ShouldHandleConcurrentUpdates()
        {
            // Arrange
            ProductViewModel product = await GetLastCreatedProduct();
            string updateUrl = "/api/v1/products";

            List<Task<HttpResponseMessage>> tasks = new List<Task<HttpResponseMessage>>();
            for (int i = 0; i < 3; i++)
            {
                ProductViewModel productToUpdate = ProductToUpdate(product);
                productToUpdate.Name = $"Concurrent Update {i}";
                tasks.Add(_client.PutAsJsonAsync(updateUrl, productToUpdate));
            }

            // Act
            HttpResponseMessage[] results = await Task.WhenAll(tasks);

            // Assert
            int successCount = results.Count(r => r.StatusCode == HttpStatusCode.NoContent);
            successCount.Should().BeGreaterThanOrEqualTo(1);
        }

        #endregion
    }
}