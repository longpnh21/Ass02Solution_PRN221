using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using NorthWind.Enums;
using NorthWind.Filters;

namespace NorthWind.Pages
{
    [AuthorizationFilter]
    public class LogoutModel : PageModel
    {
        public IActionResult OnGet()
        {
            HttpContext.Session.Clear();
            return RedirectToPage("./Index");
        }
    }
}
