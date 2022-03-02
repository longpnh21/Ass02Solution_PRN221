using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using NorthWind.Enums;
using NorthWind.Filters;
using NorthWind.Helpers;
using NorthWind.Models;

namespace NorthWind.Pages.Profile
{
    [AuthorizationFilter]
    public class InfoModel : PageModel
    {
        private readonly NorthWind.Models.ApplicationDbContext _context;

        public InfoModel(NorthWind.Models.ApplicationDbContext context)
        {
            _context = context;
        }

        public Account Account { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            var user = SessionHelper.GetObjectFromJson<Account>(HttpContext.Session, "user");
            if (id == null)
            {
                return NotFound();
            }

            Account = await _context.Accounts.FirstOrDefaultAsync(m => m.AccountId == id);

            if (Account == null)
            {
                return NotFound();
            }
            return Page();
        }
    }
}
