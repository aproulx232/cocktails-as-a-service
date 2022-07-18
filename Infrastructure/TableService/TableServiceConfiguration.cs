namespace Infrastructure.TableService
{
    public interface ITableServiceConfiguration
    {
        public string TableName { get; }
    }

    public class TableServiceConfiguration : ITableServiceConfiguration
    {
        public string TableName { get; }

        public TableServiceConfiguration(string tableName)
        {
            if (string.IsNullOrWhiteSpace(tableName)) throw new ArgumentNullException(nameof(tableName));
            TableName = tableName;
        }
    }
}
