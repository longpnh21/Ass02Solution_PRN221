using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using NorthWind.Models;
using NorthWind.Paging;

namespace NorthWind.Pages.Suppliers
{
    public class IndexModel : PageModel
    {
        private readonly NorthWind.Models.ApplicationDbContext _context;

        public IndexModel(NorthWind.Models.ApplicationDbContext context)
        {
            _context = context;
        }
        public string NameSort { get; set; }
        public string AddressSort { get; set; }
        public string CurrentFilter { get; set; }
        public string CurrentSort { get; set; }

        public PaginatedList<Supplier> Suppliers { get;set; }

        public async Task OnGetAsync(string sortOrder, string currentFilter, string searchString, int? pageIndex)
        {
            CurrentSort = sortOrder;

            NameSort = string.IsNullOrEmpty(sortOrder) ? "name_desc" : "";
            AddressSort = sortOrder == "Address" ? "address_desc" : "Address";

            if (!string.IsNullOrWhiteSpace(searchString))
            {
                pageIndex = 1;
            }
            else
            {
                searchString = currentFilter;
            }

            CurrentFilter = searchString;

            IQueryable<Supplier> query = from s in _context.Suppliers
                                        select s;
            if (!string.IsNullOrWhiteSpace(searchString))
            {
                query = query.Where(s => s.SupplierId.ToString().Contains(searchString)
                                                || s.CompanyName.Contains(searchString)
                                                || s.Phone.Contains(searchString)
                                                || s.Address.Contains(searchString));
            }
            switch (sortOrder)
            {
                case "name_desc":
                    query = query.OrderByDescending(p => p.CompanyName);
                    break;
                case "Address":
                    query = query.OrderBy(p => p.Address);
                    break;
                case "address_desc":
                    query = query.OrderByDescending(p => p.Address);
                    break;
                default:
                    query = query.OrderBy(p => p.CompanyName);
                    break;

            }

            Suppliers = await PaginatedList<Supplier>.CreateAsync(query.AsNoTracking(), pageIndex ?? 1);
        }
    }
}
