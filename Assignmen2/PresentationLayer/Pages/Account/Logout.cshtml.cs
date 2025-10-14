using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace PresentationLayer.Pages.Account
{
    public class LogoutModel : PageModel
    {
        public IActionResult OnGet()
        {
            // Xóa session khi GET request (từ URL trực tiếp)
            HttpContext.Session.Clear();
            return Redirect("/");
        }

        public IActionResult OnPost()
        {
            // Xóa session khi POST request (từ form)
            HttpContext.Session.Clear();
            return Redirect("/");
        }
    }
}


