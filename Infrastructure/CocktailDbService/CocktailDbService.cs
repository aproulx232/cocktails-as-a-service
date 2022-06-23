using Newtonsoft.Json;

namespace Infrastructure.CocktailDbService
{
    public interface ICocktailDbService
    {
        Task<CocktailResponse?> GetCocktail(string cocktailName);
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
            using var httpRequest = new HttpRequestMessage(HttpMethod.Get, requestUri);

            var response = await _httpClient.SendAsync(httpRequest);
            var responseContent = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                throw new Exception();//TODO make custom exception
            }

            return JsonConvert.DeserializeObject<CocktailResponse>(responseContent);
        }
    }
}