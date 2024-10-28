using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using PetProject.Models;
using PetProject.Services;
using System;
using System.Threading.Tasks;

namespace PetProject.Controllers
{
    public class UserController : Controller
    {
        protected readonly IUserService _userService;

        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        // GET: UserController
        public ActionResult Index()
        {
            return View();
        }

        [HttpGet]
        [AuthorizeUser(true)]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [AuthorizeUser(true)]
        //[ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginInfor loginInfor)
        {
            if (ModelState.IsValid)
            {
                var result = await _userService.LoginAsync(loginInfor);
                if (result.Success)
                {
                    if (result.Data is Session session)
                    {
                        // Lưu token vào cookie
                        Response.Cookies.Append("SessionToken", session.Token.ToString(), new CookieOptions
                        {
                            HttpOnly = true,
                            Expires = DateTimeOffset.UtcNow.AddHours(1)
                        });
                        return RedirectToAction("Profile", "User", new { id = session.UserId }); // Chuyển hướng đến trang chính
                    }
                    else
                    {
                        ViewData["ErrorMessage"] = "Có lỗi xảy ra";
                    }
                }
                ModelState.AddModelError(string.Empty, result.Message);
                ViewData["ErrorMessage"] = result.Message;
            }
            return View(loginInfor);
        }

        [HttpGet]
        [AuthorizeUser(true)]
        public ActionResult Register()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [AuthorizeUser(true)]
        public async Task<IActionResult> Register(UserCreateDto userToCreate)
        {
            if (ModelState.IsValid)
            {
                userToCreate.Role = UserRole.User;
                var result = await _userService.CreateAsync(userToCreate);
                if (result.Success)
                {
                    ViewData["ErrorMessage"] = "Đăng ký thành công";
                    return RedirectToAction("Login", "User");
                }
                ModelState.AddModelError(string.Empty, result.Message);
                ViewData["ErrorMessage"] = result.Message;
            }
            return View(userToCreate);
        }

        [AuthorizeUser(false, UserRole.User)]
        public async Task<IActionResult> Logout()
        {
            var token = Request.Cookies["SessionToken"];
            await _userService.LogoutAsync(token);
            Response.Cookies.Delete("SessionToken");
            return RedirectToAction("Login", "User"); // Chuyển hướng đến trang chính
        }

        [HttpGet]
        [AuthorizeUser(false, UserRole.User)]
        public async Task<IActionResult> Profile(Guid id)
        {
            var result = await _userService.GetByIdAsync(id);
            return View(result);
        }
    }
}
