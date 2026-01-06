using DemoApi.Api.Test.Builders.Products;
using DemoApi.Api.Test.Configuration;
using DemoApi.Api.Test.Factories;
using DemoApi.Api.Test.Helpers;
using DemoApi.Application.Models;
using DemoApi.Application.Models.Products;
using FluentAssertions;
using System.Net;
using System.Net.Http.Json;
using System.Text;

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
            ProductViewModel productFake = ProductViewModelBuilder.New().Build();

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
            ProductViewModel productFake = ProductViewModelBuilder.New()
                .WithEmptyName()
                .WithWeight(2.5)
                .Build();

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
            ProductViewModel productFake = ProductViewModelBuilder.New()
                .WithNullName()
                .WithWeight(2.5)
                .Build();

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
            ProductViewModel productFake = ProductViewModelBuilder.New()
                .WithName("Test Product")
                .WithZeroWeight()
                .Build();

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
            ProductViewModel productFake = ProductViewModelBuilder.New()
                .WithName("Test Product")
                .WithNegativeWeight()
                .Build();

            // Act
            (HttpResponseMessage result, ResponseViewModel? response) = await HttpClientHelper.PostAndReturnResponseAsync(client, url, productFake);

            // Assert
            result.StatusCode.Should().Be(HttpStatusCode.PreconditionFailed);
            response!.Should().NotBeNull();
            response!.Success.Should().BeFalse();
            response!.Errors.Should().Contain("Weight must be greater than 0");
        }

        [Fact, TestPriority(105)]
        public async Task Create_ShouldReturnCreated_WhenProductNameHasLongLength()
        {
            // Arrange
            HttpClient client = await GetAuthenticatedClient();
            string url = "/api/v1/products";
            ProductViewModel productFake = ProductViewModelBuilder.New()
                .WithLongName()
                .Build();

            // Act
            (HttpResponseMessage result, ResponseViewModel? response) = await HttpClientHelper.PostAndReturnResponseAsync(client, url, productFake);

            // Assert
            result.StatusCode.Should().Be(HttpStatusCode.Created);
            response.Should().NotBeNull();
            response!.Success.Should().BeTrue();
        }

        [Fact, TestPriority(106)]
        public async Task Create_ShouldReturnCreated_WhenWeightIsLarge()
        {
            // Arrange
            HttpClient client = await GetAuthenticatedClient();
            string url = "/api/v1/products";
            ProductViewModel productFake = ProductViewModelBuilder.New()
                .WithName("Heavy Product")
                .WithLargeWeight()
                .Build();

            // Act
            (HttpResponseMessage result, ResponseViewModel? response) = await HttpClientHelper.PostAndReturnResponseAsync(client, url, productFake);

            // Assert
            result.StatusCode.Should().Be(HttpStatusCode.Created);
            response.Should().NotBeNull();
            response!.Success.Should().BeTrue();
        }

        [Fact, TestPriority(107)]
        public async Task Create_ShouldReturnBadRequest_WhenDuplicateProductName()
        {
            // Arrange
            HttpClient client = await GetAuthenticatedClient();
            string url = "/api/v1/products";
            ProductViewModel product = ProductViewModelBuilder.New()
                .WithUniqueName()
                .Build();

            await HttpClientHelper.PostAndReturnResponseAsync(client, url, product);

            ProductViewModel duplicateProduct = ProductViewModelBuilder.New()
                .WithUniqueName()
                .Build();

            // Act
            (HttpResponseMessage result, ResponseViewModel? response) = await HttpClientHelper.PostAndReturnResponseAsync(client, url, duplicateProduct);

            // Assert
            result.StatusCode.Should().Be(HttpStatusCode.BadRequest);
            response.Should().NotBeNull();
            response!.Success.Should().BeFalse();
            response.Errors.Should().Contain(e => e.Contains("already registered"));
        }

        [Fact, TestPriority(108)]
        public async Task Create_ShouldAccept_ProductsWithDifferentNamesButSimilarWeight()
        {
            // Arrange
            HttpClient client = await GetAuthenticatedClient();
            string url = "/api/v1/products";
            ProductViewModel product1 = ProductViewModelBuilder.New()
                .WithWeight(1.5)
                .Build();

            ProductViewModel product2 = ProductViewModelBuilder.New()
                .WithWeight(1.5)
                .Build();

            // Act
            (HttpResponseMessage result1, ResponseViewModel? _) = await HttpClientHelper.PostAndReturnResponseAsync(client, url, product1);
            (HttpResponseMessage result2, ResponseViewModel? _) = await HttpClientHelper.PostAndReturnResponseAsync(client, url, product2);

            // Assert
            result1.StatusCode.Should().Be(HttpStatusCode.Created);
            result2.StatusCode.Should().Be(HttpStatusCode.Created);
        }

        [Fact, TestPriority(109)]
        public async Task Create_ShouldHandleMultipleConcurrentRequests()
        {
            // Arrange
            HttpClient client = await GetAuthenticatedClient();
            string url = "/api/v1/products";
            List<Task<HttpResponseMessage>> tasks = [];

            for (int i = 0; i < 5; i++)
            {
                ProductViewModel product = ProductViewModelBuilder.New()
                    .WithName($"Concurrent Product {Guid.NewGuid()}")
                    .Build();
                tasks.Add(client.PostAsJsonAsync(url, product));
            }

            // Act
            HttpResponseMessage[] results = await Task.WhenAll(tasks);

            // Assert
            int successCount = results.Count(r => r.StatusCode == HttpStatusCode.Created);
            successCount.Should().BeGreaterThanOrEqualTo(1, "at least one concurrent request should succeed");
        }

        [Fact, TestPriority(110)]
        public async Task Create_ShouldReturnPreconditionFailed_WhenPayloadIsNull()
        {
            // Arrange
            HttpClient client = await GetAuthenticatedClient();
            string url = "/api/v1/products";
            ProductViewModel? productFake = null;

            // Act
            (HttpResponseMessage result, ResponseViewModel? response) = await HttpClientHelper.PostAndReturnResponseAsync(client, url, productFake);

            // Assert
            result.StatusCode.Should().Be(HttpStatusCode.PreconditionFailed);
            response.Should().NotBeNull();
            response!.Success.Should().BeFalse();
        }

        [Fact, TestPriority(111)]
        public async Task Create_ShouldReturnUnsupportedMediaType_WhenContentTypeIsInvalid()
        {
            // Arrange
            HttpClient client = await GetAuthenticatedClient();
            string url = "/api/v1/products";
            StringContent content = new("invalid-json", Encoding.UTF8, "text/plain");

            // Act
            HttpResponseMessage response = await client.PostAsync(url, content);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.UnsupportedMediaType);
        }

        #endregion
    }
}