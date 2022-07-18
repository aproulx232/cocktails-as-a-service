using Azure.Data.Tables;

namespace Infrastructure.TableService
{
    public interface ITableService
    {
        Task<UserEntity?> GetUserEntity(string phoneNumber);
        Task AddUserEntity(string phoneNumber);
    }

    public class TableService : ITableService
    {
        private readonly TableClient _tableClient;

        private const string PartitionKey = "DefaultPartition";

        public TableService(TableServiceClient tableServiceClient, ITableServiceConfiguration tableServiceConfiguration)
        {
            if (tableServiceClient == null) throw new ArgumentNullException(nameof(tableServiceClient));
            if (tableServiceConfiguration == null) throw new ArgumentNullException(nameof(tableServiceConfiguration));

            tableServiceClient.CreateTableIfNotExists(tableServiceConfiguration.TableName);
            _tableClient = tableServiceClient.GetTableClient(tableServiceConfiguration.TableName);
        }

        public async Task<UserEntity?> GetUserEntity(string phoneNumber)
        {
            UserEntity? userEntity = null;
            var result = _tableClient.QueryAsync<UserEntity>($"RowKey eq '{phoneNumber}'");
            var entityEnumerator = result.GetAsyncEnumerator();
            try
            {
                while (await entityEnumerator.MoveNextAsync())
                {
                    userEntity = entityEnumerator.Current;
                }
            }
            finally
            {
                await entityEnumerator.DisposeAsync();
            }

            return userEntity;
        }

        public async Task AddUserEntity(string phoneNumber)
        {
            await _tableClient.AddEntityAsync(new UserEntity
            {
                PartitionKey = PartitionKey,
                RowKey = phoneNumber
            });
        }
    }
}
