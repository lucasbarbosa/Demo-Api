using Bogus;
using DemoApi.Api.Test.Factories;
using DemoApi.Api.Test.Helpers;
using DemoApi.Application.Models;
using DemoApi.Application.Models.Products;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Net.Http.Headers;
using System.Net.Http.Json;

namespace DemoApi.Api.Test.Products
{
    public class ProductApiTests(CustomWebApplicationFactory factory) : IClassFixture<CustomWebApplicationFactory>
    {
        #region Properties

        protected readonly CustomWebApplicationFactory _factory = factory;
        protected readonly HttpClient _client = factory.CreateClient();
        protected const string ValidSecurityKey = "b5b622cd-9f73-43b8-8dce-aab520cf1a2b";

        #endregion
        
        #region Protected Methods

        protected async Task<HttpClient> GetAuthenticatedClient()
        {
            HttpClient tokenClient = _factory.CreateClient();
            tokenClient.DefaultRequestHeaders.Add("X-Security-Key", ValidSecurityKey);
            HttpResponseMessage result = await tokenClient.PostAsync("/api/v1/auth/token", null);
            ResponseViewModel? response = await result.Content.ReadFromJsonAsync<ResponseViewModel>();

            string token = string.Empty;
            if (response?.Data != null)
            {
                JObject tokenJson = JObject.Parse(response.Data.ToString()!);
                JToken? accessToken = tokenJson["accessToken"];
                token = accessToken?.ToString() ?? string.Empty;
            }

            HttpClient client = _factory.CreateClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            return client;
        }

        protected static ProductViewModel NewProduct()
        {
            Faker<ProductViewModel> faker = new Faker<ProductViewModel>()
                .RuleFor(p => p.Id, f => 0u)
                .RuleFor(p => p.Name, f => f.Commerce.ProductName())
                .RuleFor(p => p.Weight, f => Math.Round(f.Random.Double(0.1, 10.0), 2));

            return faker.Generate();
        }

        protected static ProductViewModel NewProductWithRandomId()
        {
            Faker<ProductViewModel> faker = new Faker<ProductViewModel>()
                .RuleFor(p => p.Id, f => f.Random.UInt(0, uint.MaxValue))
                .RuleFor(p => p.Name, f => f.Commerce.ProductName())
                .RuleFor(p => p.Weight, f => Math.Round(f.Random.Double(0.1, 10.0), 2));

            return faker.Generate();
        }

        protected static ProductViewModel UpdateProduct()
        {
            Faker<ProductViewModel> faker = new Faker<ProductViewModel>()
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

        protected static ProductViewModel ProductWithWhitespaceName()
        {
            return new ProductViewModel
            {
                Id = 0,
                Name = "   ",
                Weight = 1.0
            };
        }

        protected static ProductViewModel ProductWithSpecialCharactersName()
        {
            return new ProductViewModel
            {
                Id = 0,
                Name = "Product @#$% 123",
                Weight = 1.0
            };
        }

        protected static ProductViewModel ProductWithVeryLongName()
        {
            return new ProductViewModel
            {
                Id = 0,
                Name = new string('A', 500),
                Weight = 1.0
            };
        }

        protected static ProductViewModel ProductWithUnicodeName()
        {
            return new ProductViewModel
            {
                Id = 0,
                Name = "Produto ??? ?? ???????",
                Weight = 1.0
            };
        }

        protected static ProductViewModel ProductWithSingleCharacterName()
        {
            return new ProductViewModel
            {
                Id = 0,
                Name = "A",
                Weight = 1.0
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

        protected static ProductViewModel ProductWithSpecificWeight(double weight)
        {
            return new ProductViewModel
            {
                Id = 0,
                Name = $"Product Weight {weight}",
                Weight = weight
            };
        }

        protected static ProductViewModel ProductWithPreciseWeight()
        {
            return new ProductViewModel
            {
                Id = 0,
                Name = "Product Precise Weight",
                Weight = 1.123456789
            };
        }

        protected static ProductViewModel ProductWithVerySmallPositiveWeight()
        {
            return new ProductViewModel
            {
                Id = 0,
                Name = "Very Light Product",
                Weight = double.Epsilon
            };
        }

        protected static ProductViewModel ProductWithInvalidNameAndWeight()
        {
            return new ProductViewModel
            {
                Id = 0,
                Name = string.Empty,
                Weight = -1
            };
        }

        protected static ProductViewModel ProductWithWhitespaceNameAndZeroWeight()
        {
            return new ProductViewModel
            {
                Id = 0,
                Name = "   ",
                Weight = 0
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
            string url = "/api/v1/products";
            ProductViewModel newProduct = NewProductWithRandomId();
            (HttpResponseMessage _, ResponseViewModel? createResponse) = await HttpClientHelper.PostAndReturnResponseAsync(client, url, newProduct);

            ProductViewModel? createdProduct = JsonConvert.DeserializeObject<ProductViewModel>(
                createResponse!.Data!.ToString()!);

            return createdProduct!;
        }

        #endregion
    }
}