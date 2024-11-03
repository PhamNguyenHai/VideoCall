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
        private readonly IPrivateMessageService _privateMessageService;

        public ChatController(IUserService userService, IPrivateMessageService privateMessageService)
        {
            _userService = userService;
            _privateMessageService = privateMessageService;
        }

        // GET: ChatController
        [HttpGet]
        //[AuthorizeUser(false, UserRole.User)]
        [AuthorizeUser(true)]
        public async Task<IActionResult> Index()
        {
            return View();
        }

        [HttpGet]
        [AuthorizeUser(true)]
        public async Task<JsonResult> GetPrivateChats()
        {
            var privateChats = await _userService.GetPrivateChatsByUserId(UserId);
            return Json(privateChats);
        }

        [HttpGet]
        [AuthorizeUser(true)]
        public async Task<JsonResult> GetPrivateMessagesByPartnerId(Guid PartnerId)
        {
            var messages = await _userService.GetUserPrivateMessagesByUserIdAndPartnerId(UserId, PartnerId);
            return Json(messages);
        }

        [HttpPost]
        [AuthorizeUser(true)]
        public async Task<JsonResult> SendPrivateMessage(PrivateMessageCreateDto privateMessageCreate)
        {
            privateMessageCreate.SenderId = UserId;
            var result = await _privateMessageService.CreateAsync(privateMessageCreate);
            return Json(result);
        }
    }
}
