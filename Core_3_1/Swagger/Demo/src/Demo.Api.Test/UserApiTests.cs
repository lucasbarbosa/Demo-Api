using Demo.Api.Test.Configuration;
using Demo.Api.ViewModels;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Newtonsoft.Json;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Demo.Api.Test
{
    [TestCaseOrderer("Demo.Api.Test.Configuration.PriorityOrderer", "Demo.Api.Test")]
    public class UserApiTests
    {
        #region Attributes

        private readonly TestServer _server;
        private readonly HttpClient _client;

        #endregion

        #region Constructors

        public UserApiTests()
        {
            _server = new TestServer(new WebHostBuilder().UseStartup<Startup>());

            _client = _server.CreateClient();
        }

        #endregion

        #region Public Methods

        [Fact, TestPriority(1)]
        public async Task User_Create_Ok()
        {
            // Arrange
            var userFake = NewUser();
            var userJson = JsonConvert.SerializeObject(userFake);
            var userContent = new StringContent(userJson, UnicodeEncoding.UTF8, "application/json");

            // Act
            var response = await _client.PostAsync("/api/v1/User/Create", userContent);

            // Assert
            Assert.True(response.IsSuccessStatusCode);
        }

        [Fact, TestPriority(2)]
        public async Task User_Update_Ok()
        {
            // Arrange
            var userFake = NewUser();
            userFake.Name = "User Name Update";
            var userJson = JsonConvert.SerializeObject(userFake);
            var userContent = new StringContent(userJson, UnicodeEncoding.UTF8, "application/json");

            // Act
            var response = await _client.PutAsync("/api/v1/User/Update", userContent);

            // Assert
            Assert.True(response.IsSuccessStatusCode);
        }

        [Fact, TestPriority(3)]
        public async Task User_GetAll_Ok()
        {
            // Act
            var response = await _client.GetAsync("/api/v1/User/GetAll");

            // Assert
            Assert.True(response.IsSuccessStatusCode);
        }

        [Fact, TestPriority(4)]
        public async Task User_GetById_Ok()
        {
            // Arrange
            var userFake = NewUser();

            // Act
            var response = await _client.GetAsync($"/api/v1/User/GetById/{userFake.Id}");

            // Assert
            Assert.True(response.IsSuccessStatusCode);
        }

        [Fact, TestPriority(5)]
        public async Task User_Delete_Ok()
        {
            // Arrange
            var userFake = NewUser();

            // Act
            var response = await _client.DeleteAsync($"/api/v1/User/DeleteById/{userFake.Id}");

            // Assert
            Assert.True(response.IsSuccessStatusCode);
        }

        [Fact, TestPriority(6)]
        public async Task User_Create_WithOutId()
        {
            // Arrange
            var userFake = NewUserWithOutId();
            var userJson = JsonConvert.SerializeObject(userFake);
            var userContent = new StringContent(userJson, UnicodeEncoding.UTF8, "application/json");

            // Act
            var response = await _client.PostAsync("/api/v1/User/Create", userContent);
            var responseString = await response.Content.ReadAsStringAsync();
            var responseObject = JsonConvert.DeserializeObject<ResponseViewModel>(responseString);

            // Assert
            Assert.False(responseObject.Success);
            Assert.Contains(responseObject.Errors, x => x.Contains("Id deve ser um valor entre 1 e 4294967295"));
        }

        [Fact, TestPriority(7)]
        public async Task User_Create_WithOutName()
        {
            // Arrange
            var userFake = NewUserWithOutName();
            var userJson = JsonConvert.SerializeObject(userFake);
            var userContent = new StringContent(userJson, UnicodeEncoding.UTF8, "application/json");

            // Act
            var response = await _client.PostAsync("/api/v1/User/Create", userContent);
            var responseString = await response.Content.ReadAsStringAsync();
            var responseObject = JsonConvert.DeserializeObject<ResponseViewModel>(responseString);

            // Assert
            Assert.False(responseObject.Success);
            Assert.Contains(responseObject.Errors, x => x.Contains("Campo Name é obrigatório"));
        }

        [Fact, TestPriority(8)]
        public async Task User_Create_WithOutEmail()
        {
            // Arrange
            var userFake = NewUserWithOutEmail();
            var userJson = JsonConvert.SerializeObject(userFake);
            var userContent = new StringContent(userJson, UnicodeEncoding.UTF8, "application/json");

            // Act
            var response = await _client.PostAsync("/api/v1/User/Create", userContent);
            var responseString = await response.Content.ReadAsStringAsync();
            var responseObject = JsonConvert.DeserializeObject<ResponseViewModel>(responseString);

            // Assert
            Assert.False(responseObject.Success);
            Assert.Contains(responseObject.Errors, x => x.Contains("Campo E-mail é obrigatório"));
        }

        #endregion

        #region Private Methods

        private UserViewModel NewUser()
        {
            return new UserViewModel
            {
                Id = 1,
                Name = "User Name 1",
                Email = "name1@email.com"
            };
        }

        private UserViewModel NewUserWithOutId()
        {
            return new UserViewModel
            {
                Name = "User Name 2",
                Email = "name2@email.com"
            };
        }

        private UserViewModel NewUserWithOutName()
        {
            return new UserViewModel
            {
                Id = 3,
                Email = "name3@email.com"
            };
        }

        private UserViewModel NewUserWithOutEmail()
        {
            return new UserViewModel
            {
                Id = 4,
                Name = "User Name 4"
            };
        }

        #endregion
    }
}