using Application;
using Microsoft.AspNetCore.Mvc;
using Twilio.AspNet.Common;
using Twilio.AspNet.Core;
using Twilio.TwiML;

namespace Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class SmsController : TwilioController
    {
        private readonly ICocktailProvider _cocktailProvider;

        public SmsController(ICocktailProvider cocktailProvider)
        {
            _cocktailProvider = cocktailProvider;
        }

        [HttpGet]
        public async Task<TwiMLResult> GetCocktailRecipe([FromQuery]SmsRequest smsRequest)
        {
            //TODO check if we have seen this number before, if not, send welcome message

            var message = smsRequest.Body?.ToLowerInvariant();
            if (message == null)
                return GetErrorResponse("query message is null");

            return message switch
            {
                "help" => GetHelpResponse(),
                "save" => AddToFavoritesResponse(),
                "random" => AddToFavoritesResponse(),
                _ => await GetCocktail(message)
            };
        }

        private TwiMLResult GetErrorResponse(string message)
        {
            var messagingResponse = new MessagingResponse();
            messagingResponse.Message(message);

            return TwiML(messagingResponse);
        }

        private async Task<TwiMLResult> GetCocktail(string cocktailName)
        {
            var cocktail = await _cocktailProvider.GetCocktail(cocktailName);
            return GetCocktailResponse(cocktail);
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

        private TwiMLResult GetCocktailResponse(Cocktail cocktail)
        {
            var messagingResponse = new MessagingResponse();
            messagingResponse.Message(cocktail.Instructions);

            var ingredientResponse = cocktail.Ingredients?
                .Select(i => $"{i.Measurement} {i.Name}")
                .Aggregate(string.Empty, (s, s1) => $"{s}\r\n{s1}");
            messagingResponse.Message(ingredientResponse);

            return TwiML(messagingResponse);
        }
    }
}