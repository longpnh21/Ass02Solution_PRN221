using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using NorthWind.Enums;
using NorthWind.Filters;
using NorthWind.Helpers;
using NorthWind.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NorthWind.Pages.Profile
{
    [AuthorizationFilter(AccountType.Member)]
    public class CartModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public List<OrderDetail> Cart { get; set; }
        public decimal Total { get; set; }

        public CartModel(ApplicationDbContext context)
        {
            _context = context;
        }

        public void OnGet()
        {
            Cart = SessionHelper.GetObjectFromJson<List<OrderDetail>>(HttpContext.Session, "cart");

            if(Cart != null)
            {
                Total = Cart.Sum(c => c.Quantity * c.UnitPrice);
            }
            else
            {
                Total = 0;
            }
        }

        public async Task<IActionResult> OnGetBuyNow(int id)
        {
            var product = await _context.Products.FirstOrDefaultAsync(p => p.ProductId == id);
            Cart = SessionHelper.GetObjectFromJson<List<OrderDetail>>(HttpContext.Session, "cart");
            if (Cart == null)
            {
                Cart = new List<OrderDetail>();
                Cart.Add(new OrderDetail
                {
                    Quantity = 1,
                    UnitPrice = product.UnitPrice.Value,
                    ProductId = product.ProductId,
                });
                SessionHelper.SetObjectAsJson(HttpContext.Session, "cart", Cart);
            }
            else
            {
                int index = Exists(Cart, id);
                if (index == -1)
                {
                    Cart.Add(new OrderDetail
                    {
                        Quantity = 1,
                        UnitPrice = product.UnitPrice.Value,
                        ProductId = product.ProductId,
                    });
                }
                else
                {
                    Cart[index].Quantity++;
                }
                SessionHelper.SetObjectAsJson(HttpContext.Session, "cart", Cart);
            }
            return RedirectToPage("Cart");
        }

        public IActionResult OnGetDelete(int id)
        {
            Cart = SessionHelper.GetObjectFromJson<List<OrderDetail>>(HttpContext.Session, "cart");
            int index = Exists(Cart, id);
            Cart.RemoveAt(index);
            SessionHelper.SetObjectAsJson(HttpContext.Session, "cart", Cart);
            return RedirectToPage("Cart");
        }

        public IActionResult OnPostUpdate(short[] quantities)
        {
            Cart = SessionHelper.GetObjectFromJson<List<OrderDetail>>(HttpContext.Session, "cart");
            for (var i = 0; i < Cart.Count; i++)
            {
                Cart[i].Quantity = quantities[i];
            }
            SessionHelper.SetObjectAsJson(HttpContext.Session, "cart", Cart);
            return RedirectToPage("Cart");
        }

        private int Exists(List<OrderDetail> cart, int id)
        {
            for (var i = 0; i < cart.Count; i++)
            {
                if (cart[i].ProductId == id)
                {
                    return i;
                }
            }
            return -1;
        }

    }
}
