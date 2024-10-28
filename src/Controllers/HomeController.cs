using Microsoft.AspNetCore.Mvc;

namespace PetProject.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Error()
        {
            // Truyền thông tin lỗi vào ViewBag nếu cần
            ViewBag.ErrorMessage = "Có lỗi xảy ra. Vui lòng liên hệ để xử lý.";
            return View();
        }
    }
}
