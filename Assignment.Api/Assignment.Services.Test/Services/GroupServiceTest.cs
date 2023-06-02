using Assignment.Dtos.Common;
using Assignment.Dtos.Group;
using Assignment.Models.Common;
using Assignment.Models.Group;
using Assignment.Services.Group;
using Assignment.Services.Group.Interface;
using Assignment.Utilities.Helpers;
using FluentAssertions;
using Newtonsoft.Json;
using System.Net;
using Xunit;
using GroupData = Assignment.Infrastructures.EntityFrameworkCore.Entity.Group;

namespace Assignment.Services.Test.Services
{
    public class GroupServiceTest : BaseServiceTest
    {
        private readonly IGroupService _groupService;

        public GroupServiceTest()
        {
            _groupService = new GroupService(_dataContext);
        }

        [Fact]
        public async Task CreateGroupAsync_ShouldCreateGroup_CreateGroupWithValidData()
        {
            // Arrange  
            AddUpdateGroupModel model = new()
            {
                Name = "Test Group",
                Description = "It is Test Group."
            };

            // Act  
            CommonDto<GroupDto> result = await _groupService.CreateGroupAsync(model, Guid.NewGuid().ToString());

            //Assert  
            result.Should().NotBeNull();
            result.IsSuccess.Should().BeTrue();
            result.Response.Should().NotBeNull();
        }

        [Fact]
        public async Task UpdateGroupAsync_ShouldUpdateGroup_UpadteGroupWithValidData()
        {
            // Arrange  
            Guid loggedInUserId = Guid.NewGuid();

            GroupData group = new()
            {
                Name = "Test Group",
                Description = "This is Test Group",
                CreatedBy = loggedInUserId,
                CreatedDate = DateTime.Now,
                IsActive = true
            };

            await _dataContext.Groups.AddAsync(group);
            await _dataContext.SaveChangesAsync();

            AddUpdateGroupModel model = new()
            {
                Name = "Update Test Group",
                Description = "It is Test Group."
            };

            // Act  
            CommonDto<GroupDto> result = await _groupService.UpdateGroupAsync(group.Id, model, loggedInUserId.ToString());

            //Assert  
            result.Should().NotBeNull();
            result.IsSuccess.Should().BeTrue();
            result.Response.Should().NotBeNull();
            result.Response?.Name.Should().BeEquivalentTo(model.Name);
        }

        [Fact]
        public async Task UpdateGroupAsync_ShouldNotUpdateGroup_ThrowErrorWithInValidGroupId()
        {
            // Arrange  
            AddUpdateGroupModel model = new()
            {
                Name = "Update Test Group",
                Description = "It is Test Group."
            };

            // Act  
            Func<Task> result = async () => await _groupService.UpdateGroupAsync(Guid.NewGuid(), model, Guid.NewGuid().ToString());

            //Assert  
            RaiseError error = await Assert.ThrowsAsync<RaiseError>(result);

            error.Should().NotBeNull();
            ExceptionDetail? errorDetail = JsonConvert.DeserializeObject<ExceptionDetail>(error.Message);
            errorDetail.Should().NotBeNull();
            errorDetail?.Code.Should().Be("427");
            errorDetail?.Message.Should().Be("Group doesn't exists.");
        }

        [Fact]
        public async Task UpdateGroupAsync_ShouldNotUpdateGroup_ThrowErrorWithInValidLoggedInUserId()
        {
            // Arrange  
            GroupData group = new()
            {
                Name = "Test Group",
                Description = "This is Test Group",
                CreatedBy = Guid.NewGuid(),
                CreatedDate = DateTime.Now,
                IsActive = true
            };

            await _dataContext.Groups.AddAsync(group);
            await _dataContext.SaveChangesAsync();

            AddUpdateGroupModel model = new()
            {
                Name = "Update Test Group",
                Description = "It is Test Group."
            };

            // Act  
            Func<Task> result = async () => await _groupService.UpdateGroupAsync(group.Id, model, Guid.NewGuid().ToString());

            //Assert  
            RaiseError error = await Assert.ThrowsAsync<RaiseError>(result);

            error.Should().NotBeNull();
            ExceptionDetail? errorDetail = JsonConvert.DeserializeObject<ExceptionDetail>(error.Message);
            errorDetail.Should().NotBeNull();
            errorDetail?.Code.Should().Be(((int)HttpStatusCode.Forbidden).ToString());
            errorDetail?.Message.Should().Be(HttpStatusCode.Forbidden.ToString());
        }

        [Fact]
        public async Task FindGroupByIdAsync_ShouldReturnGroupData_ReturnDataWithValidGroupId()
        {
            // Arrange  
            Guid loggedInUserId = Guid.NewGuid();

            GroupData group = new()
            {
                Name = "Test Group",
                Description = "This is Test Group",
                CreatedBy = loggedInUserId,
                CreatedDate = DateTime.Now,
                IsActive = true
            };

            await _dataContext.Groups.AddAsync(group);
            await _dataContext.SaveChangesAsync();

            // Act  
            CommonDto<GroupDto> result = await _groupService.FindGroupByIdAsync(group.Id);

            //Assert  
            result.Should().NotBeNull();
            result.IsSuccess.Should().BeTrue();
            result.Response.Should().NotBeNull();
            result.Response?.Name.Should().BeEquivalentTo(group.Name);
        }

        [Fact]
        public async Task FindGroupByIdAsync_ShouldNotReturnGroupData_ThrowErrorWithInValidGroupId()
        {
            // Arrange  
            Guid groupId = Guid.NewGuid();

            // Act  
            Func<Task> result = async () => await _groupService.FindGroupByIdAsync(groupId);

            //Assert  
            RaiseError error = await Assert.ThrowsAsync<RaiseError>(result);

            error.Should().NotBeNull();
            ExceptionDetail? errorDetail = JsonConvert.DeserializeObject<ExceptionDetail>(error.Message);
            errorDetail.Should().NotBeNull();
            errorDetail?.Code.Should().Be("427");
            errorDetail?.Message.Should().Be("Group doesn't exists.");
        }
    }
}
