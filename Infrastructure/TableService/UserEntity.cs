using Azure;
using Azure.Data.Tables;

namespace Infrastructure.TableService
{
    public class UserEntity : ITableEntity
    {
        public string? FavoriteCocktails { get; set; }
        public string? PreviousCocktail { get; set; }
        public string? PartitionKey { get; set; }
        public string? RowKey { get; set; } // Phone Number
        public DateTimeOffset? Timestamp { get; set; }
        public ETag ETag { get; set; }
    }
}
