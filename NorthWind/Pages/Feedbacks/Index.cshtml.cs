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

namespace NorthWind.Pages.Feedbacks
{
    [AuthorizationFilter]
    public class IndexModel : PageModel
    {
        private readonly NorthWind.Models.ApplicationDbContext _context;

        public IndexModel(NorthWind.Models.ApplicationDbContext context)
        {
            _context = context;
        }

        public string DateSort { get; set; }
        public string CustomerIdSort { get; set; }
        public string TitleSort { get; set; }
        public string CurrentFilter { get; set; }
        public string CurrentSort { get; set; }

        public PaginatedList<Feedback> Feedbacks { get;set; }

        public async Task OnGetAsync(string sortOrder, string currentFilter, string searchString, int? pageIndex)
{
            CurrentSort = sortOrder;

            DateSort = string.IsNullOrEmpty(sortOrder) ? "date_asc" : "";
            CustomerIdSort = sortOrder == "CustomerId" ? "customerId_desc" : "CustomerId";
            TitleSort = sortOrder == "Title" ? "title_desc" : "Title";

            if (!string.IsNullOrWhiteSpace(searchString))
            {
                pageIndex = 1;
            }
            else
            {
                searchString = currentFilter;
            }

            currentFilter = searchString;

            IQueryable<Feedback> query = from f in _context.Feedbacks
                                        select f;
            if (!string.IsNullOrWhiteSpace(searchString))
            {
                query = query.Where(f => f.CustomerId.ToString().Contains(searchString)
                                                || f.Ftitle.Contains(searchString)
                                                || f.Fcontent.Contains(searchString)
                                                || f.Fdate.ToString().Contains(searchString));
}
            switch (sortOrder)
            {
                case "date_asc":
                    query = query.OrderBy(f => f.Fdate);
                    break;
                case "CustomerId":
                    query = query.OrderBy(f => f.CustomerId);
                    break;
                case "customerId_desc":
                    query = query.OrderByDescending(f => f.CustomerId);
                    break;
                case "Title":
                    query = query.OrderBy(f => f.Ftitle);
                    break;
                case "title_desc":
                    query = query.OrderByDescending(f => f.Ftitle);
                    break;
                default:
                    query = query.OrderByDescending(f => f.Fdate);
                    break;
            }

            query = query.Include(p => p.Customer);
            Feedbacks = await PaginatedList<Feedback>.CreateAsync(query.AsNoTracking(), pageIndex ?? 1);
        }
    }
}
