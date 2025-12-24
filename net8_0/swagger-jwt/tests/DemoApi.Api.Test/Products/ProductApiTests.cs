using Bogus;
using DemoApi.Api.Test.Factories;
using DemoApi.Api.Test.Helpers;
using DemoApi.Application.Models;
using DemoApi.Application.Models.Products;
using Newtonsoft.Json.Linq;
using System.Net.Http.Headers;
using System.Net.Http.Json;

namespace DemoApi.Api.Test.Products
{
    public class ProductApiTests : IClassFixture<CustomWebApplicationFactory>
    {
        #region Properties

        protected readonly CustomWebApplicationFactory _factory;
        protected readonly HttpClient _client;
        protected readonly Lazy<Task<HttpClient>> _authenticatedClientLazy;
        protected HttpClient AuthenticatedClient => _authenticatedClientLazy.Value.Result;
        protected const string ValidSecurityKey = "b5b622cd-9f73-43b8-8dce-aab520cf1a2b";
        
        #endregion

        #region Constructors

        public ProductApiTests(CustomWebApplicationFactory factory)
        {
            _factory = factory;
            _client = factory.CreateClient();
            _authenticatedClientLazy = new Lazy<Task<HttpClient>>(GetAuthenticatedClient);
        }

        #endregion

        #region Protected Methods

        protected async Task<HttpClient> GetAuthenticatedClient()
        {
            var tokenClient = _factory.CreateClient();
            tokenClient.DefaultRequestHeaders.Add("X-Security-Key", ValidSecurityKey);
            var result = await tokenClient.PostAsync("/api/v1/auth/token", null);
            var response = await result.Content.ReadFromJsonAsync<ResponseViewModel>();

            string token = string.Empty;
            if (response?.Data != null)
            {
                var tokenJson = JObject.Parse(response.Data.ToString()!);
                var accessToken = tokenJson["accessToken"];
                token = accessToken?.ToString() ?? string.Empty;
            }

            var client = _factory.CreateClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            return client;
        }

        protected static ProductViewModel NewProduct()
        {
            var faker = new Faker<ProductViewModel>()
                .RuleFor(p => p.Id, f => 0u)
                .RuleFor(p => p.Name, f => f.Commerce.ProductName())
                .RuleFor(p => p.Weight, f => Math.Round(f.Random.Double(0.1, 10.0), 2));

            return faker.Generate();
        }

        protected static ProductViewModel NewProductWithRandomId()
        {
            var faker = new Faker<ProductViewModel>()
                .RuleFor(p => p.Id, f => f.Random.UInt(0, uint.MaxValue))
                .RuleFor(p => p.Name, f => f.Commerce.ProductName())
                .RuleFor(p => p.Weight, f => Math.Round(f.Random.Double(0.1, 10.0), 2));

            return faker.Generate();
        }

        protected static ProductViewModel UpdateProduct()
        {
            var faker = new Faker<ProductViewModel>()
                .RuleFor(p => p.Id, f => 1u)
                .RuleFor(p => p.Name, f => f.Commerce.ProductName())
                .RuleFor(p => p.Weight, f => Math.Round(f.Random.Double(0.1, 10.0), 2));

            return faker.Generate();
        }

        protected static ProductViewModel NonExistentProduct()
        {
            return new ProductViewModel
            {
                Id = 999999,
                Name = "Non Existent Product",
                Weight = 1.0
            };
        }

        protected static ProductViewModel ProductWithEmptyName()
        {
            return new ProductViewModel
            {
                Id = 0,
                Name = string.Empty,
                Weight = 2.5
            };
        }

        protected static ProductViewModel ProductWithNullName()
        {
            return new ProductViewModel
            {
                Id = 0,
                Name = null!,
                Weight = 2.5
            };
        }

        protected static ProductViewModel ProductWithZeroWeight()
        {
            return new ProductViewModel
            {
                Id = 0,
                Name = "Test Product",
                Weight = 0
            };
        }

        protected static ProductViewModel ProductWithNegativeWeight()
        {
            return new ProductViewModel
            {
                Id = 0,
                Name = "Test Product",
                Weight = -1.5
            };
        }

        protected static ProductViewModel ProductToUpdate(ProductViewModel createdProduct)
        {
            return new ProductViewModel
            {
                Id = createdProduct!.Id,
                Name = "Updated Product Name",
                Weight = 5.0
            };
        }

        protected async Task<ProductViewModel> GetLastCreatedProduct()
        {
            HttpClient client = await GetAuthenticatedClient();
            var url = "/api/v1/products";
            var newProduct = NewProductWithRandomId();
            var (_, createResponse) = await HttpClientHelper.PostAndReturnResponseAsync(client, url, newProduct);

            var createdProduct = Newtonsoft.Json.JsonConvert.DeserializeObject<ProductViewModel>(
                createResponse!.Data!.ToString()!);

            return createdProduct!;
        }

        #endregion
    }
}