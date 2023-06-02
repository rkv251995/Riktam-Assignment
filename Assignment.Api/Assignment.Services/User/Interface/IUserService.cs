using Assignment.Dtos.Common;
using Assignment.Dtos.User;
using Assignment.Models.User;

namespace Assignment.Services.User.Interface
{
    public interface IUserService
    {
        Task<CommonDto<UserDto>> AddUserAsync(AddUpdateUserModel model);
        Task<CommonDto<UserDto>> UpdateUserAsync(Guid userId, AddUpdateUserModel model);
        Task<CommonDto<UserDto>> FindUserByIdAsync(Guid userId);
        Task<CommonDto<Guid>> DeleteUserByIdAsync(Guid userId);
    }
}
