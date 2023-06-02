using Assignment.Models.Common;
using Assignment.Utilities.Helpers;
using Microsoft.AspNetCore.Diagnostics;
using Newtonsoft.Json;
using System.Net;

namespace Assignment.Api.Extensions
{
    public static class ExceptionMiddlewareExtension
    {
        public static void ConfigureExceptionHandler(this IApplicationBuilder app)
        {
            app.UseExceptionHandler(configure =>
            {
                configure.Run(async context =>
                {
                    context.Response.ContentType = "application/json";
                    IExceptionHandlerFeature? contextFeature = context.Features.Get<IExceptionHandlerFeature>();
                    if (contextFeature is not null)
                    {
                        string response = string.Empty;
                        ExceptionDetail? exceptionDetail = null;
                        try
                        {
                            exceptionDetail = JsonConvert.DeserializeObject<ExceptionDetail>(contextFeature.Error.Message);
                        }
                        catch
                        {
                            exceptionDetail = null;
                        }

                        if (contextFeature.Error.GetType() == typeof(RaiseError)
                            && exceptionDetail is not null
                            && int.TryParse(exceptionDetail.Code, out int code))
                        {
                            context.Response.StatusCode = code;
                            response = new ExceptionDetail()
                            {
                                Code = code.ToString(),
                                Message = exceptionDetail.Message
                            }.ToString();
                        }
                        else
                        {
                            context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                            response = new ExceptionDetail()
                            {
                                Code = ((int)HttpStatusCode.InternalServerError).ToString(),
                                Message = HttpStatusCode.InternalServerError.ToString()
                            }.ToString();
                        }

                        await context.Response.WriteAsync(response);
                    }
                });
            });
        }
    }
}
