using Match.Models;
using Match.Models.RequestModels;

namespace Match.Interfaces
{
    public interface IUserService
    {
        Task<Tuple<bool, User?, string?>> RegisterUserAsync(RegisterRequestModel model);
        Task<Tuple<bool, User?, string?>> LoginUserAsync(LoginRequestModel model);
        Task<Tuple<bool, User?, string?>> DepositAsync(int coin);
        Task<Tuple<bool, User?>> GetUserById(string userId);
        Task<Tuple<bool, User?>> GetUserAsync();
        Task<List<string>> GetUserRolesAsync();
        Task<string> DeleteUserAsync();
        Task<Tuple<bool, User?>> UpdateUserAsync(UpdateUserRequestModel user);
        Task<Tuple<bool, User?>> UpdateUserBalanceAsync(decimal newBalance);
        Task<Tuple<bool, string?>> LogoutUserAsync(LoginRequestModel model);
    }
}
