using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace PetProject
{
    public class ExceptionMiddleware
    {
        private readonly RequestDelegate _next;

        public ExceptionMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                await HandleExceptionAsync(context, ex);
            }
        }
        /// <summary>
        /// Hàm thực hiện kiểm soát lỗi
        /// </summary>
        /// <param name="context">http context </param>
        /// <param name="exception">exception trả về</param>
        /// <returns></returns>
        /// Author: PNNHai
        /// Date
        private async Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            Console.WriteLine(exception);
            context.Response.ContentType = "application/json";
            switch (exception)
            {
                case NotFoundException:
                    context.Response.StatusCode = StatusCodes.Status404NotFound;
                    await context.Response.WriteAsync(
                        text: new BaseNotifyException()
                        {
                            ErrorCode = ((NotFoundException)exception).ErrorCode,
                            UserMessage = exception.Message,
                            DevMessage = exception.Message,
                            TraceId = context.TraceIdentifier,
                            MoreInfo = exception.HelpLink,
                        }.ToString() ?? ""      // ToString là chuyển sang json (hàm được override bên base)
                    );
                    break;

                case ValidateException:
                    context.Response.StatusCode = StatusCodes.Status400BadRequest;
                    await context.Response.WriteAsync(
                        text: new BaseNotifyException()
                        {
                            ErrorCode = ((ValidateException)exception).ErrorCode,
                            UserMessage = exception.Message,
                            DevMessage = exception.Message,
                            TraceId = context.TraceIdentifier,
                            MoreInfo = exception.HelpLink,
                        }.ToString() ?? ""
                    );
                    break;

                case UnauthorizeException:
                    context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                    await context.Response.WriteAsync(
                        text: new BaseNotifyException()
                        {
                            ErrorCode = ((ValidateException)exception).ErrorCode,
                            UserMessage = exception.Message,
                            DevMessage = exception.Message,
                            TraceId = context.TraceIdentifier,
                            MoreInfo = exception.HelpLink,
                        }.ToString() ?? ""
                    );
                    break;

                case ForbiddenException:
                    context.Response.StatusCode = StatusCodes.Status403Forbidden;
                    await context.Response.WriteAsync(
                        text: new BaseNotifyException()
                        {
                            ErrorCode = ((ValidateException)exception).ErrorCode,
                            UserMessage = exception.Message,
                            DevMessage = exception.Message,
                            TraceId = context.TraceIdentifier,
                            MoreInfo = exception.HelpLink,
                        }.ToString() ?? ""
                    );
                    break;

                default:
                    context.Response.StatusCode = StatusCodes.Status500InternalServerError;
                    await context.Response.WriteAsync(
                        text: new BaseNotifyException()
                        {
                            ErrorCode = context.Response.StatusCode,
                            UserMessage = $"Có lỗi xảy ra. Vui lòng liên hệ để xử lý",
                            DevMessage = exception.Message,
                            TraceId = context.TraceIdentifier,
                            MoreInfo = exception.HelpLink,
                        }.ToString() ?? "");
                    break;
            }
        }
    }
}