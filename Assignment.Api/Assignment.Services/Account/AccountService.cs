using Assignment.Dtos.Account;
using Assignment.Infrastructures.EntityFrameworkCore;
using Assignment.Infrastructures.EntityFrameworkCore.Entity;
using Assignment.Models.Account;
using Assignment.Models.Common;
using Assignment.Services.Account.Interface;
using Assignment.Services.Configuration.Interface;
using Assignment.Services.Security.Interface;
using Assignment.Utilities.Constants;
using Assignment.Utilities.Helpers;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using UserData = Assignment.Infrastructures.EntityFrameworkCore.Entity.User;

namespace Assignment.Services.Account
{
    public class AccountService : BaseService, IAccountService
    {
        private readonly IJwtConfiguration _jwtConfiguration;
        private readonly ISecurityService _securityService;

        public AccountService(DataContext dataContext,
                              IJwtConfiguration jwtConfiguration,
                              ISecurityService securityService) : base(dataContext)
        {
            _jwtConfiguration = jwtConfiguration;
            _securityService = securityService;
        }

        public async Task<LoginDto> SignInAsync(LoginModel model)
        {
            UserData? user = await _dataContext.Users.FirstOrDefaultAsync(f => f.Email.ToLower().Equals(model.Email.ToLower()) && f.IsActive);

            if (user == null || !user.Password.Equals(_securityService.Encrypt(model.Password)))
                throw new RaiseError(new ExceptionDetail { Code = "421", Message = "Invalid Credentials." }.ToString());

            string refreshToken = await CreateRefreshTokenAsync();
            Guid tokenId = await SaveTokenAsync(user.Id, refreshToken);

            return new LoginDto
            {
                AccessToken = await CreateTokenAsync(new()
                {
                    TokenId = tokenId.ToString(),
                    UserId = user.Id.ToString(),
                    Username = user.Username ?? string.Empty,
                    Email = user.Email,
                    FirstName = user.FirstName ?? string.Empty,
                    LastName = user.LastName ?? string.Empty,
                }),
                RefreshToken = refreshToken
            };
        }

        public async Task<RefreshTokenDto> RefreshTokenAsync(RefreshTokenModel model)
        {
            string? userId = (await FindPrincipalAsync(model.AccessToken))?.Identities?.FirstOrDefault()?.Claims?.FirstOrDefault(f => f.Type == ClaimTypes.NameIdentifier)?.Value;
            string? username = (await FindPrincipalAsync(model.AccessToken))?.Identities?.FirstOrDefault()?.Claims?.FirstOrDefault(f => f.Type == ClaimsConstant.Username)?.Value;
            string? email = (await FindPrincipalAsync(model.AccessToken))?.Identities?.FirstOrDefault()?.Claims?.FirstOrDefault(f => f.Type == ClaimsConstant.Email)?.Value;
            string? firstName = (await FindPrincipalAsync(model.AccessToken))?.Identities?.FirstOrDefault()?.Claims?.FirstOrDefault(f => f.Type == ClaimsConstant.FirstName)?.Value;
            string? lastName = (await FindPrincipalAsync(model.AccessToken))?.Identities?.FirstOrDefault()?.Claims?.FirstOrDefault(f => f.Type == ClaimsConstant.LastName)?.Value;
            string? tokenId = (await FindPrincipalAsync(model.AccessToken))?.Identities?.FirstOrDefault()?.Claims?.FirstOrDefault(f => f.Type == ClaimsConstant.TokenId)?.Value;

            if (!string.IsNullOrEmpty(userId)
                && !string.IsNullOrEmpty(username)
                && !string.IsNullOrEmpty(email)
                && !string.IsNullOrEmpty(firstName)
                && !string.IsNullOrEmpty(lastName)
                && !string.IsNullOrEmpty(tokenId))
            {
                Guid guidTokenId = new(tokenId);
                TokenManager? tokenData = await _dataContext.TokenManagers.FirstOrDefaultAsync(f => f.Id== guidTokenId && f.IsActive);

                if (tokenData != null)
                {
                    if (tokenData.RefreshToken.Equals(model.RefreshToken))
                    {
                        if (tokenData.RefreshTokenExpiredOn >= DateTime.UtcNow)
                        {
                            tokenData.IsActive = false;
                            tokenData.UpdatedDate = DateTime.Now;
                            tokenData.UpdatedBy = new(userId);

                            _dataContext.TokenManagers.Update(tokenData);
                            await _dataContext.SaveChangesAsync();

                            string refreshToken = await CreateRefreshTokenAsync();
                            guidTokenId = await SaveTokenAsync(new(userId), refreshToken);

                            return new()
                            {
                                AccessToken = await CreateTokenAsync(new()
                                {
                                    TokenId = guidTokenId.ToString(),
                                    UserId = userId,
                                    Username = username ?? string.Empty,
                                    Email = email,
                                    FirstName = firstName ?? string.Empty,
                                    LastName = lastName ?? string.Empty,
                                }),
                                RefreshToken = refreshToken
                            };
                        }

                        throw new RaiseError(new ExceptionDetail { Code = "423", Message = "Refresh Token is expired." }.ToString());
                    }

                    throw new RaiseError(new ExceptionDetail { Code = "424", Message = "Invalid Refresh Token." }.ToString());
                }

                throw new RaiseError(new ExceptionDetail { Code = "422", Message = "Invalid Access Token." }.ToString());
            }

            throw new RaiseError(new ExceptionDetail { Code = "422", Message = "Invalid Access Token." }.ToString());
        }

