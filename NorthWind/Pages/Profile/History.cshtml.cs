using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using NorthWind.Enums;
using NorthWind.Filters;

namespace NorthWind.Pages.Profile
{
    [AuthorizationFilter(AccountType.Member)]
    public class HistoryModel : PageModel
    {
        public void OnGet()
        {
        }
    }
}
