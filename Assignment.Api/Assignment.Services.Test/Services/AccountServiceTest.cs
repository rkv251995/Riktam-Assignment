using Assignment.Dtos.Account;
using Assignment.Models.Account;
using Assignment.Models.Common;
using Assignment.Services.Account;
using Assignment.Services.Account.Interface;
using Assignment.Services.Configuration;
using Assignment.Services.Configuration.Interface;
using Assignment.Services.Security.Interface;
using Assignment.Utilities.Constants;
using Assignment.Utilities.Helpers;
using FluentAssertions;
using Microsoft.Extensions.Configuration;
using Moq;
using Newtonsoft.Json;
using Xunit;

namespace Assignment.Services.Test.Services
{
    public class AccountServiceTest : BaseServiceTest
    {
        private readonly IAccountService _accountService;
        private readonly Mock<ISecurityService> _mockSecurityService;

        public AccountServiceTest()
        {
            IConfigurationRoot configuration = new ConfigurationBuilder().AddJsonFile("appsettings.Development.json", false, true).Build();
            IJwtConfiguration jwtConfiguration = configuration.GetSection(AppConstant.JwtConfiguration).Get<JwtConfiguration>();
            _mockSecurityService = new Mock<ISecurityService>();

            _accountService = new AccountService(_dataContext, jwtConfiguration, _mockSecurityService.Object);
        }

        [Fact]
        public async Task SignInAsync_ShouldBeAbleToSignIn_SignInWithValidData()
        {
            // Arrange  
            LoginModel model = new()
            {
                Email = "rajesh@gmail.com",
                Password = "password"
            };

            _mockSecurityService.Setup(s => s.Encrypt(It.IsAny<string>())).Returns(() => "fORRNGKa8yYj1WaiuGE0728PF1fkz+NDVB10/sXLmcY=");

            await _dataContext.Users.AddAsync(new()
            {
                Email = model.Email,
                Password = _mockSecurityService.Object.Encrypt(model.Password),
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

            // Act  
            LoginDto result = await _accountService.SignInAsync(model);

            //Assert  
            result.Should().NotBeNull();
            result.AccessToken.Should().NotBeNullOrEmpty();
            result.RefreshToken.Should().NotBeNullOrEmpty();
        }

        [Fact]
        public async Task SignInAsync_ShouldBeNotAbleToSignIn_SignInWithInValidData()
        {
            // Arrange  
            LoginModel model = new()
            {
                Email = "rajesh@gmail.com",
                Password = "password"
            };

            _mockSecurityService.Setup(s => s.Encrypt(It.IsAny<string>())).Returns(() => "fORRNGKa8yYj1WaiuGE0728PF1fkz+NDVB10/sXLmcY=");


            //Act
            Func<Task> result = async () => await _accountService.SignInAsync(model);

            //Assert
            RaiseError error = await Assert.ThrowsAsync<RaiseError>(result);

            error.Should().NotBeNull();
            ExceptionDetail? errorDetail = JsonConvert.DeserializeObject<ExceptionDetail>(error.Message);
            errorDetail.Should().NotBeNull();
            errorDetail?.Code.Should().Be("421");
            errorDetail?.Message.Should().Be("Invalid Credentials.");
        }
    }
}
