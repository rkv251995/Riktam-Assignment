using Assignment.Api.Filters;
using Assignment.Dtos.Common;
using Assignment.Dtos.Group;
using Assignment.Models.Common;
using Assignment.Models.Group;
using Assignment.Models.Message;
using Assignment.Services.Group.Interface;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Assignment.Api.Controllers
{
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [TokenValidator]
    [Route("api/groups")]
    [ApiController]
    public class GroupController : ControllerBase
    {
        private readonly IGroupService _groupService;

        public GroupController(IGroupService groupService)
        {
            _groupService = groupService;
        }


        [HttpPost]
        [ProducesResponseType(typeof(CommonDto<GroupDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorDetail), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ExceptionDetail), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> CreateGroupAsync(AddUpdateGroupModel model) => Ok(await _groupService.CreateGroupAsync(model, new BaseModel().LoginUserId ?? string.Empty));

        [HttpPut("{groupId:guid}")]
        [ProducesResponseType(typeof(CommonDto<GroupDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorDetail), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ExceptionDetail), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> UpdateGroupAsync(Guid groupId, AddUpdateGroupModel model) => Ok(await _groupService.UpdateGroupAsync(groupId, model, new BaseModel().LoginUserId ?? string.Empty));

        [HttpGet("{groupId:guid}")]
        [ProducesResponseType(typeof(CommonDto<GroupDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorDetail), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ExceptionDetail), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> FindGroupByIdAsync(Guid groupId) => Ok(await _groupService.FindGroupByIdAsync(groupId));

        [HttpDelete("{groupId:guid}")]
        [ProducesResponseType(typeof(CommonDto<Guid>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorDetail), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ExceptionDetail), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> DeleteGroupByIdAsync(Guid groupId) => Ok(await _groupService.DeleteGroupByIdAsync(groupId, new(new BaseModel().LoginUserId ?? string.Empty)));

        [HttpPost("{groupId:guid}/join")]
        [ProducesResponseType(typeof(CommonDto<GroupDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorDetail), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ExceptionDetail), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> JoinGroupAsync(Guid groupId) => Ok(await _groupService.JoinGroupAsync(groupId, new(new BaseModel().LoginUserId ?? string.Empty)));

        [HttpPost("{groupId:guid}/leave")]
        [ProducesResponseType(typeof(CommonDto<GroupDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorDetail), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ExceptionDetail), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> LeaveGroupAsync(Guid groupId) => Ok(await _groupService.LeaveGroupAsync(groupId, new(new BaseModel().LoginUserId ?? string.Empty)));

        [HttpPost("{groupId:guid}/sent-message")]
        [ProducesResponseType(typeof(CommonDto<Guid>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorDetail), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ExceptionDetail), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> SentMessageAsync(Guid groupId, MessageModel model) => Ok(await _groupService.SentMessageAsync(groupId, model));

        [HttpDelete("{groupId:guid}/messages/{messageId:guid}")]
        [ProducesResponseType(typeof(CommonDto<Guid>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorDetail), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ExceptionDetail), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> DeleteMessageAsync(Guid groupId, Guid messageId) => Ok(await _groupService.DeleteMessageAsync(groupId, messageId, new(new BaseModel().LoginUserId ?? string.Empty)));

        [HttpGet("{groupId:guid}/messages")]
        [ProducesResponseType(typeof(CommonDto<GroupMessageDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorDetail), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ExceptionDetail), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> FindAllMessagesByGroupIdAsync(Guid groupId) => Ok(await _groupService.FindAllMessagesByGroupIdAsync(groupId));

        [HttpGet("{groupId:guid}/members")]
        [ProducesResponseType(typeof(CommonDto<GroupMemberDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorDetail), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ExceptionDetail), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> FindAllMembersByGroupIdAsync(Guid groupId) => Ok(await _groupService.FindAllMembersByGroupIdAsync(groupId));
    }
}