        public async Task SignOutAsync(BaseModel model)
        {
            TokenManager? token = await _dataContext.TokenManagers.FirstOrDefaultAsync(w => w.Id == new Guid(model.TokenId ?? string.Empty) && w.IsActive);

            if (token == null)
                throw new RaiseError(new ExceptionDetail { Code = $"{(int)HttpStatusCode.Unauthorized}", Message = HttpStatusCode.Unauthorized.ToString() }.ToString());

            token.RefreshTokenExpiredOn = DateTime.Now;
            token.UpdatedDate = DateTime.Now;
            token.UpdatedBy = new(model.LoginUserId ?? string.Empty);
            token.IsActive = false;

            _dataContext.TokenManagers.Update(token);
            await _dataContext.SaveChangesAsync();
        }

        private async Task<string> CreateRefreshTokenAsync()
        {
            byte[] randomNumber = new byte[64];
            using RandomNumberGenerator? randomNumberGenerator = RandomNumberGenerator.Create();
            randomNumberGenerator.GetBytes(randomNumber);
            return await Task.FromResult(Convert.ToBase64String(randomNumber));
        }

        private async Task<Guid> SaveTokenAsync(Guid userId, string refreshToken)
        {
            TokenManager tokenManager = new()
            {
                UserId = userId,
                RefreshToken = refreshToken,
                RefreshTokenExpiredOn = DateTime.Now.AddDays(_jwtConfiguration.RefreshTokenExpiresInDay),
                CreatedBy = userId,
                IsActive = true,
                CreatedDate = DateTime.Now
            };

            await _dataContext.TokenManagers.AddAsync(tokenManager);
            await _dataContext.SaveChangesAsync();

            return tokenManager.Id;
        }

        private async Task<string> CreateTokenAsync(TokenModel model)
        {
            JwtSecurityTokenHandler tokenHandler = new();
            ClaimsIdentity claimsIdentity = new(new List<Claim>
            {
                new Claim(ClaimsConstant.TokenId, $"{model.TokenId}"),
                new Claim(ClaimTypes.NameIdentifier, $"{model.UserId}"),
                new Claim(ClaimsConstant.Username, $"{model.Username}"),
                new Claim(ClaimsConstant.Email, $"{model.Email}"),
                new Claim(ClaimsConstant.FirstName, $"{model.FirstName}"),
                new Claim(ClaimsConstant.LastName, $"{model.LastName}")
            });

            SigningCredentials signingCredentials = new(new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtConfiguration.SecurityKey)), SecurityAlgorithms.HmacSha256Signature);

            SecurityTokenDescriptor tokenDescriptor = new()
            {
                Subject = claimsIdentity,
                Issuer = _jwtConfiguration.Issuer,
                Audience = _jwtConfiguration.Audience,
                Expires = DateTime.Now.AddDays(_jwtConfiguration.ExpiresInDay),
                SigningCredentials = signingCredentials
            };

            SecurityToken token = tokenHandler.CreateToken(tokenDescriptor);
            return await Task.FromResult(tokenHandler.WriteToken(token));
        }

        private async Task<ClaimsPrincipal?> FindPrincipalAsync(string token)
        {
            try
            {
                TokenValidationParameters tokenValidationParameters = new()
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtConfiguration.SecurityKey)),
                    ValidateIssuer = true,
                    ValidIssuer = _jwtConfiguration.Issuer,
                    ValidateAudience = true,
                    ValidAudience = _jwtConfiguration.Audience,
                    RequireExpirationTime = true,
                    ClockSkew = TimeSpan.FromSeconds(0)
                };

                JwtSecurityTokenHandler tokenHandler = new JwtSecurityTokenHandler();
                ClaimsPrincipal principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out SecurityToken securityToken);

                if (securityToken is not JwtSecurityToken jwtSecurityToken || !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
                    throw new RaiseError(new ExceptionDetail { Code = "422", Message = "Invalid Access Token." }.ToString());

                return await Task.FromResult(principal);
            }
            catch
            {
                throw new RaiseError(new ExceptionDetail { Code = "422", Message = "Invalid Access Token." }.ToString());
            }
        }
    }
}
