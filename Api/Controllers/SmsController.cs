using Application;
using Microsoft.AspNetCore.Mvc;
using Twilio.AspNet.Common;
using Twilio.AspNet.Core;

namespace Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class SmsController : TwilioController
    {
        private readonly ICocktailProvider _cocktailProvider;
        private readonly IUserProvider _userProvider;
        private readonly IViewProvider _viewProvider;

        public SmsController(ICocktailProvider? cocktailProvider, IUserProvider? userProvider, IViewProvider? viewProvider)
        {
            _cocktailProvider = cocktailProvider ?? throw new ArgumentNullException(nameof(cocktailProvider));
            _userProvider = userProvider ?? throw new ArgumentNullException(nameof(userProvider));
            _viewProvider = viewProvider ?? throw new ArgumentNullException(nameof(viewProvider));
        }

        [HttpGet]
        public async Task<TwiMLResult> GetCocktailRecipe([FromQuery]SmsRequest smsRequest)
        {
            _viewProvider.CreateResponseMessage();
            
            var phoneNumber = smsRequest.From;
            if (await _userProvider.IsNewUser(phoneNumber))
            {
                _viewProvider.AddWelcomeMessage();
            }

            var message = smsRequest.Body?.ToLowerInvariant().Trim();
            if (message == null)
            {
                _viewProvider.AddErrorMessage("query message is null");
                return CompleteMessage();
            }

            switch (message)
            {
                case "help":
                    GetHelpResponse();
                    break;
                case "save":
                    AddToFavoritesResponse();
                    break;
                case "random":
                    await GetRandomCocktail();
                    break;
                default:
                    await GetCocktail(message);
                    break;
            }

            return CompleteMessage();
        }

        private void GetHelpResponse()
        {
            _viewProvider.AddErrorMessage("Please list cocktail name");
        }

        private void AddToFavoritesResponse()
        {
            _viewProvider.AddErrorMessage("Cocktail will be added to favorites list. (Not implemented yet)");
        }

        private async Task GetRandomCocktail()
        {
            try
            {
                var cocktail = await _cocktailProvider.GetRandomCocktail();
                _viewProvider.AddCocktailResponse(cocktail);
            }
            catch (Exception)
            {
                _viewProvider.AddErrorMessage("Failed to get random cocktail");
            }
        }

        private async Task GetCocktail(string cocktailName)
        {
            try
            {
                var cocktail = await _cocktailProvider.GetCocktail(cocktailName);
                _viewProvider.AddCocktailResponse(cocktail);
            }
            catch (Exception)
            {
                _viewProvider.AddErrorMessage("Unknown cocktail");
            }
        }
        private TwiMLResult CompleteMessage()
        {
            return TwiML(_viewProvider.GetResponseMessage());
        }
    }
}