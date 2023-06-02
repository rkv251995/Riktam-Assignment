using Assignment.Dtos.Account;
using Assignment.Models.Account;
using Assignment.Models.Common;

namespace Assignment.Services.Account.Interface
{
    public interface IAccountService
    {
        Task<LoginDto> SignInAsync(LoginModel model);
        Task<RefreshTokenDto> RefreshTokenAsync(RefreshTokenModel model);
        Task SignOutAsync(BaseModel model);
    }
}
