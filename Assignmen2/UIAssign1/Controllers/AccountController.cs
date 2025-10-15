using Microsoft.AspNetCore.Mvc;
using BusinessLayer.Services;
using BusinessLayer.ViewModels;

namespace PresentationLayer.Controllers
{
    public class AccountController : Controller
    {
        //gọi từ BusinessLayer để sử dụng 
        private readonly IAuthenService _authenService;

        //constructor
        public AccountController(IAuthenService authenService)
        {
            _authenService = authenService;
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View(new LoginViewModel());
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var result = await _authenService.LoginAsync(model.Email, model.Password);
            if (!result.Success)
            {
                ModelState.AddModelError(string.Empty, result.Error);
                return View(model);
            }

            // Lưu thông tin user vào session
            HttpContext.Session.SetString("UserId", result.User.Id.ToString());
            HttpContext.Session.SetString("UserFullName", result.User.FullName);
            HttpContext.Session.SetString("UserEmail", result.User.Email);
            HttpContext.Session.SetString("UserRole", result.User.Role.ToString());
            
            // Debug log để kiểm tra
            Console.WriteLine($"[DEBUG] User logged in: {result.User.Email}, Role: {result.User.Role}, DealerId: {result.User.DealerId}");
            
            // Lưu DealerId nếu user là Dealer Manager/Staff
            if (result.User.Role == DataAccessLayer.Enum.UserRole.DealerManager || 
                result.User.Role == DataAccessLayer.Enum.UserRole.DealerStaff)
            {
                if (result.User.DealerId.HasValue)
                {
                    HttpContext.Session.SetString("DealerId", result.User.DealerId.Value.ToString());
                    Console.WriteLine($"[DEBUG] DealerId saved to session: {result.User.DealerId.Value}");
                }
                else
                {
                    Console.WriteLine($"[DEBUG] WARNING: Dealer user has no DealerId assigned!");
                }
            }
            
            TempData["LoginMessage"] = "Đăng nhập thành công";
            
            // Redirect vào Dashboard thay vì Homepage
            return RedirectToAction("Index", "Dashboard");
        }


        [HttpPost]
        public IActionResult Logout()
        {
            HttpContext.Session.Remove("UserId");
            HttpContext.Session.Remove("UserFullName");
            HttpContext.Session.Remove("UserEmail");
            HttpContext.Session.Remove("UserRole");
            HttpContext.Session.Remove("DealerId");
            HttpContext.Session.Clear();
            return RedirectToAction("Index", "Home");
        }
    }
}
