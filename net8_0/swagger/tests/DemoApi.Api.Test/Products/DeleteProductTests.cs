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
    public class DeleteProductTests(CustomWebApplicationFactory factory) : ProductTests(factory)
    {
        #region Public Methods

        [Fact, TestPriority(400)]
        public async Task Delete_ShouldReturnNotFound_WhenProductDoesNotExist()
        {
            // Arrange
            string url = "/api/v1/products/999999";

            // Act
            (HttpResponseMessage response, ResponseViewModel? viewModel) = await HttpClientHelper.DeleteAndReturnResponseAsync(_client, url);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
            viewModel?.Should().NotBeNull();
            viewModel?.Success.Should().BeFalse();
        }

        [Fact, TestPriority(401)]
        public async Task Delete_ShouldReturnBadRequest_WhenIdIsNotNumeric()
        {
            // Arrange
            string url = "/api/v1/products/XYZ";

            // Act
            (HttpResponseMessage response, ResponseViewModel? viewModel) = await HttpClientHelper.DeleteAndReturnResponseAsync(_client, url);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
            viewModel?.Should().NotBeNull();
            viewModel?.Success.Should().BeFalse();
            viewModel?.Errors.Should().NotBeEmpty();
        }

        [Fact, TestPriority(402)]
        public async Task Delete_ShouldReturnNoContent_WhenProductExists()
        {
            // Arrange
            ProductViewModel product = await GetLastCreatedProduct();
            string url = $"/api/v1/products/{product.Id}";

            // Act
            (HttpResponseMessage response, _) = await HttpClientHelper.DeleteAndReturnResponseAsync(_client, url);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.NoContent);
        }

        [Fact, TestPriority(403)]
        public async Task Delete_ShouldReturnNotFound_WhenIdIsZero()
        {
            // Arrange
            string url = "/api/v1/products/0";

            // Act
            (HttpResponseMessage response, ResponseViewModel? viewModel) = await HttpClientHelper.DeleteAndReturnResponseAsync(_client, url);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
            viewModel.Should().NotBeNull();
            viewModel!.Success.Should().BeFalse();
        }

        [Fact, TestPriority(404)]
        public async Task Delete_ShouldReturnNotFound_WhenDeletingSameProductTwice()
        {
            // Arrange
            ProductViewModel product = await GetLastCreatedProduct();
            string url = $"/api/v1/products/{product.Id}";

            // Act
            (HttpResponseMessage firstResponse, _) = await HttpClientHelper.DeleteAndReturnResponseAsync(_client, url);
            
            // Assert
            firstResponse.StatusCode.Should().Be(HttpStatusCode.NoContent);

            // Act
            (HttpResponseMessage secondResponse, ResponseViewModel? secondViewModel) = await HttpClientHelper.DeleteAndReturnResponseAsync(_client, url);

            // Assert
            secondResponse.StatusCode.Should().Be(HttpStatusCode.NotFound);
            secondViewModel.Should().NotBeNull();
            secondViewModel!.Success.Should().BeFalse();
        }

        #endregion
    }
}