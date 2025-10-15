using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace PresentationLayer.Infrastructure
{
    public class DashboardAuthorizePageFilter : IPageFilter
    {
        public void OnPageHandlerExecuting(PageHandlerExecutingContext context)
        {
            // Chỉ áp dụng cho các trang trong thư mục Dashboard
            if (context.ActionDescriptor.RelativePath?.StartsWith("Pages/Dashboard/") == true)
            {
                var role = context.HttpContext.Session.GetString("UserRole");
                var userId = context.HttpContext.Session.GetString("UserId");

                if (string.IsNullOrEmpty(role) || string.IsNullOrEmpty(userId))
                {
                    context.Result = new RedirectToPageResult("/Account/Login");
                    return;
                }
            }
        }

        public void OnPageHandlerExecuted(PageHandlerExecutedContext context)
        {
            // Không cần xử lý gì sau khi page được thực thi
        }

        public void OnPageHandlerSelected(PageHandlerSelectedContext context)
        {
            // Không cần xử lý gì khi page được chọn
        }
    }
}
