using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System;
using Microsoft.Extensions.DependencyInjection;
using PetProject.Repositories;

namespace PetProject
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, Inherited = true, AllowMultiple = false)]
    public class AuthorizeUserAttribute : Attribute, IAuthorizationFilter
    {
        private readonly UserRole[] _roles;
        private readonly bool _allowAnonymous;

        public AuthorizeUserAttribute(bool allowAnonymous = false, params UserRole[] roles)
        {
            _allowAnonymous = allowAnonymous;
            _roles = roles;
        }

        public async void OnAuthorization(AuthorizationFilterContext context)
        {
            if(_allowAnonymous)
            {
                return;
            }
            // Lấy token từ cookie
            var token = context.HttpContext.Request.Cookies["SessionToken"];

            // Nếu ko có token trong cookie => đẩy đến trang đăng nhập
            if (string.IsNullOrEmpty(token))
            {
                context.Result = new RedirectToActionResult("Login", "User", null);
            }
            else
            {
                // Lấy IUserRepository từ IServiceProvider
                var userRepository = context.HttpContext.RequestServices.GetService<IUserRepository>();

                var user = await userRepository.FindUserByToken(token);
                if (user == null)
                {
                    context.Result = new RedirectToActionResult("Login", "User", null);
                }
                else
                {
                    // Kiểm tra vai trò nếu có
                    if (_roles.Length > 0)
                    {
                        if (!_roles.Contains(user.Role))
                        {
                            // Nếu không có quyền, trả về Forbidden
                            context.Result = new ForbidResult();
                        }
                    }
                }
            }
        }
    }
}
