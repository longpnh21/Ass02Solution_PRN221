using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using NorthWind.Enums;
using NorthWind.Filters;
using NorthWind.Models;
using NorthWind.Paging;

namespace NorthWind.Pages.Categories
{
    [AuthorizationFilter(AccountType.Staff)]
    public class IndexModel : PageModel
    {
        private readonly NorthWind.Models.ApplicationDbContext _context;

        public IndexModel(NorthWind.Models.ApplicationDbContext context)
        {
            _context = context;
        }

        public string NameSort { get; set; }
        public string CurrentFilter { get; set; }
        public string CurrentSort { get; set; }
        public PaginatedList<Category> Categories { get;set; }

        public async Task OnGetAsync(string sortOrder, string currentFilter, string searchString, int? pageIndex)
{
            CurrentSort = sortOrder;

            NameSort = string.IsNullOrEmpty(sortOrder) ? "name_desc" : "";

            if (!string.IsNullOrWhiteSpace(searchString))
            {
                pageIndex = 1;
            }
            else
            {
                searchString = currentFilter;
            }

            CurrentFilter = searchString;

            IQueryable<Category> query = from c in _context.Categories
                                         select c;
            if (!string.IsNullOrWhiteSpace(searchString))
            {
                query = query.Where(c => c.CategoryId.ToString().Contains(searchString)
                                                || c.CategoryName.Contains(searchString)
                                                || c.Description.Contains(searchString));
            }

            switch (sortOrder)
            {
                case "name_desc":
                    query = query.OrderByDescending(c => c.CategoryName);
                    break;
                default:
                    query = query.OrderBy(c => c.CategoryName);
                    break;

            }

            Categories = await PaginatedList<Category>.CreateAsync(query.AsNoTracking(), pageIndex ?? 1);
        }
    }
}
