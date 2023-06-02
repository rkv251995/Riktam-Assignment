using Assignment.Dtos.Common;
using Assignment.Dtos.User;
using Assignment.Models.Common;
using Assignment.Models.User;
using Assignment.Services.Security.Interface;
using Assignment.Services.User;
using Assignment.Services.User.Interface;
using Assignment.Utilities.Helpers;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Moq;
using Newtonsoft.Json;
using Xunit;
using UserData = Assignment.Infrastructures.EntityFrameworkCore.Entity.User;

namespace Assignment.Services.Test.Services
{
    public class UserServiceTest : BaseServiceTest
    {
        private readonly IUserService _userService;
        private readonly Mock<ISecurityService> _mockSecurityService;

        public UserServiceTest()
        {
            _mockSecurityService = new Mock<ISecurityService>();
            _userService = new UserService(_dataContext, _mockSecurityService.Object);
            _mockSecurityService.Setup(s => s.Encrypt(It.IsAny<string>())).Returns(() => "fORRNGKa8yYj1WaiuGE0728PF1fkz+NDVB10/sXLmcY=");
        }

        [Fact]
        public async Task AddUserAsync_ShouldAddUser_AddUserWithValidData()
        {
            // Arrange  
            AddUpdateUserModel model = new()
            {
                Email = "rajesh@gmail.com",
                Password = "password",
                Username = "rk_vishwakarma_world",
                FirstName = "Rajesh",
                LastName = "Vishwakarma",
                DateOfBirth = "",
                Mobile = "906905690",
                Address = "Road no. 8B, Rajeev Nagar",
                City = "Patna",
                State = "Bihar",
                Country = "India"
            };

            // Act  
            CommonDto<UserDto> result = await _userService.AddUserAsync(model);

            //Assert  
            result.Should().NotBeNull();
            result.IsSuccess.Should().BeTrue();
            result.Response.Should().NotBeNull();
        }

        [Fact]
        public async Task AddUserAsync_ShouldNotAddUser_AddUserWithExistingUsername()
        {
            // Arrange  
            await _dataContext.Users.AddAsync(new()
            {
                Email = "test@gmail.com",
                Password = "fORRNGKa8yYj1WaiuGE0728PF1fkz+NDVB10/sXLmcY=",
                Username = "rk_world",
                FirstName = "Rajesh",
                LastName = "Vishwakarma",
                DateOfBirth = "",
                Mobile = "906905690",
                Address = "Road no. 8B, Rajeev Nagar",
                City = "Patna",
                State = "Bihar",
                Country = "India",
                IsActive = true,
                CreatedDate = DateTime.Now
            });

            await _dataContext.SaveChangesAsync();

            AddUpdateUserModel model = new()
            {
                Email = "rajesh@gmail.com",
                Password = "password",
                Username = "rk_world",
                FirstName = "Rajesh",
                LastName = "Vishwakarma",
                DateOfBirth = "",
                Mobile = "906905690",
                Address = "Road no. 8B, Rajeev Nagar",
                City = "Patna",
                State = "Bihar",
                Country = "India"
            };

            // Act  
            Func<Task> result = async () => await _userService.AddUserAsync(model);

            //Assert  
            RaiseError error = await Assert.ThrowsAsync<RaiseError>(result);

            error.Should().NotBeNull();
            ExceptionDetail? errorDetail = JsonConvert.DeserializeObject<ExceptionDetail>(error.Message);
            errorDetail.Should().NotBeNull();
            errorDetail?.Code.Should().Be("425");
            errorDetail?.Message.Should().Be("Username already exists.");
        }

        [Fact]
        public async Task UpdateUserAsync_ShouldUpdateUser_UpdateUserWithValidData()
        {
            // Arrange  
            UserData user = new()
            {
                Email = "test@gmail.com",
                Password = "fORRNGKa8yYj1WaiuGE0728PF1fkz+NDVB10/sXLmcY=",
                Username = "rk_world",
                FirstName = "Rajesh",
                LastName = "Vishwakarma",
                DateOfBirth = "",
                Mobile = "906905690",
                Address = "Road no. 8B, Rajeev Nagar",
                City = "Patna",
                State = "Bihar",
                Country = "India",
                IsActive = true,
                CreatedDate = DateTime.Now
            };

            await _dataContext.Users.AddAsync(user);

            await _dataContext.SaveChangesAsync();

            AddUpdateUserModel model = new()
            {
                Email = "rajesh@gmail.com",
                Username = "rk_world",
                FirstName = "Rajesh",
                LastName = "Vishwakarma",
                DateOfBirth = "",
                Mobile = "906905690",
                Address = "Road no. 8B, Rajeev Nagar",
                City = "Patna",
                State = "Bihar",
                Country = "India"
            };

            // Act  
            CommonDto<UserDto> result = await _userService.UpdateUserAsync(user.Id, model);

            //Assert  
            result.Should().NotBeNull();
            result.IsSuccess.Should().BeTrue();
            result.Response.Should().NotBeNull();
            result.Response?.Email.Should().BeEquivalentTo(model.Email);
        }

