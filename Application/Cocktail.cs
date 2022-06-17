namespace Application
{
    public class Cocktail
    {
        public string? Name { get; set; }
        public string? Glass { get; set; }
        public string? Instructions { get; set; }
        public string? Thumbnail { get; set; }
        public List<Ingredient>? Ingredients { get; set; }
    }
}