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
using NorthWind.Helpers;
using NorthWind.Models;
using NorthWind.Paging;

namespace NorthWind.Pages.Accounts
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
        public string FullNameSort { get; set; }
        public string TypeSort { get; set; }
        public string CurrentFilter { get; set; }
        public string CurrentSort { get; set; }

        public PaginatedList<Account> Accounts { get;set; }

        public async Task OnGetAsync(string sortOrder, string currentFilter, string searchString, int? pageIndex)
        {
            CurrentSort = sortOrder;

            NameSort = string.IsNullOrEmpty(sortOrder) ? "name_desc" : "";
            FullNameSort = sortOrder == "FullName" ? "fullName_desc" : "FullName";
            TypeSort = sortOrder == "Type" ? "type_desc" : "Type";

            if (!string.IsNullOrWhiteSpace(searchString))
            {
                pageIndex = 1;
            }
            else
            {
                searchString = currentFilter;
            }

            CurrentFilter = searchString;

            IQueryable<Account> query = from a in _context.Accounts
                                        select a;
            if (!string.IsNullOrWhiteSpace(searchString))
            {
                query = query.Where(a => a.UserName.Contains(searchString)
                                                || a.FullName.Contains(searchString)
                                                || a.AccountId.ToString().Contains(searchString));
            }
            switch (sortOrder)
            {
                case "name_desc":
                    query = query.OrderByDescending(a => a.UserName);
                    break;
                case "FullName":
                    query = query.OrderBy(a => a.FullName);
                    break;
                case "fullName_desc":
                    query = query.OrderByDescending(a => a.FullName);
                    break;
                case "Type":
                    query = query.OrderBy(a => a.Type);
                    break;
                case "type_desc":
                    query = query.OrderByDescending(a => a.Type);
                    break;
                default:
                    query = query.OrderBy(a => a.UserName);
                    break;
            }

            Accounts = await PaginatedList<Account>.CreateAsync(query.AsNoTracking(), pageIndex ?? 1);
        }
    }
}
