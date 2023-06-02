using Assignment.Dtos.Common;
using Assignment.Dtos.User;
using Assignment.Infrastructures.EntityFrameworkCore;
using Assignment.Models.Common;
using Assignment.Models.User;
using Assignment.Services.Security.Interface;
using Assignment.Services.User.Interface;
using Assignment.Utilities.Helpers;
using Microsoft.EntityFrameworkCore;
using UserData = Assignment.Infrastructures.EntityFrameworkCore.Entity.User;

namespace Assignment.Services.User
{
    public class UserService : BaseService, IUserService
    {
        private readonly ISecurityService _securityService;

        public UserService(DataContext dataContext,
                           ISecurityService securityService) : base(dataContext)
        {
            _securityService = securityService;
        }

        public async Task<CommonDto<UserDto>> AddUserAsync(AddUpdateUserModel model)
        {
            if (await _dataContext.Users.AnyAsync(a => a.Username.ToLower().Equals(model.Username)))
                throw new RaiseError(new ExceptionDetail { Code = "425", Message = "Username already exists." }.ToString());

            UserData user = new()
            {
                Email = model.Email,
                Password = _securityService.Encrypt(model.Password),
                Username = model.Username,
                FirstName = model.FirstName,
                LastName = model.LastName,
                DateOfBirth = model.DateOfBirth,
                Mobile = model.Mobile,
                Address = model.Address,
                City = model.City,
                State = model.State,
                Country = model.Country,
                IsActive = true,
                CreatedDate = DateTime.Now
            };

            await _dataContext.Users.AddAsync(user);

            await _dataContext.SaveChangesAsync();

            return await FindUserByIdAsync(user.Id);
        }

        public async Task<CommonDto<UserDto>> UpdateUserAsync(Guid userId, AddUpdateUserModel model)
        {
            UserData? user = await _dataContext.Users.FirstOrDefaultAsync(f => f.Id == userId && f.IsActive);

            if (user is null)
                throw new RaiseError(new ExceptionDetail { Code = "426", Message = "User doesn't exists." }.ToString());

            user.Email = model.Email;
            user.FirstName = model.FirstName;
            user.LastName = model.LastName;
            user.DateOfBirth = model.DateOfBirth;
            user.Mobile = model.Mobile;
            user.Address = model.Address;
            user.City = model.City;
            user.State = model.State;
            user.Country = model.Country;
            user.UpdatedDate = DateTime.Now;

            _dataContext.Update(user);
            await _dataContext.SaveChangesAsync();

            return await FindUserByIdAsync(userId);
        }

        public async Task<CommonDto<UserDto>> FindUserByIdAsync(Guid userId)
        {
            UserData? user = await _dataContext.Users.FirstOrDefaultAsync(f => f.Id == userId && f.IsActive);

            if (user is null)
                throw new RaiseError(new ExceptionDetail { Code = "426", Message = "User doesn't exists." }.ToString());

            return new CommonDto<UserDto>
            {
                IsSuccess = true,
                Message = "User Data.",
                Response = new UserDto
                {
                    Id = user.Id,
                    Email = user.Email,
                    Username = user.Username,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    DateOfBirth = user.DateOfBirth,
                    Mobile = user.Mobile,
                    Address = user.Address,
                    City = user.City,
                    State = user.State,
                    Country = user.Country,
                    IsEmailVerified = user.IsEmailVerified,
                    IsMobileVerified = user.IsMobileVerified,
                }
            };
        }

        public async Task<CommonDto<Guid>> DeleteUserByIdAsync(Guid userId)
        {
            UserData? user = await _dataContext.Users.FirstOrDefaultAsync(f => f.Id == userId && f.IsActive);

            if (user is null)
                throw new RaiseError(new ExceptionDetail { Code = "426", Message = "User doesn't exists." }.ToString());

            user.IsActive = false;
            user.UpdatedDate = DateTime.Now;

            _dataContext.Update(user);
            await _dataContext.SaveChangesAsync();

            return new CommonDto<Guid>
            {
                IsSuccess = true,
                Message = "User deleted successfully",
                Response = user.Id
            };
        }
    }
}
