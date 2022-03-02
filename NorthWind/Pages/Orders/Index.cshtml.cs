using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using NorthWind.Models;
using NorthWind.Paging;

namespace NorthWind.Pages.Orders
{
    public class IndexModel : PageModel
    {
        private readonly NorthWind.Models.ApplicationDbContext _context;

        public IndexModel(NorthWind.Models.ApplicationDbContext context)
        {
            _context = context;
        }

        public string OrderDateSort { get; set; }
        public string CustomerSort { get; set; }
        public string CurrentFilter { get; set; }
        public string CurrentSort { get; set; }

        public PaginatedList<Order> Orders { get;set; }

        public async Task OnGetAsync(string sortOrder, string currentFilter, string searchString, int? pageIndex)
{
            CurrentSort = sortOrder;
            OrderDateSort = string.IsNullOrEmpty(sortOrder) ? "orderDate_desc" : "";
            CustomerSort = sortOrder == "Customer" ? "customer_desc" : "Customer";

            if (!string.IsNullOrWhiteSpace(searchString))
            {
                pageIndex = 1;
            }
            else
            {
                searchString = currentFilter;
            }

            currentFilter = searchString;

            IQueryable<Order> query = from o in _context.Orders
                                      select o;

            if (!string.IsNullOrWhiteSpace(searchString))
            {
                query = query.Where(o => o.OrderId.ToString().Contains(searchString)
                                                || o.CustomerId.Contains(searchString)
                                                || o.Customer.ContactName.Contains(searchString));
}
            switch (sortOrder)
            {
                case "orderDate_desc":
                    query = query.OrderByDescending(p => p.OrderDate);
                    break;
                case "Customer":
                    query = query.OrderBy(o => o.CustomerId);
                    break;
                case "customer_desc":
                    query = query.OrderByDescending(o => o.CustomerId);
                    break;
                default:
                    query = query.OrderBy(o => o.OrderDate);
                    break;

            }

            query = query.Include(o => o.OrderDetails);
            query = query.Include(o => o.Customer);

            Orders = await PaginatedList<Order>.CreateAsync(query.AsNoTracking(), pageIndex ?? 1);
        }
    }
}
