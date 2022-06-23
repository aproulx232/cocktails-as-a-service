using Infrastructure.CocktailDbService;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.WebUtilities;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Twilio.AspNet.Common;
using Xunit;

namespace Api.Test.Acceptance.Features
{
    public class GetCocktailRecipeFeature : ApiAcceptanceTestBase
    {
        private string? _requestUri;

        public GetCocktailRecipeFeature(WebApplicationFactory<Startup> factory) : base(factory) { }

        [Fact(DisplayName = "Given a cocktail name, Then the cocktail recipe is sent to the user")]
        public async Task GetCocktailRecipeScenario()
        {
            GivenValidRequest();
            GivenSuccessfulCallToCocktailDb();

            await WhenCallingGetCocktailRecipeEndpoint();

            await ThenResponseIs(HttpStatusCode.OK, CreatedExpectedResponse());
        }

        private void GivenValidRequest()
        {
            var request = CreateSmsRequest();
            var query = new Dictionary<string, string?>
            {
                [nameof(SmsRequest.Body)] = request.Body
            };

            _requestUri = QueryHelpers.AddQueryString("/sms", query);
        }

        private static SmsRequest CreateSmsRequest()
        {
            return new SmsRequest
            {
                Body = "MockCocktailName"
            };
        }

        private void GivenSuccessfulCallToCocktailDb()
        {
            var cocktailResponse = new CocktailResponse
            {
               Drinks = new List<DrinkResponse>
               {
                   new()
                   {
                       Instructions = "MockInstructions",
                       Ingredient1 = "MockIngredient1",
                       Measure1 = "MockMeasure1"
                   }
               }
            };
            var response = new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent(JsonConvert.SerializeObject(cocktailResponse))
            };

            var query = new Dictionary<string, string?>
            {
                ["s"] = "mockcocktailname"
            };
            var uri = QueryHelpers.AddQueryString($"{CocktailDbBaseUrl}/api/json/v1/1/search.php", query);
            MockHelper.SetupHttpCall(MockCocktailDbHttpMessageHandler, uri, response);
        }

        private async Task WhenCallingGetCocktailRecipeEndpoint()
        {
            HttpResponse = await HttpClient.GetAsync(_requestUri);
        }

        private static string CreatedExpectedResponse()
        {
            return "<?xml version=\"1.0\" encoding=\"utf-8\"?>\r\n<Response>\r\n  <Message>MockInstructions</Message>\r\n  <Message>\r\nMockMeasure1 MockIngredient1</Message>\r\n</Response>";
        }
    }
}