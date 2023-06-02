using Assignment.Api.Filters;
using Assignment.Dtos.Account;
using Assignment.Models.Account;
using Assignment.Models.Common;
using Assignment.Services.Account.Interface;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Assignment.Api.Controllers
{
    [ApiController]
    [Route("api/accounts")]
    public class AccountController : ControllerBase
    {
        private readonly IAccountService _accountService;

        public AccountController(IAccountService accountService)
        {
            _accountService = accountService;
        }

        [HttpPost("login")]
        [AllowAnonymous]
        [ProducesResponseType(typeof(LoginDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorDetail), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ExceptionDetail), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> SignInAsync(LoginModel model) => Ok(await _accountService.SignInAsync(model));


        [HttpPost("refresh-token")]
        [AllowAnonymous]
        [ProducesResponseType(typeof(RefreshTokenDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorDetail), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ExceptionDetail), 458)]
        [ProducesResponseType(typeof(ExceptionDetail), 459)]
        [ProducesResponseType(typeof(ExceptionDetail), 460)]
        [ProducesResponseType(typeof(ExceptionDetail), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> RefreshTokenAsync(RefreshTokenModel model) => Ok(await _accountService.RefreshTokenAsync(model));


        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [TokenValidator]
        [HttpPost("logout")]
        [ProducesResponseType(typeof(void), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ExceptionDetail), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(void), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(void), StatusCodes.Status405MethodNotAllowed)]
        [ProducesResponseType(typeof(ExceptionDetail), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> SignOutAsync()
        {
            await _accountService.SignOutAsync(new());
            return Ok();
        }
    }
}