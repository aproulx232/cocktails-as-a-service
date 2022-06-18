using Infrastructure.CocktailDbService;

namespace Application
{
    public interface ICocktailProvider
    {
        Task<Cocktail> GetCocktail(string cocktailName);
    }

    public class CocktailProvider : ICocktailProvider
    {
        private readonly ICocktailDbService _cocktailDbService;

        public CocktailProvider(ICocktailDbService cocktailDbService)
        {
            _cocktailDbService = cocktailDbService ?? throw new ArgumentNullException(nameof(cocktailDbService));
        }

        public async Task<Cocktail> GetCocktail(string cocktailName)
        {
           var cocktailResponse = await _cocktailDbService.GetCocktail(cocktailName);

           return MapToCocktail(cocktailResponse);
        }

        private static Cocktail MapToCocktail(CocktailResponse? cocktailResponse)
        {
            var drink = cocktailResponse?.Drinks?.FirstOrDefault();
            if (drink == null)
            {
                throw new Exception();
            }

            var ingredients = new List<Ingredient>();
            AddIngredientIfValid(drink.Ingredient1, drink.Measure1, ingredients);
            AddIngredientIfValid(drink.Ingredient2, drink.Measure2, ingredients);
            AddIngredientIfValid(drink.Ingredient3, drink.Measure3, ingredients);
            AddIngredientIfValid(drink.Ingredient4, drink.Measure4, ingredients);
            AddIngredientIfValid(drink.Ingredient5, drink.Measure5, ingredients);
            AddIngredientIfValid(drink.Ingredient6, drink.Measure6, ingredients);
            AddIngredientIfValid(drink.Ingredient7, drink.Measure7, ingredients);
            AddIngredientIfValid(drink.Ingredient8, drink.Measure8, ingredients);
            AddIngredientIfValid(drink.Ingredient9, drink.Measure9, ingredients);
            AddIngredientIfValid(drink.Ingredient10, drink.Measure10, ingredients);
            AddIngredientIfValid(drink.Ingredient11, drink.Measure11, ingredients);
            AddIngredientIfValid(drink.Ingredient12, drink.Measure12, ingredients);
            AddIngredientIfValid(drink.Ingredient13, drink.Measure13, ingredients);
            AddIngredientIfValid(drink.Ingredient14, drink.Measure14, ingredients);
            AddIngredientIfValid(drink.Ingredient15, drink.Measure15, ingredients);

            return new Cocktail
            {
                Name = drink.Name,
                Glass = drink.Glass,
                Instructions = drink.Instructions,
                Thumbnail = drink.Thumbnail,
                Ingredients = ingredients
            };
        }

        private static void AddIngredientIfValid(string? ingredient, string? measure, ICollection<Ingredient> ingredients)
        {
            if (string.IsNullOrWhiteSpace(ingredient))
                return;

            measure ??= string.Empty;
            ingredients.Add(new Ingredient
            {
                Name = ingredient,
                Measurement = measure
            });
        }
    }
}
