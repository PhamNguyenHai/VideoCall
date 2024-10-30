using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PetProject.Services;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;
using Microsoft.AspNetCore.Mvc.Filters;

namespace PetProject.Controllers
{
    public abstract class BaseController : Controller
    {
        protected Guid UserId { get; private set; }

        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            base.OnActionExecuting(filterContext);
            UserId = GetUserIdFromCookie();
        }

        private Guid GetUserIdFromCookie()
        {
            var cookie = HttpContext.Request.Cookies["UserId"];
            if (cookie != null)
            {
                if (Guid.TryParse(cookie, out var userId))
                {
                    return userId;
                }
            }
            return Guid.Empty; // Nếu không tìm thấy cookie hoặc không thể phân tích cú pháp
        }
    }
}
