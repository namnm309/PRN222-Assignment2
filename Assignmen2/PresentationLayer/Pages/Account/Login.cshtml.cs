using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using BusinessLayer.Services;
using BusinessLayer.DTOs.Requests;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using BusinessLayer.Enums;

namespace PresentationLayer.Pages.Account
{
    public class LoginModel : PageModel
    {
        private readonly IAuthenService _authenService;

        public LoginModel(IAuthenService authenService)
        {
            _authenService = authenService;
        }

        [BindProperty]
        public LoginRequest Input { get; set; } = new LoginRequest();

        public void OnGet()
        {
            // Clear any existing session data related to user on login page load
            HttpContext.Session.Clear();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            var result = await _authenService.LoginAsync(Input.Email, Input.Password);
            if (!result.Success)
            {
                ModelState.AddModelError(string.Empty, result.Error);
                return Page();
            }

            // Lưu thông tin user vào session
            HttpContext.Session.SetString("UserId", result.User.Id.ToString());
            HttpContext.Session.SetString("UserFullName", result.User.FullName);
            HttpContext.Session.SetString("UserEmail", result.User.Email);
            HttpContext.Session.SetString("UserRole", result.User.Role.ToString());

            // Lưu DealerId nếu user là Dealer Manager/Staff
            if (result.User.Role == DataAccessLayer.Enum.UserRole.DealerManager ||
                result.User.Role == DataAccessLayer.Enum.UserRole.DealerStaff)
            {
                if (result.User.DealerId.HasValue)
                {
                    HttpContext.Session.SetString("DealerId", result.User.DealerId.Value.ToString());
                }
            }

            TempData["LoginMessage"] = "Đăng nhập thành công";

            // Redirect dựa trên role
            if (result.User.Role == DataAccessLayer.Enum.UserRole.EVMStaff)
            {
                return RedirectToPage("/EVMStaff/Dashboard/Index");
            }
            else if (result.User.Role == DataAccessLayer.Enum.UserRole.Admin)
            {
                return RedirectToPage("/EVMStaff/Dashboard/Index"); // Admin cũng có thể truy cập EVM Staff Dashboard
            }
            else if (result.User.Role == DataAccessLayer.Enum.UserRole.DealerStaff)
            {
                return RedirectToPage("/DealerStaff/Dashboard");
            }
            else
            {
                return RedirectToPage("/Dashboard/Index");
            }
        }
    }
}


