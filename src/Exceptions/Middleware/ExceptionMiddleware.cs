using Microsoft.AspNetCore.Http;
using PetProject.Repositories;
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
        private readonly ILoggerCustom _logger;

        public ExceptionMiddleware(RequestDelegate next, ILoggerCustom logger)
        {
            _next = next;
            _logger = logger;
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
            // Ghi log chi tiết
            _logger.LogError($"Lỗi hệ thống: {exception.Message}", exception);

            // Kết thúc pipeline
            context.Response.StatusCode = StatusCodes.Status500InternalServerError;
            context.Response.ContentType = "text/html";

            // Chuyển hướng đến trang lỗi
            context.Response.Redirect("/Home/Error");

            // Kết thúc pipeline
            await Task.CompletedTask;
        }
    }
}