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
        [HttpPost]
        public TwiMLResult Index([FromQuery]SmsRequest request)
        {
            return GetCocktailResponse();
            //TODO check if we have seen this number before, if not, send welcome message

            var message = request.Body.ToLowerInvariant();

            return message switch
            {
                "help" => GetHelpResponse(),
                "save" => AddToFavoritesResponse(),
                _ => GetCocktailResponse()
            };
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
            messagingResponse.Message("Cocktail will be added to favorites list");

            return TwiML(messagingResponse);
        }
        private TwiMLResult GetCocktailResponse()
        {
            var messagingResponse = new MessagingResponse();
            messagingResponse.Message("5 mint leaves, more for garnish\r\n2 ounces white rum\r\n1 ounce fresh lime juice\r\n½ ounce simple syrup\r\nIce\r\nClub soda or sparkling water\r\nLime slices, for garnish");

            return TwiML(messagingResponse);
        }
    }
}