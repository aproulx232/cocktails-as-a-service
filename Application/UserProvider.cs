using Infrastructure.TableService;

namespace Application
{
    public interface IUserProvider
    {
        Task<bool> IsNewUser(string phoneNumber);
    }

    public class UserProvider : IUserProvider
    {
        private readonly ITableService _tableService;

        public UserProvider(ITableService tableService)
        {
            _tableService = tableService ?? throw new ArgumentNullException(nameof(tableService));
        }

        public async Task<bool> IsNewUser(string phoneNumber)
        {
            var isNewUser =  await _tableService.GetUserEntity(phoneNumber) == null;

            if (isNewUser)
            {
                await _tableService.AddUserEntity(phoneNumber);
            }
            return isNewUser;
        }
    }
}
