using DemoApi.Api.Test.Configuration;
using DemoApi.Api.Test.Factories;
using DemoApi.Api.Test.Helpers;
using DemoApi.Application.Models;
using FluentAssertions;
using System.Net;
using System.Net.Http.Json;

namespace DemoApi.Api.Test.Products
{
    public class ProductApiTests(CustomWebApplicationFactory factory) : IClassFixture<CustomWebApplicationFactory>
    {
        #region Properties

        private readonly HttpClient _client = factory.CreateClient();

        #endregion

        #region Public Methods

        [Fact, TestPriority(1)]
        public async Task Create_ShouldReturnProduct_WhenRepositoryCreatesSuccessfully()
        {
            // Arrange
            var url = "/api/v1/products";
            var productFake = NewProduct();

            // Act
            var result = await HttpClientHelper.PostAndEnsureSuccessAsync(_client, url, productFake);
            var response = await result.Content.ReadFromJsonAsync<ResponseViewModel>();

            // Assert
            result.StatusCode.Should().Be(HttpStatusCode.Created);
            response.Should().NotBeNull();
            response.Success.Should().BeTrue();
            response.Data.Should().NotBeNull();
        }

        [Fact, TestPriority(2)]
        public async Task GetProducts_WhenRequestIsValid_ShouldReturnProductList()
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

        #endregion

        #region Private Methods

        private static ProductViewModel NewProduct()
        {
            return new ProductViewModel
            {
                Id = 0,
                Name = "Product Name 1",
                Weight = 3.12
            };
        }

        #endregion
    }
}