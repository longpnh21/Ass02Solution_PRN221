using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;
using NorthWind.Enums;
using NorthWind.Helpers;
using NorthWind.Models;
using System.Threading.Tasks;

namespace NorthWind.Filters
{
    public class AuthorizationFilter : ResultFilterAttribute
    {
        private AccountType? _type;

        public AuthorizationFilter()
        {

        }

        public AuthorizationFilter(AccountType type)
        {
            _type = type;
        }

        public override async Task OnResultExecutionAsync(ResultExecutingContext context, ResultExecutionDelegate next)
        {
            var user = SessionHelper.GetObjectFromJson<Account>(context.HttpContext.Session, "user");
            if (user == null)
            {
                context.Result = new RedirectToPageResult("/Permission");
            }
            else if (_type != null && user.Type == _type)
            {
                context.Result = new RedirectToPageResult("/Permission");
            }
            await next();
        }
    }
}
