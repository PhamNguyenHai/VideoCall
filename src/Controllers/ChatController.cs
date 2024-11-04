using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PetProject.Repositories;
using PetProject.Services;
using System;
using System.Threading.Tasks;

namespace PetProject.Controllers
{
    public class ChatController : BaseController
    {
        private readonly IUserService _userService; 
        private readonly IPrivateMessageService _privateMessageService;
        private readonly IPrivateChatService _privateChatService;

        public ChatController(IUserService userService, IPrivateMessageService privateMessageService, IPrivateChatService privateChatService)
        {
            _userService = userService;
            _privateMessageService = privateMessageService;
            _privateChatService = privateChatService;
        }

        // GET: ChatController
        [HttpGet]
        [AuthorizeUser(false, UserRole.User)]
        public async Task<IActionResult> Index()
        {
            return View();
        }

        [HttpGet]
        [AuthorizeUser(false, UserRole.User)]
        public async Task<JsonResult> GetPrivateChats()
        {
            var privateChats = await _userService.GetPrivateChatsByUserId(UserId);
            return Json(privateChats);
        }

        [HttpGet]
        [AuthorizeUser(false, UserRole.User)]
        public async Task<JsonResult> GetPrivateMessagesByPartnerId(Guid PartnerId)
        {
            var messages = await _userService.GetUserPrivateMessagesByUserIdAndPartnerId(UserId, PartnerId);
            return Json(messages);
        }

        [HttpPost]
        [AuthorizeUser(false, UserRole.User)]
        public async Task<JsonResult> SendPrivateMessage([FromBody]PrivateMessageCreateDto privateMessageCreate)
        {
            privateMessageCreate.SenderId = UserId;

            var privateChat = await _privateChatService.GetPrivateChatByUserIdAndPartnerId(privateMessageCreate.SenderId, privateMessageCreate.ReceiverId);
            if(privateChat == null)
            {
                var result = await _privateMessageService.CreateChatMessageAndSendMessageAsync(privateMessageCreate);
                return Json(result);
            }
            else
            {
                var result = await _privateMessageService.CreateAsync(privateChat.ChatId, privateMessageCreate);
                return Json(result);
            }
        }
    }
}
