﻿using Microsoft.AspNetCore.Http;
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
        [AuthorizeUser(false, UserRole.User)]
        public async Task<IActionResult> Index()
        {
            var privateChats = await _userService.GetPrivateChatsByUserId(UserId);
            return View(privateChats);
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
