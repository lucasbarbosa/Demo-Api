using DemoApi.Api.Test.Builders.Products;
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

        protected async Task<ProductViewModel> GetLastCreatedProduct()
        {
            HttpClient client = await GetAuthenticatedClient();
            string url = "/api/v1/products";
            ProductViewModel newProduct = ProductViewModelBuilder.New().Build();
            (HttpResponseMessage _, ResponseViewModel? createResponse) = await HttpClientHelper.PostAndReturnResponseAsync(client, url, newProduct);

            ProductViewModel? createdProduct = JsonConvert.DeserializeObject<ProductViewModel>(
                createResponse!.Data!.ToString()!);

            return createdProduct!;
        }

        #endregion
    }
}