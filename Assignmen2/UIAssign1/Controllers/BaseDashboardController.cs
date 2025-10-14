using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Authorization;
using BusinessLayer.Enums;

namespace PresentationLayer.Controllers
{
    public class BaseDashboardController : Controller
    {
        protected UserRole? CurrentUserRole { get; private set; }
        protected string CurrentUserName { get; private set; }
        protected string CurrentUserEmail { get; private set; }

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            base.OnActionExecuting(context);

            // Cho phép bỏ qua kiểm tra đăng nhập với các action có [AllowAnonymous]
            var isAnonymous = context.ActionDescriptor.EndpointMetadata
                .OfType<AllowAnonymousAttribute>()
                .Any();
            if (isAnonymous)
            {
                return;
            }

            // Lấy thông tin user từ session
            var roleString = HttpContext.Session.GetString("UserRole");
            var userName = HttpContext.Session.GetString("UserFullName");
            var userEmail = HttpContext.Session.GetString("UserEmail");

            // Kiểm tra đăng nhập
            if (string.IsNullOrEmpty(roleString) || string.IsNullOrEmpty(userName))
            {
                TempData["Error"] = "Vui lòng đăng nhập để truy cập Dashboard.";
                context.Result = RedirectToAction("Login", "Account");
                return;
            }

            // Parse role
            if (System.Enum.TryParse<UserRole>(roleString, out var role))
            {
                CurrentUserRole = role;
            }

            CurrentUserName = userName;
            CurrentUserEmail = userEmail ?? "";

            // Set ViewBag cho tất cả views
            ViewBag.UserRole = CurrentUserRole;
            ViewBag.UserRoleName = CurrentUserRole.ToString();
            ViewBag.UserName = CurrentUserName;
            ViewBag.UserEmail = CurrentUserEmail;
        }

        protected bool IsDealer()
        {
            return CurrentUserRole == UserRole.DealerStaff || CurrentUserRole == UserRole.DealerManager;
        }

        protected bool IsAdmin()
        {
            return CurrentUserRole == UserRole.Admin || CurrentUserRole == UserRole.EVMStaff;
        }
    }
}

