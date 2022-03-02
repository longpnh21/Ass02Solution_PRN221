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

namespace NorthWind.Pages.Customers
{
    [AuthorizationFilter(AccountType.Staff)]
    public class IndexModel : PageModel
    {
        private readonly NorthWind.Models.ApplicationDbContext _context;

        public IndexModel(NorthWind.Models.ApplicationDbContext context)
        {
            _context = context;
        }

        public string CustomerIdSort { get; set; }
        public string NameSort { get; set; }
        public string AddressSort { get; set; }
        public string TypeSort { get; set; }
        public string CurrentFilter { get; set; }
        public string CurrentSort { get; set; }

        public PaginatedList<Customer> Customers { get;set; }

        public async Task OnGetAsync(string sortOrder, string currentFilter, string searchString, int? pageIndex)
{
            CurrentSort = sortOrder;

            CustomerIdSort = string.IsNullOrEmpty(sortOrder) ? "id_desc" : "";
            NameSort = sortOrder == "Name" ? "name_desc" : "Name";
            AddressSort = sortOrder == "Address" ? "address_desc" : "Address";

            if (!string.IsNullOrWhiteSpace(searchString))
            {
                pageIndex = 1;
            }
            else
            {
                searchString = currentFilter;
            }

            currentFilter = searchString;

            IQueryable<Customer> query = from c in _context.Customers
                                         select c;
            if (!string.IsNullOrWhiteSpace(searchString))
            {
                query = query.Where(c => c.ContactName.Contains(searchString)
                                                || c.Address.Contains(searchString)
                                                || c.CustomerId.ToString().Contains(searchString));
}
            switch (sortOrder)
            {
                case "id_desc":
                    query = query.OrderByDescending(c => c.CustomerId);
                    break;
                case "Name":
                    query = query.OrderBy(c => c.ContactName);
                    break;
                case "name_desc":
                    query = query.OrderByDescending(c => c.ContactName);
                    break;
                case "Address":
                    query = query.OrderBy(c => c.Address);
                    break;
                case "address_desc":
                    query = query.OrderByDescending(c => c.Address);
                    break;
                default:
                    query = query.OrderBy(c => c.CustomerId);
                    break;
            }

            Customers = await PaginatedList<Customer>.CreateAsync(query.AsNoTracking(), pageIndex ?? 1);
        }
    }
}
