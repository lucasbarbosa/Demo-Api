using FluentAssertions;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using System.Text;

namespace DemoApi.Api.Test.Helpers
{
    public static class HttpClientHelper
    {
        public static async Task<HttpResponseMessage> GetAndEnsureSuccessAsync(HttpClient client, string url)
        {
            var response = await client.GetAsync(url);

            response.IsSuccessStatusCode.Should().BeTrue();

            return response;
        }

        public static async Task<HttpResponseMessage> PostAndEnsureSuccessAsync(HttpClient client, string url, object request)
        {
            var requestJson = JsonConvert.SerializeObject(request);
            var requestContent = new StringContent(requestJson, UnicodeEncoding.UTF8, "application/json");
            requestContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");

            var response = await client.PostAsync(url, requestContent);

            response.IsSuccessStatusCode.Should().BeTrue();

            return response;
        }
    }
}