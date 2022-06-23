using FluentAssertions;
using Infrastructure.CocktailDbService;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Newtonsoft.Json;
using System;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Api.Test.Acceptance
{
    public class ApiAcceptanceTestBase : IClassFixture<WebApplicationFactory<Startup>>
    {
        protected readonly HttpClient HttpClient;
        protected HttpResponseMessage? HttpResponse;
        protected const string CocktailDbBaseUrl = "https://thecocktaildb-mock.com";

        protected readonly Mock<HttpMessageHandler> MockCocktailDbHttpMessageHandler = new();

        protected ApiAcceptanceTestBase(WebApplicationFactory<Startup> factory)
        {
            HttpClient = factory
                         .WithWebHostBuilder(webHostBuilder =>
                         {
                             webHostBuilder
                                 .ConfigureTestServices(serviceCollection =>
                                 {
                                     serviceCollection.AddSingleton(BuildMockHttpClientFactory());
                                 });
                         })
                         .CreateClient();
        }

        private IHttpClientFactory BuildMockHttpClientFactory()
        {
            var mockHttpClientFactory = new Mock<IHttpClientFactory>();

            var mockCocktailDbToggleHttpClient =
                new HttpClient(MockCocktailDbHttpMessageHandler.Object)
                {
                    BaseAddress = new Uri(CocktailDbBaseUrl)
                };
            mockHttpClientFactory
                .Setup(httpClientFactory => httpClientFactory.CreateClient(nameof(ICocktailDbService)))
                .Returns(mockCocktailDbToggleHttpClient);

            return mockHttpClientFactory.Object;
        }

        protected static StringContent? BuildRequestContent(object request)
        {
            return new StringContent(JsonConvert.SerializeObject(request),
                                     Encoding.UTF8,
                                     "application/json");
        }

        protected async Task ThenResponseIs(HttpStatusCode expectedStatusCode, string? expectedContent = null)
        {
            HttpResponse?.StatusCode.Should().Be(expectedStatusCode);
         
            if (expectedContent != null)
            {
                var responseContent = await HttpResponse!.Content.ReadAsStringAsync();
                responseContent.Should().Be(expectedContent);
            }
        }
    }
}
