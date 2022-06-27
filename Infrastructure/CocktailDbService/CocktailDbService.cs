using Newtonsoft.Json;

namespace Infrastructure.CocktailDbService
{
    public interface ICocktailDbService
    {
        Task<CocktailResponse?> GetCocktail(string cocktailName);
        Task<CocktailResponse?> GetRandomCocktail();
    }

    public class CocktailDbService : ICocktailDbService
    {
        private readonly HttpClient _httpClient;

        public CocktailDbService(HttpClient? httpClient)
        {
            _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
        }

        public async Task<CocktailResponse?> GetCocktail(string cocktailName)
        {
            var requestUri = $"/api/json/v1/1/search.php?s={cocktailName}";
            return await Publish(requestUri);
        }

        public async Task<CocktailResponse?> GetRandomCocktail()
        {
            const string requestUri = "/api/json/v1/1/random.php";
            return await Publish(requestUri);
        }

        private async Task<CocktailResponse?> Publish(string requestUri)
        {
            using var httpRequest = new HttpRequestMessage(HttpMethod.Get, requestUri);

            var response = await _httpClient.SendAsync(httpRequest);
            var responseContent = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                throw new Exception(); //TODO make custom exception
            }

            return JsonConvert.DeserializeObject<CocktailResponse>(responseContent);
        }
    }
}