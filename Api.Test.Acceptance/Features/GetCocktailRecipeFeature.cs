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
            GivenSuccessfulDrinkResponseFromCocktailDb();

            await WhenCallingGetCocktailRecipeEndpoint();

            await ThenResponseIs(HttpStatusCode.OK, CreatedExpectedSuccessResponse());
        }

        [Fact(DisplayName = "Given an unknown cocktail name, Then the error message is sent to the user")]
        public async Task UnknownCocktailRecipeScenario()
        {
            GivenValidRequest();
            GivenUnknownDrinkResponseFromCocktailDb();

            await WhenCallingGetCocktailRecipeEndpoint();

            await ThenResponseIs(HttpStatusCode.OK, CreatedExpectedUnknownResponse());
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

        private void GivenSuccessfulDrinkResponseFromCocktailDb()
        {
            var cocktailResponse = new CocktailResponse
            {
               Drinks = new List<DrinkResponse>
               {
                   new()
                   {
                       Name = "MockName",
                       Instructions = "MockInstructions",
                       Ingredient1 = "MockIngredient1",
                       Measure1 = "MockMeasure1"
                   }
               }
            };
            SetupSuccessfulGetCocktailCallToCocktailDb(cocktailResponse);
        }

        private void GivenUnknownDrinkResponseFromCocktailDb()
        {
            var cocktailResponse = new CocktailResponse
            {
                Drinks = null
            };
            SetupSuccessfulGetCocktailCallToCocktailDb(cocktailResponse);
        }

        private void SetupSuccessfulGetCocktailCallToCocktailDb(CocktailResponse cocktailResponse)
        {
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

        private static string CreatedExpectedSuccessResponse()
        {
            return "<?xml version=\"1.0\" encoding=\"utf-8\"?>\r\n<Response>\r\n  <Message>MockName: MockInstructions</Message>\r\n  <Message>\r\nMockMeasure1 MockIngredient1</Message>\r\n</Response>";
        } 

        private static string CreatedExpectedUnknownResponse()
        {
            return "<?xml version=\"1.0\" encoding=\"utf-8\"?>\r\n<Response>\r\n  <Message>Unknown cocktail</Message>\r\n</Response>";
        }
    }
}