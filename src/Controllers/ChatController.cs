using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PetProject.Services;
using System;
using System.Threading.Tasks;

namespace PetProject.Controllers
{
    public class ChatController : BaseController
    {
        private readonly IUserService _userService;

        public ChatController(IUserService userService)
        {
            _userService = userService;
        }

        // GET: ChatController
        [HttpGet]
        //[AuthorizeUser(false, UserRole.User)]
        [AuthorizeUser(true)]
        public async Task<IActionResult> Index()
        {
            //var privateChats = await _userService.GetPrivateChatsByUserId(UserId);
            var privateChats = await _userService.GetUserPrivateMessagesByUserIdAndPartnerId(Guid.Parse("2DBBF9A7-A5B6-43CD-8DE8-1143C8E75CB2"), Guid.Parse("FFC866A3-295D-411B-B0F3-87A1E7C9258B"));
            return View(privateChats);
        }

        [HttpGet]
        [AuthorizeUser(true)]
        public async Task<JsonResult> GetPrivateMessagesByPartnerId(Guid PartnerId)
        {
            var messages = await _userService.GetUserPrivateMessagesByUserIdAndPartnerId(UserId, PartnerId);
            return Json(messages);
        }

        // GET: ChatController/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: ChatController/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: ChatController/Create
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

        // GET: ChatController/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: ChatController/Edit/5
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

        // GET: ChatController/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: ChatController/Delete/5
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
