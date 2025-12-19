using Demo.Api.Test.Configuration;
using Demo.Application.ViewModels;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Newtonsoft.Json;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Demo.Api.Test
{
    [TestCaseOrderer("Demo.Api.Test.Configuration.PriorityOrderer", "Demo.Api.Test")]
    public class ProductApiTests
    {
        #region Attributes

        private readonly TestServer _server;
        private readonly HttpClient _client;
        private readonly string _secretKey = "2d3844a6-f348-4c0e-b2e1-ad9c3181488f";

        #endregion

        #region Constructors

        public ProductApiTests()
        {
            var builder = new WebHostBuilder().UseEnvironment("Development").UseStartup<StartupTest>();

            _server = new TestServer(builder);

            _client = _server.CreateClient();
        }

        #endregion

        #region Public Methods

        [Fact, TestPriority(1)]
        public async Task Product_Create_Ok()
        {
            // Arrange
            var ProductFake = NewProduct();
            var ProductJson = JsonConvert.SerializeObject(ProductFake);
            var ProductContent = new StringContent(ProductJson, UnicodeEncoding.UTF8, "application/json");

            // Act
            await RequestSetToken();
            var response = await _client.PostAsync("/api/v1/Product/Create", ProductContent);

            // Assert
            Assert.True(response.IsSuccessStatusCode);
        }

        [Fact, TestPriority(2)]
        public async Task Product_Update_Ok()
        {
            // Arrange
            var ProductFake = NewProduct();
            ProductFake.Name = "Product Name Update";
            var ProductJson = JsonConvert.SerializeObject(ProductFake);
            var ProductContent = new StringContent(ProductJson, UnicodeEncoding.UTF8, "application/json");

            // Act
            await RequestSetToken();
            var response = await _client.PutAsync("/api/v1/Product/Update", ProductContent);

            // Assert
            Assert.True(response.IsSuccessStatusCode);
        }

        [Fact, TestPriority(3)]
        public async Task Product_GetAll_Ok()
        {
            // Act
            await RequestSetToken();
            var response = await _client.GetAsync("/api/v1/Product/GetAll");

            // Assert
            Assert.True(response.IsSuccessStatusCode);
        }

        [Fact, TestPriority(4)]
        public async Task Product_GetById_Ok()
        {
            // Arrange
            var ProductFake = NewProduct();

            // Act
            await RequestSetToken();
            var response = await _client.GetAsync($"/api/v1/Product/GetById/{ProductFake.Id}");

            // Assert
            Assert.True(response.IsSuccessStatusCode);
        }

        [Fact, TestPriority(5)]
        public async Task Product_Delete_Ok()
        {
            // Arrange
            var ProductFake = NewProduct();

            // Act
            await RequestSetToken();
            var response = await _client.DeleteAsync($"/api/v1/Product/DeleteById/{ProductFake.Id}");

            // Assert
            Assert.True(response.IsSuccessStatusCode);
        }

        [Fact, TestPriority(6)]
        public async Task Product_Create_WithOutId()
        {
            // Arrange
            var ProductFake = NewProductWithOutId();
            var ProductJson = JsonConvert.SerializeObject(ProductFake);
            var ProductContent = new StringContent(ProductJson, UnicodeEncoding.UTF8, "application/json");

            // Act
            await RequestSetToken();
            var response = await _client.PostAsync("/api/v1/Product/Create", ProductContent);
            var responseString = await response.Content.ReadAsStringAsync();
            var responseObject = JsonConvert.DeserializeObject<ResponseViewModel>(responseString);

            // Assert
            Assert.False(responseObject.Success);
            Assert.Contains(responseObject.Errors, x => x.Contains("Id deve ser um valor entre 1 e 4294967295"));
        }

        [Fact, TestPriority(7)]
        public async Task Product_Create_WithOutName()
        {
            // Arrange
            var ProductFake = NewProductWithOutName();
            var ProductJson = JsonConvert.SerializeObject(ProductFake);
            var ProductContent = new StringContent(ProductJson, UnicodeEncoding.UTF8, "application/json");

            // Act
            await RequestSetToken();
            var response = await _client.PostAsync("/api/v1/Product/Create", ProductContent);
            var responseString = await response.Content.ReadAsStringAsync();
            var responseObject = JsonConvert.DeserializeObject<ResponseViewModel>(responseString);

            // Assert
            Assert.False(responseObject.Success);
            Assert.Contains(responseObject.Errors, x => x.Contains("Campo Name é obrigatório"));
        }

        [Fact, TestPriority(8)]
        public async Task Product_Create_WithOutWeight()
        {
            // Arrange
            var ProductFake = NewProductWithOutWeight();
            var ProductJson = JsonConvert.SerializeObject(ProductFake);
            var ProductContent = new StringContent(ProductJson, UnicodeEncoding.UTF8, "application/json");

            // Act
            await RequestSetToken();
            var response = await _client.PostAsync("/api/v1/Product/Create", ProductContent);
            var responseString = await response.Content.ReadAsStringAsync();
            var responseObject = JsonConvert.DeserializeObject<ResponseViewModel>(responseString);

            // Assert
            Assert.False(responseObject.Success);
            Assert.Contains(responseObject.Errors, x => x.Contains("Weight deve ser maior que 0"));
        }

        #endregion

        #region Private Methods

        private async Task RequestSetToken()
        {
            if (_client.DefaultRequestHeaders.Authorization == null)
            {
                _client.DefaultRequestHeaders.Add("SecurityKey", _secretKey);
                var result = await _client.GetAsync("/api/v1/Token/RequestToken");

                var resultContent = await result.Content.ReadAsStringAsync();
                var response = JsonConvert.DeserializeObject<ResponseViewModel>(resultContent);

                if (response.Success)
                {
                    var responseData = JsonConvert.DeserializeObject<TokenViewModel>(response.Data.ToString());

                    _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", responseData.AccessToken);
                }
            }
        }

        private ProductViewModel NewProduct()
        {
            return new ProductViewModel
            {
                Id = 1,
                Name = "Product Name 1",
                Weight = 3.12
            };
        }

        private ProductViewModel NewProductWithOutId()
        {
            return new ProductViewModel
            {
                Name = "Product Name 2",
                Weight = 5.7
            };
        }

        private ProductViewModel NewProductWithOutName()
        {
            return new ProductViewModel
            {
                Id = 3,
                Weight = 4.65
            };
        }

        private ProductViewModel NewProductWithOutWeight()
        {
            return new ProductViewModel
            {
                Id = 4,
                Name = "Product Name 4"
            };
        }

        #endregion
    }
}