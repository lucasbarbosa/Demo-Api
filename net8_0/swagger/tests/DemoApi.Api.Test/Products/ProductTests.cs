using Bogus;
using DemoApi.Api.Test.Factories;
using DemoApi.Api.Test.Helpers;
using DemoApi.Application.Models;
using DemoApi.Application.Models.Products;
using System.Text.Json;

namespace DemoApi.Api.Test.Products
{
    public class ProductTests(CustomWebApplicationFactory factory) : IClassFixture<CustomWebApplicationFactory>
    {
        #region Properties

        protected readonly HttpClient _client = factory.CreateClient();

        #endregion
        
        #region Protected Methods

        protected static ProductViewModel NewProduct()
        {
            Faker<ProductViewModel> faker = new Faker<ProductViewModel>()
                .RuleFor(p => p.Id, f => 0u)
                .RuleFor(p => p.Name, f => f.Commerce.ProductName())
                .RuleFor(p => p.Weight, f => Math.Round(f.Random.Double(0.1, 10.0), 2));

            return faker.Generate();
        }

        protected static ProductViewModel ProductWithUniqueName()
        {
            Faker<ProductViewModel> faker = new Faker<ProductViewModel>()
                .RuleFor(p => p.Id, f => 0u)
                .RuleFor(p => p.Name, "Unique Product Name Test")
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

        protected static ProductViewModel ProductWithLongName()
        {
            return new ProductViewModel
            {
                Id = 0,
                Name = new string('A', 100),
                Weight = 1.5
            };
        }

        protected static ProductViewModel ProductWithLargeWeight()
        {
            return new ProductViewModel
            {
                Id = 0,
                Name = "Heavy Product",
                Weight = 1000000.99
            };
        }

        protected static ProductViewModel ProductWithIdZero()
        {
            return new ProductViewModel
            {
                Id = 0,
                Name = "Test Product",
                Weight = 1.5
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

        protected static ProductViewModel ProductToUpdateWeightOnly(ProductViewModel createdProduct)
        {
            return new ProductViewModel
            {
                Id = createdProduct.Id,
                Name = createdProduct.Name,
                Weight = createdProduct.Weight + 1.0
            };
        }

        protected static ProductViewModel ProductToUpdateNameOnly(ProductViewModel createdProduct)
        {
            return new ProductViewModel
            {
                Id = createdProduct.Id,
                Name = "New Name Only",
                Weight = createdProduct.Weight
            };
        }

        protected async Task<ProductViewModel> GetLastCreatedProduct()
        {
            string url = "/api/v1/products";
            ProductViewModel newProduct = NewProduct();
            (HttpResponseMessage _, ResponseViewModel? createResponse) = await HttpClientHelper.PostAndReturnResponseAsync(_client, url, newProduct);

            ProductViewModel? createdProduct = JsonSerializer.Deserialize<ProductViewModel>(
                createResponse!.Data!.ToString()!,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            return createdProduct!;
        }

        #endregion
    }
}