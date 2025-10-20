using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using System.Threading.Tasks;
using BusinessLayer.Services;
using BusinessLayer.DTOs.Requests;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
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
            var userRole = (UserRole)(int)result.User.Role;
            if (userRole == UserRole.DealerManager ||
                userRole == UserRole.DealerStaff)
            {
                if (result.User.DealerId.HasValue)
                {
                    HttpContext.Session.SetString("DealerId", result.User.DealerId.Value.ToString());
                }
            }

            TempData["LoginMessage"] = "Đăng nhập thành công";

            // Đăng nhập Cookie Authentication để [Authorize] hoạt động
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, result.User.Id.ToString()),
                new Claim(ClaimTypes.Name, result.User.FullName ?? result.User.Email),
                new Claim(ClaimTypes.Email, result.User.Email ?? string.Empty),
                new Claim(ClaimTypes.Role, result.User.Role.ToString())
            };

            if (result.User.DealerId.HasValue)
            {
                claims.Add(new Claim("DealerId", result.User.DealerId.Value.ToString()));
            }

            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var principal = new ClaimsPrincipal(identity);
            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                principal,
                new AuthenticationProperties
                {
                    IsPersistent = true,
                    ExpiresUtc = DateTimeOffset.UtcNow.AddHours(8)
                });

            // Redirect dựa trên role
            if (userRole == UserRole.EVMStaff)
            {
                return RedirectToPage("/EVMStaff/Dashboard/Index");
            }
            else if (userRole == UserRole.Admin)
            {
                return RedirectToPage("/EVMStaff/Dashboard/Index"); // Admin cũng có thể truy cập EVM Staff Dashboard
            }
            else if (userRole == UserRole.DealerStaff)
            {
                return RedirectToPage("/DealerStaff/Dashboard");
            }
            else
            {
                // Dealer role -> điều hướng về khu vực phù hợp
                if (userRole == UserRole.DealerManager)
                {
                    return RedirectToPage("/DealerManager/Index");
                }
                return RedirectToPage("/Dashboard/Index");
            }
        }
    }
}


