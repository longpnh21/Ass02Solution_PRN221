using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using NorthWind.Enums;
using NorthWind.Filters;
using NorthWind.Models;

namespace NorthWind.Pages.Products
{
    [AuthorizationFilter(AccountType.Staff)]
    public class DetailsModel : PageModel
    {
        private readonly NorthWind.Models.ApplicationDbContext _context;

        public DetailsModel(NorthWind.Models.ApplicationDbContext context)
        {
            _context = context;
        }

        public Product Product { get; set; }

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Product = await _context.Products
                .Include(p => p.Category)
                .Include(p => p.Supplier).FirstOrDefaultAsync(m => m.ProductId == id);

            if (Product == null)
            {
                return NotFound();
            }
            return Page();
        }
    }
}
