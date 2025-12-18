using Bogus;
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
    public class ProductApiTests(CustomWebApplicationFactory factory) : IClassFixture<CustomWebApplicationFactory>
    {
        #region Properties

        private readonly HttpClient _client = factory.CreateClient();

        #endregion

        #region Public Methods

        [Fact, TestPriority(1)]
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

        [Fact, TestPriority(2)]
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

        [Fact, TestPriority(3)]
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

        [Fact, TestPriority(4)]
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

        [Fact, TestPriority(5)]
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

        [Fact, TestPriority(6)]
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

        [Fact, TestPriority(7)]
        public async Task Update_ShouldReturnNoContent_WhenProductIsValid()
        {
            // Arrange
            var url = "/api/v1/products";
            var productFake = UpdateProduct();

            // Act
            var result = await _client.PutAsJsonAsync(url, productFake);

            // Assert
            result.StatusCode.Should().Be(HttpStatusCode.NoContent);
        }

        [Fact, TestPriority(8)]
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

        [Fact, TestPriority(9)]
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

        [Fact, TestPriority(10)]
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

        [Fact, TestPriority(11)]
        public async Task Delete_ShouldReturnNoContent_WhenProductExists()
        {
            // Arrange
            var url = "/api/v1/products/1";

            // Act
            var result = await _client.DeleteAsync(url);

            // Assert
            result.StatusCode.Should().Be(HttpStatusCode.NoContent);
        }

        [Fact, TestPriority(12)]
        public async Task Delete_ShouldReturnNotFound_WhenProductDoesNotExist()
        {
            // Arrange
            var url = "/api/v1/products/999999";

            // Act
            var result = await _client.DeleteAsync(url);
            var response = await result.Content.ReadFromJsonAsync<ResponseViewModel>();

            // Assert
            result.StatusCode.Should().Be(HttpStatusCode.NotFound);
            response?.Should().NotBeNull();
            response?.Success.Should().BeFalse();
        }

        #endregion

        #region Private Methods

        private static ProductViewModel NewProduct()
        {
            var faker = new Faker<ProductViewModel>()
                .RuleFor(p => p.Id, f => 0u)
                .RuleFor(p => p.Name, f => f.Commerce.ProductName())
                .RuleFor(p => p.Weight, f => Math.Round(f.Random.Double(0.1, 10.0), 2));

            return faker.Generate();
        }

        private static ProductViewModel UpdateProduct()
        {
            var faker = new Faker<ProductViewModel>()
                .RuleFor(p => p.Id, f => 1u)
                .RuleFor(p => p.Name, f => f.Commerce.ProductName())
                .RuleFor(p => p.Weight, f => Math.Round(f.Random.Double(0.1, 10.0), 2));

            return faker.Generate();
        }
        
        private static ProductViewModel NonExistentProduct()
        {
            return new ProductViewModel
            {
                Id = 999999,
                Name = "Non Existent Product",
                Weight = 1.0
            };
        }

        private static ProductViewModel ProductWithEmptyName()
        {
            return new ProductViewModel
            {
                Id = 0,
                Name = string.Empty,
                Weight = 2.5
            };
        }

        private static ProductViewModel ProductWithNullName()
        {
            return new ProductViewModel
            {
                Id = 0,
                Name = null!,
                Weight = 2.5
            };
        }

        #endregion
    }
}