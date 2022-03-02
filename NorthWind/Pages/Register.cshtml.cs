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
    public class RegisterModel : PageModel
    {
        private readonly NorthWind.Models.ApplicationDbContext _context;

        public RegisterModel(NorthWind.Models.ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult OnGet()
        {
            var user = SessionHelper.GetObjectFromJson<Account>(HttpContext.Session, "user");
            if (user != null)
            {
                return RedirectToPage("./Index");
            }
            return Page();
        }

        [BindProperty]
        public string Message { get; set; }
        [BindProperty]
        public Customer Customer { get; set; }

        // To protect from overposting attacks, see https://aka.ms/RazorPagesCRUD
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }
            if (await IsExists())
            {
                Message = "Account already existed";
            }

            _context.Customers.Add(Customer);
            _context.Accounts.Add(new Account { UserName = Customer.CustomerId, Password = Customer.Password, FullName = Customer.ContactName, Type = AccountType.Member });
            await _context.SaveChangesAsync();

            return RedirectToPage("./Index");
        }

        private async Task<bool> IsExists()
        {
            dynamic user = await _context.Accounts.FirstOrDefaultAsync(a => a.UserName.Equals(Customer.CustomerId));
            user = await _context.Customers.FirstOrDefaultAsync(a => a.CustomerId.Equals(Customer.CustomerId));
            return user != null;
        }
    }
}
