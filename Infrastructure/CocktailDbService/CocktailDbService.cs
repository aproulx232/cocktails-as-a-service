using Microsoft.Extensions.Logging;

namespace Infrastructure.CocktailDbService
{
    public interface ICocktailDbService
    {
        Task<string> GetRecipe(string cocktailName);
    }

    public class CocktailDbService : ICocktailDbService
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<CocktailDbService> _logger;

        public CocktailDbService(HttpClient httpClient, ILogger<CocktailDbService> logger)
        {
            _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<string> GetRecipe(string cocktailName)
        {
            var requestUri = $"/api/json/v1/1/search.php?s={cocktailName}";
            using var httpRequest = new HttpRequestMessage(HttpMethod.Get, requestUri);

            var response = await _httpClient.SendAsync(httpRequest);

            if (!response.IsSuccessStatusCode)
            {
                return await response.Content.ReadAsStringAsync();
            }

            return await response.Content.ReadAsStringAsync();
        }
    }
}