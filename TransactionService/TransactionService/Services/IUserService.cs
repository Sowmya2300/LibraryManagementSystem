namespace TransactionService.Services
{
    public interface IUserService
    {
        Task<User> GetUserByIdAsync(int userId);
    }
}

