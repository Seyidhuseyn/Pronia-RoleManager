using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using WebApplication1.DAL;
using WebApplication1.ViewModels;
using WebApplication1.ViewModels.Basket;
using WebApplication1.ViewModels.Component;

namespace WebApplication1.ViewComponents
{
    public class HeaderViewComponent:ViewComponent
    {
        readonly AppDbContext _context;

        public HeaderViewComponent(AppDbContext context)
        {
            _context = context;
        }
        public async Task<IViewComponentResult> InvokeAsync()
        {
            HeaderVM header = new HeaderVM
            {
                Settings = _context.Settings.ToDictionary(s => s.Key, s => s.Value),
                Basket = GetBasket()
            };
            return View(header);
        }
        BasketVM GetBasket()
        {
            BasketVM basket = new BasketVM();
            List<BasketItemVM> items = new List<BasketItemVM>();
            if (!string.IsNullOrEmpty(HttpContext.Request.Cookies["Basket"]))
            {
                items = JsonConvert.DeserializeObject<List<BasketItemVM>>(HttpContext.Request.Cookies["Basket"]);
            }
            if (items != null)
            {
                basket.Fruits = new List<FruitBasketItemVM>();
                foreach (var item in items)
                {
                    FruitBasketItemVM fruit = new FruitBasketItemVM();
                    fruit.Product = _context.Products.Include(p=>p.ProductImages).Where(p=>p.ProductImages.Any(pi=>pi.IsCover==true)).FirstOrDefault(p=>p.Id==item.Id);
                    fruit.Count = item.Count;
                    basket.Fruits.Add(fruit);
                    basket.TotalPrice += fruit.Product.SellPrice * fruit.Count;
                }
            }
            return basket;
        }
    }
}
