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
    public class GetRandomCocktailRecipeFeature : ApiAcceptanceTestBase
    {
        private string? _requestUri;

        public GetRandomCocktailRecipeFeature(WebApplicationFactory<Startup> factory) : base(factory) { }

        [Fact(DisplayName = "Given a random cocktail is request, Then a random cocktail recipe is sent to the user")]
        public async Task RandomCocktailRecipeScenario()
        {
            GivenValidRequest();
            GivenSuccessfulDrinkResponseFromCocktailDb();

            await WhenCallingGetCocktailRecipeEndpoint();

            await ThenResponseIs(HttpStatusCode.OK, CreatedExpectedRandomResponse());
        }

        private void GivenValidRequest()
        {
            var request = CreateRandomSmsRequest();
            var query = new Dictionary<string, string?>
            {
                [nameof(SmsRequest.Body)] = request.Body
            };

            _requestUri = QueryHelpers.AddQueryString("/sms", query);
        }

        private static SmsRequest CreateRandomSmsRequest()
        {
            return new SmsRequest
            {
                Body = "Random"
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
            SetupSuccessfulGetRandomCocktailCallToCocktailDb(cocktailResponse);
        }

        private void SetupSuccessfulGetRandomCocktailCallToCocktailDb(CocktailResponse cocktailResponse)
        {
            var response = new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent(JsonConvert.SerializeObject(cocktailResponse))
            };

            const string uri = $"{CocktailDbBaseUrl}/api/json/v1/1/random.php";
            MockHelper.SetupHttpCall(MockCocktailDbHttpMessageHandler, uri, response);
        }

        private async Task WhenCallingGetCocktailRecipeEndpoint()
        {
            HttpResponse = await HttpClient.GetAsync(_requestUri);
        }

        private static string CreatedExpectedRandomResponse()
        {
            return "<?xml version=\"1.0\" encoding=\"utf-8\"?>\r\n<Response>\r\n  <Message>MockName: MockInstructions</Message>\r\n  <Message>\r\nMockMeasure1 MockIngredient1</Message>\r\n</Response>";
        }
    }
}
