using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using NorthWind.Enums;
using NorthWind.Helpers;
using NorthWind.Models;

namespace NorthWind.Pages
{
    public class LoginModel : PageModel
    {
        private readonly NorthWind.Models.ApplicationDbContext _context;

        public LoginModel(NorthWind.Models.ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult OnGet()
        {
            var user = SessionHelper.GetObjectFromJson<Account>(HttpContext.Session,"user");
            if (user != null)
            {
                return RedirectToPage("./Index");
            }
            return Page();
        }
        [BindProperty]
        public string Message { get; set; }
        [BindProperty]
        public string UserName { get; set; }
        [BindProperty]
        public string Password { get; set; }

        // To protect from overposting attacks, see https://aka.ms/RazorPagesCRUD
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            Account account = await _context.Accounts.AsNoTracking().FirstOrDefaultAsync(a => a.UserName.Equals(UserName) && a.Password.Equals(Password));
            Customer customer = await _context.Customers.AsNoTracking().FirstOrDefaultAsync(c => c.CustomerId.Equals(UserName) && c.Password.Equals(Password));

            if (account == null && customer == null)
            {
                Message = "Incorrect username or password.";
                return Page();
            }

            var user = new Account();
            if (account != null)
            {
                user.UserName = account.UserName;
                user.Type = account.Type;

            }
            else if (customer != null)
            {
                user.UserName = customer.CustomerId;
                user.Type = AccountType.Member;

            }
            SessionHelper.SetObjectAsJson(HttpContext.Session, "user", user);
            return RedirectToPage("./Index");
        }
    }
}
