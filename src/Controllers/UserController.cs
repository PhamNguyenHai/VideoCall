using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
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
        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        //[ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginInfor loginInfor)
        {
            if (ModelState.IsValid)
            {
                var result = await _userService.LoginAsync(loginInfor);
                if (result.Success)
                {
                    // Lưu token vào cookie
                    Response.Cookies.Append("SessionToken", result.Data.ToString(), new CookieOptions
                    {
                        HttpOnly = true,
                        Expires = DateTimeOffset.UtcNow.AddHours(1)
                    });
                    return RedirectToAction("Register", "User"); // Chuyển hướng đến trang chính
                }
                ModelState.AddModelError(string.Empty, result.Message);
                ViewData["ErrorMessage"] = result.Message;
            }
            return View(loginInfor);
        }

        [HttpGet]
        public ActionResult Register()
        {
            return View();
        }

        public async Task<IActionResult> Logout()
        {
            var token = Request.Cookies["SessionToken"];
            await _userService.LogoutAsync(token);
            Response.Cookies.Delete("SessionToken");
            return RedirectToAction("Index", "Home"); // Chuyển hướng đến trang chính
        }

        // GET: UserController/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: UserController/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: UserController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: UserController/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: UserController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: UserController/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: UserController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }
    }
}
