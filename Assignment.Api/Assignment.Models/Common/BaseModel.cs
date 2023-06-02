using Assignment.Utilities.Constants;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace Assignment.Models.Common
{
    public class BaseModel
    {
        private readonly HttpContextAccessor _httpContextAccessor;
        public BaseModel()
        {
            _httpContextAccessor = new HttpContextAccessor();
        }
        public string? LoginUserId { get => _httpContextAccessor.HttpContext?.User.Claims.FirstOrDefault(f => f.Type == ClaimTypes.NameIdentifier)?.Value; }
        public string? TokenId { get => _httpContextAccessor.HttpContext?.User.Claims.FirstOrDefault(f => f.Type == ClaimsConstant.TokenId)?.Value; }
    }
}
