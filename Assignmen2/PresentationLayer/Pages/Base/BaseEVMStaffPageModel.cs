using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.RazorPages;
using BusinessLayer.Enums;

namespace PresentationLayer.Pages.Base
{
    /// <summary>
    /// Base Page Model cho tất cả pages của EVM Staff
    /// Tự động check authentication và authorization
    /// </summary>
    public class BaseEVMStaffPageModel : PageModel
    {
        public UserRole? CurrentUserRole { get; private set; }
        public string CurrentUserName { get; private set; } = string.Empty;
        public string CurrentUserEmail { get; private set; } = string.Empty;
        public Guid? CurrentUserId { get; private set; }

        public override void OnPageHandlerExecuting(PageHandlerExecutingContext context)
        {
            base.OnPageHandlerExecuting(context);

            // Lấy thông tin user từ session
            var roleString = HttpContext.Session.GetString("UserRole");
            var userName = HttpContext.Session.GetString("UserFullName");
            var userEmail = HttpContext.Session.GetString("UserEmail");
            var userIdString = HttpContext.Session.GetString("UserId");

            // ✅ KIỂM TRA ĐĂNG NHẬP
            if (string.IsNullOrEmpty(roleString) || string.IsNullOrEmpty(userName))
            {
                TempData["Error"] = "Vui lòng đăng nhập để truy cập trang này.";
                context.Result = RedirectToPage("/Account/Login", new { returnUrl = HttpContext.Request.Path });
                return;
            }

            // Parse role
            if (System.Enum.TryParse<UserRole>(roleString, out var role))
            {
                CurrentUserRole = role;
            }

            // ✅ KIỂM TRA QUYỀN - CHỈ EVMStaff và Admin mới được truy cập
            if (CurrentUserRole != UserRole.EVMStaff && CurrentUserRole != UserRole.Admin)
            {
                TempData["Error"] = "Bạn không có quyền truy cập trang này.";
                context.Result = RedirectToPage("/Dashboard/Index");
                return;
            }

            CurrentUserName = userName;
            CurrentUserEmail = userEmail ?? "";
            
            if (Guid.TryParse(userIdString, out var userId))
            {
                CurrentUserId = userId;
            }

            // Set ViewData cho views
            ViewData["UserRole"] = CurrentUserRole;
            ViewData["UserRoleName"] = CurrentUserRole.ToString();
            ViewData["UserName"] = CurrentUserName;
            ViewData["UserEmail"] = CurrentUserEmail;
        }

        /// <summary>
        /// Check xem user có phải là Admin không (full access)
        /// </summary>
        protected bool IsAdmin()
        {
            return CurrentUserRole == UserRole.Admin;
        }

        /// <summary>
        /// Check xem user có phải là EVM Staff không
        /// </summary>
        protected bool IsEVMStaff()
        {
            return CurrentUserRole == UserRole.EVMStaff || CurrentUserRole == UserRole.Admin;
        }
    }
}
