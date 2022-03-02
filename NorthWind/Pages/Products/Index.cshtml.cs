using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using NorthWind.Models;
using NorthWind.Paging;

namespace NorthWind.Pages.Products
{
    public class IndexModel : PageModel
    {
        private readonly NorthWind.Models.ApplicationDbContext _context;

        public IndexModel(NorthWind.Models.ApplicationDbContext context)
        {
            _context = context;
        }
        public string NameSort { get; set; }
        public string UnitPriceSort { get; set; }
        public string CurrentFilter { get; set; }
        public string CurrentSort { get; set; }

        public PaginatedList<Product> Products { get;set; }


        public async Task OnGetAsync(string sortOrder, string currentFilter, string searchString, int? pageIndex)
        {
            CurrentSort = sortOrder;

            NameSort = string.IsNullOrEmpty(sortOrder) ? "name_desc" : "";
            UnitPriceSort = sortOrder == "Price" ? "price_desc" : "Price";

            if (!string.IsNullOrWhiteSpace(searchString))
            {
                pageIndex = 1;
            }
            else
            {
                searchString = currentFilter;
            }

            currentFilter = searchString;

            IQueryable<Product> query = from p in _context.Products
                                        select p;
            if (!string.IsNullOrWhiteSpace(searchString))
            {
                query = query.Where(p => p.ProductId.ToString().Contains(searchString)
                                                || p.ProductName.Contains(searchString)
                                                || p.UnitPrice.ToString().Contains(searchString));
            }
            switch (sortOrder)
            {
                case "name_desc":
                    query = query.OrderByDescending(p => p.ProductName);
                    break;
                case "Price":
                    query = query.OrderBy(p => p.UnitPrice);
                    break;
                case "price_desc":
                    query = query.OrderByDescending(p => p.UnitPrice);
                    break;
                default:
                    query = query.OrderBy(p => p.ProductName);
                    break;

            }

            query = query.Include(p => p.Category);
            query = query.Include(p => p.Supplier);
            Products = await PaginatedList<Product>.CreateAsync(query.AsNoTracking(), pageIndex ?? 1);
        }
    }
}
