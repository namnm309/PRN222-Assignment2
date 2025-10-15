using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using BusinessLayer.Enums;

namespace PresentationLayer.Pages.Dashboard
{
    public class IndexModel : PageModel
    {
        public void OnGet()
        {
            // Lấy thông tin user từ session
            var roleString = HttpContext.Session.GetString("UserRole");
            var userName = HttpContext.Session.GetString("UserFullName");
            var userEmail = HttpContext.Session.GetString("UserEmail");

            // Kiểm tra đăng nhập
            if (string.IsNullOrEmpty(roleString) || string.IsNullOrEmpty(userName))
            {
                TempData["Error"] = "Vui lòng đăng nhập để truy cập Dashboard.";
                Response.Redirect("/Account/Login"); // Redirect to login page
                return;
            }

            // Parse role
            UserRole? currentUserRole = null;
            if (System.Enum.TryParse<UserRole>(roleString, out var role))
            {
                currentUserRole = role;
            }

            // Set ViewData for all views
            ViewData["UserRole"] = currentUserRole;
            ViewData["UserRoleName"] = currentUserRole?.ToString();
            ViewData["UserName"] = userName;
            ViewData["UserEmail"] = userEmail ?? "";
        }
    }
}