        [Fact]
        public async Task UpdateUserAsync_ShouldNotUpdateUser_UpdateUserWithInValidData()
        {
            // Arrange  
            AddUpdateUserModel model = new()
            {
                Email = "rajesh@gmail.com",
                Username = "rk_world",
                FirstName = "Rajesh",
                LastName = "Vishwakarma",
                DateOfBirth = "",
                Mobile = "906905690",
                Address = "Road no. 8B, Rajeev Nagar",
                City = "Patna",
                State = "Bihar",
                Country = "India"
            };

            // Act  
            Func<Task> result = async () => await _userService.UpdateUserAsync(Guid.NewGuid(), model);

            //Assert  
            RaiseError error = await Assert.ThrowsAsync<RaiseError>(result);

            error.Should().NotBeNull();
            ExceptionDetail? errorDetail = JsonConvert.DeserializeObject<ExceptionDetail>(error.Message);
            errorDetail.Should().NotBeNull();
            errorDetail?.Code.Should().Be("426");
            errorDetail?.Message.Should().Be("User doesn't exists.");
        }

        [Fact]
        public async Task FindUserByIdAsync_ShouldReturnUser_ReturnUserDataWithValidUserId()
        {
            // Arrange  
            UserData user = new()
            {
                Email = "test@gmail.com",
                Password = "fORRNGKa8yYj1WaiuGE0728PF1fkz+NDVB10/sXLmcY=",
                Username = "rk_world",
                FirstName = "Rajesh",
                LastName = "Vishwakarma",
                DateOfBirth = "",
                Mobile = "906905690",
                Address = "Road no. 8B, Rajeev Nagar",
                City = "Patna",
                State = "Bihar",
                Country = "India",
                IsActive = true,
                CreatedDate = DateTime.Now
            };

            await _dataContext.Users.AddAsync(user);

            await _dataContext.SaveChangesAsync();

            // Act  
            CommonDto<UserDto> result = await _userService.FindUserByIdAsync(user.Id);

            //Assert  
            result.Should().NotBeNull();
            result.IsSuccess.Should().BeTrue();
            result.Response.Should().NotBeNull();
            result.Response?.Email.Should().BeEquivalentTo(user.Email);
            result.Response?.Username.Should().BeEquivalentTo(user.Username);
        }

        [Fact]
        public async Task FindUserByIdAsync_ShouldNotReturnUser_ThrowErrorWithInValidUserId()
        {
            // Arrange  
            Guid userId = Guid.NewGuid();

            // Act  
            Func<Task> result = async () => await _userService.FindUserByIdAsync(userId);

            //Assert  
            RaiseError error = await Assert.ThrowsAsync<RaiseError>(result);

            error.Should().NotBeNull();
            ExceptionDetail? errorDetail = JsonConvert.DeserializeObject<ExceptionDetail>(error.Message);
            errorDetail.Should().NotBeNull();
            errorDetail?.Code.Should().Be("426");
            errorDetail?.Message.Should().Be("User doesn't exists.");
        }

        [Fact]
        public async Task DeleteUserByIdAsync_ShouldDeleteUserData_DeleteUserWithValidUserId()
        {
            // Arrange  
            UserData? user = new()
            {
                Email = "test@gmail.com",
                Password = "fORRNGKa8yYj1WaiuGE0728PF1fkz+NDVB10/sXLmcY=",
                Username = "rk_world",
                FirstName = "Rajesh",
                LastName = "Vishwakarma",
                DateOfBirth = "",
                Mobile = "906905690",
                Address = "Road no. 8B, Rajeev Nagar",
                City = "Patna",
                State = "Bihar",
                Country = "India",
                IsActive = true,
                CreatedDate = DateTime.Now
            };

            await _dataContext.Users.AddAsync(user);

            await _dataContext.SaveChangesAsync();

            // Act  
            CommonDto<Guid> result = await _userService.DeleteUserByIdAsync(user.Id);

            user = await _dataContext.Users.FirstOrDefaultAsync(f => f.Id == user.Id && f.IsActive);

            //Assert  
            result.Should().NotBeNull();
            result.IsSuccess.Should().BeTrue();
            user.Should().BeNull();
        }

        [Fact]
        public async Task DeleteUserByIdAsync_ShouldNotDeleteUserData_ThrowErrorWithInValidUserId()
        {
            // Arrange  
            Guid userId = Guid.NewGuid();

            // Act  
            Func<Task> result = async () => await _userService.DeleteUserByIdAsync(userId);

            //Assert  
            RaiseError error = await Assert.ThrowsAsync<RaiseError>(result);

            error.Should().NotBeNull();
            ExceptionDetail? errorDetail = JsonConvert.DeserializeObject<ExceptionDetail>(error.Message);
            errorDetail.Should().NotBeNull();
            errorDetail?.Code.Should().Be("426");
            errorDetail?.Message.Should().Be("User doesn't exists.");
        }
    }
}
