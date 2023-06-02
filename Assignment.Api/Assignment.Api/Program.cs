using Assignment.Api.Extensions;
using Assignment.Models.Common;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.Filters;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers()
                .AddFluentValidation()
                .ConfigureApiBehaviorOptions(options =>
                {
                    options.InvalidModelStateResponseFactory = actionContext =>
                    {
                        ErrorDetail errors = new()
                        {
                            Code = StatusCodes.Status400BadRequest.ToString(),
                            Message = "Model Validation Failed."
                        };

                        foreach (string key in actionContext.ModelState.Keys)
                        {
                            KeyValuePair<string, ModelStateEntry?> errorData = actionContext.ModelState.FirstOrDefault(f => f.Key.Equals(key));

                            if (errorData.Value is not null)
                            {
                                ModelError? error = errorData.Value.Errors.FirstOrDefault();

                                if (error != null)
                                    errors.Errors.Add(new Error
                                    {
                                        Code = StatusCodes.Status400BadRequest.ToString(),
                                        Message = error.ErrorMessage,
                                        Property = errorData.Key
                                    });
                            }
                        }

                        return new BadRequestObjectResult(errors);
                    };
                });

builder.Services.AddServices(builder.Configuration);
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.CustomSchemaIds(i => i.FullName);
    options.SwaggerDoc("v1", new OpenApiInfo { Title = "Eve World Frontend Api", Version = "v1" });
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "JWT Authorization header using the Bearer scheme."
    });
    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[] {}
        }
    });
    options.OperationFilter<AppendAuthorizeToSummaryOperationFilter>();
});
builder.Services.AddRouting(options => options.LowercaseUrls = true);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.ConfigureExceptionHandler();
app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.Run();
