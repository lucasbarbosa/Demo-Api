using DemoApi.Api.Test.Factories;

namespace DemoApi.Api.Test.Auth
{
    public class AuthTests(CustomWebApplicationFactory factory) : IClassFixture<CustomWebApplicationFactory>
    {
        #region Properties

        protected readonly HttpClient _client = factory.CreateClient();
        protected const string ValidSecurityKey = "b5b622cd-9f73-43b8-8dce-aab520cf1a2b";

        #endregion
    }
}