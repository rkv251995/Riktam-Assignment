using Assignment.Api.Filters;
using Assignment.Dtos.Common;
using Assignment.Dtos.User;
using Assignment.Models.Common;
using Assignment.Models.User;
using Assignment.Services.User.Interface;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Assignment.Api.Controllers
{
    [Route("api/users")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;

        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [TokenValidator]
        [HttpGet("{userId:guid}")]
        [ProducesResponseType(typeof(CommonDto<UserDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorDetail), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ExceptionDetail), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> FindUserByIdAsync(Guid userId) => Ok(await _userService.FindUserByIdAsync(userId));

        [AllowAnonymous]
        [HttpPost]
        [ProducesResponseType(typeof(CommonDto<UserDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorDetail), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ExceptionDetail), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> AddUserAsync(AddUpdateUserModel model) => Ok(await _userService.AddUserAsync(model));

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [TokenValidator]
        [HttpPut("{userId:guid}")]
        [ProducesResponseType(typeof(CommonDto<UserDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorDetail), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ExceptionDetail), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> UpdateUserAsync(Guid userId, AddUpdateUserModel model) => Ok(await _userService.UpdateUserAsync(userId, model));

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [TokenValidator]
        [HttpDelete("{userId:guid}")]
        [ProducesResponseType(typeof(CommonDto<Guid>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorDetail), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ExceptionDetail), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> DeleteUserByIdAsync(Guid userId) => Ok(await _userService.DeleteUserByIdAsync(userId));
    }
}
