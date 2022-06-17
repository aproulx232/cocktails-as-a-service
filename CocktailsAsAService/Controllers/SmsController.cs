using Infrastructure;
using Infrastructure.CocktailDbService;
using Microsoft.AspNetCore.Mvc;
using Twilio.AspNet.Common;
using Twilio.AspNet.Core;
using Twilio.TwiML;

namespace CocktailsAsAService.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class SmsController : TwilioController
    {
        private readonly ICocktailDbService _cocktailDbService;

        public SmsController(ICocktailDbService cocktailDbService)
        {
            _cocktailDbService = cocktailDbService ?? throw new ArgumentNullException(nameof(cocktailDbService));
        }

        [HttpPost]
        public async Task<TwiMLResult> Index([FromQuery]SmsRequest request)
        {
            //TODO check if we have seen this number before, if not, send welcome message

            var message = request.Body.ToLowerInvariant();

            return message switch
            {
                "help" => GetHelpResponse(),
                "save" => AddToFavoritesResponse(),
                _ => await GetCocktail(message)
            };
        }

        private async Task<TwiMLResult> GetCocktail(string cocktailName)
        {
            var recipe = await _cocktailDbService.GetRecipe(cocktailName);
            return GetCocktailResponse(recipe);
        }

        private TwiMLResult GetHelpResponse()
        {
            var messagingResponse = new MessagingResponse();
            messagingResponse.Message("Please list cocktail name");

            return TwiML(messagingResponse);
        }

        private TwiMLResult AddToFavoritesResponse()
        {
            var messagingResponse = new MessagingResponse();
            messagingResponse.Message("Cocktail will be added to favorites list. (Not implemented yet)");

            return TwiML(messagingResponse);
        }
        private TwiMLResult GetCocktailResponse(string message)
        {
            var messagingResponse = new MessagingResponse();
            //messagingResponse.Message("5 mint leaves, more for garnish\r\n2 ounces white rum\r\n1 ounce fresh lime juice\r\n½ ounce simple syrup\r\nIce\r\nClub soda or sparkling water\r\nLime slices, for garnish");
            messagingResponse.Message(message);

            return TwiML(messagingResponse);
        }
    }
}