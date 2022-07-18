using Application;
using Twilio.TwiML;

namespace Api
{
    public interface IViewProvider
    {
        void CreateResponseMessage();
        void AddWelcomeMessage();
        void AddErrorMessage(string message);
        void AddCocktailResponse(Cocktail cocktail);
        MessagingResponse GetResponseMessage();
    }

    public class ViewProvider : IViewProvider
    {
        private MessagingResponse _messagingResponse;

        public ViewProvider()
        {
            _messagingResponse = new MessagingResponse();
        }

        public void CreateResponseMessage()
        {
            _messagingResponse = new MessagingResponse();
        }

        public void AddWelcomeMessage()
        {
            _messagingResponse.Message("Welcome to Cocktails As A Service!\r\nEnter a cocktail's name to get the recipe or reply \"Random\" to get a random cocktail recipe\r\n");
        }

        public void AddErrorMessage(string message)
        {
            _messagingResponse.Message(message);
        }

        public void AddCocktailResponse(Cocktail cocktail)
        {
            var instructionsResponse = $"{cocktail.Name}: {cocktail.Instructions}";
            _messagingResponse.Message(instructionsResponse);

            var ingredientResponse = cocktail.Ingredients?
                .Select(i => $"{i.Measurement} {i.Name}")
                .Aggregate(string.Empty, (s, s1) => $"{s}\r\n{s1}");
            _messagingResponse.Message(ingredientResponse);
        }

        public MessagingResponse GetResponseMessage() => _messagingResponse;
    }
}
