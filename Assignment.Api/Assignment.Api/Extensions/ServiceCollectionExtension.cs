using Assignment.Infrastructures.EntityFrameworkCore;
using Assignment.Models.Account;
using Assignment.Models.Account.Validators;
using Assignment.Models.Common;
using Assignment.Models.Group;
using Assignment.Models.Group.Validators;
using Assignment.Models.Message;
using Assignment.Models.Message.Validators;
using Assignment.Models.User;
using Assignment.Models.User.Validators;
using Assignment.Services.Account;
using Assignment.Services.Account.Interface;
using Assignment.Services.Configuration;
using Assignment.Services.Configuration.Interface;
using Assignment.Services.Group;
using Assignment.Services.Group.Interface;
using Assignment.Services.Security;
using Assignment.Services.Security.Interface;
using Assignment.Services.User;
using Assignment.Services.User.Interface;
using Assignment.Utilities.Constants;
using FluentValidation;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Net;
using System.Text;

namespace Assignment.Api.Extensions
{
    public static class ServiceCollectionExtension
    {
        public static void AddServices(this IServiceCollection services, ConfigurationManager configuration)
        {
            services.AddDbContext<DataContext>(options => options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));
            services.AddHttpContextAccessor();

            IJwtConfiguration jwtconfiguration = configuration.GetSection(AppConstant.JwtConfiguration).Get<JwtConfiguration>();

            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(options =>
            {
                options.RequireHttpsMetadata = false;
                options.SaveToken = true;
                options.TokenValidationParameters = new TokenValidationParameters()
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtconfiguration.SecurityKey)),
                    ValidateIssuer = true,
                    ValidIssuer = jwtconfiguration.Issuer,
                    ValidateAudience = true,
                    ValidAudience = jwtconfiguration.Audience,
                    RequireExpirationTime = true,
                    ClockSkew = TimeSpan.FromSeconds(0)
                };
                options.Events = new JwtBearerEvents()
                {
                    OnChallenge = context =>
                    {
                        context.HandleResponse();
                        context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                        context.Response.ContentType = "application/json";

                        if (string.IsNullOrEmpty(context.Error))
                            context.Error = "invalid_token";
                        if (string.IsNullOrEmpty(context.ErrorDescription))
                            context.ErrorDescription = "This request requires a valid JWT access token to be provided.";

                        if (context.AuthenticateFailure != null && context.AuthenticateFailure.GetType() == typeof(SecurityTokenExpiredException))
                        {
                            SecurityTokenExpiredException? authenticationException = context.AuthenticateFailure as SecurityTokenExpiredException;
                            context.Response.Headers.Add("x-token-expired", authenticationException?.Expires.ToString("o"));
                            context.ErrorDescription = $"The token expired on {authenticationException?.Expires:o}";
                        }

                        return context.Response.WriteAsync(new ExceptionDetail
                        {
                            Code = $"{(int)HttpStatusCode.Unauthorized}",
                            Message = HttpStatusCode.Unauthorized.ToString()
                        }.ToString());
                    }
                };
            });

            //Add CORS
            services.AddCors(options =>
            {
                options.AddDefaultPolicy(
                    policy =>
                    {
                        policy.AllowAnyOrigin()
                              .AllowAnyHeader()
                              .AllowAnyMethod();
                    });
            });

            //Register Services
            services.AddTransient<IAccountService, AccountService>();
            services.AddTransient<IUserService, UserService>();
            services.AddTransient<IGroupService, GroupService>();
            services.AddTransient<ISecurityService, SecurityService>();
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

            //Register Validators
            services.AddTransient<IValidator<LoginModel>, LoginModelValidator>();
            services.AddTransient<IValidator<RefreshTokenModel>, RefreshTokenModelValidator>();
            services.AddTransient<IValidator<AddUpdateGroupModel>, AddUpdateGroupModelValidator>();
            services.AddTransient<IValidator<MessageModel>, MessageModelValidator>();
            services.AddTransient<IValidator<AddUpdateUserModel>, AddUpdateUserModelValidator>();

            //Read appSetting.json file
            services.AddSingleton<IJwtConfiguration>(configuration.GetSection(AppConstant.JwtConfiguration).Get<JwtConfiguration>());
        }
    }
}
