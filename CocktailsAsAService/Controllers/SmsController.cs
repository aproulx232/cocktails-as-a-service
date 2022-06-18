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
        public async Task<TwiMLResult> Index([FromQuery]SmsRequest queryRequest, [FromBody] SmsRequest bodyRequest)
        {
            //TODO check if we have seen this number before, if not, send welcome message

            var message = bodyRequest.Body?.ToLowerInvariant();
            if (message == null)
                return GetErrorResponse("body message is null");

            message = queryRequest.Body?.ToLowerInvariant();
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
            var cocktail = await _cocktailDbService.GetCocktail(cocktailName);
            return GetCocktailResponse(cocktail.Instructions);
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
        private TwiMLResult GetCocktailResponse(string? message)
        {
            var messagingResponse = new MessagingResponse();
            messagingResponse.Message(message);

            return TwiML(messagingResponse);
        }
    }
}