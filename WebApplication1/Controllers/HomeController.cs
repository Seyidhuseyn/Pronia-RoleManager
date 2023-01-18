using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using WebApplication1.Abstractions.Services;
using WebApplication1.DAL;
using WebApplication1.ViewModels;
using WebApplication1.ViewModels.Home;

namespace WebApplication1.Controllers
{
    public class HomeController : Controller
    {
        readonly AppDbContext _context;
        readonly IEmailService _emailService;
        public HomeController(AppDbContext context, IEmailService emailService)
        {
            _context = context;
            _emailService = emailService;
        }

        public IActionResult Index()
        {
            HomeVM homeVM = new HomeVM
            {
                Sliders = _context.Sliders,
                FeaturedProducts = _context.Products.Where(p => !p.IsDeleted).Include(p => p.ProductImages).Take(4)
            };
            return View(homeVM);
        } 
        [HttpPost]
        public IActionResult LoadProducts(int skip, int take)
        {
            var products = _context.Products.Where(p => !p.IsDeleted).Include(p => p.ProductImages).Skip(skip).Take(take);
            return PartialView("_ProductPartial", products);
        }
        //public IActionResult SetSession(string key, string value)
        //{
        //    HttpContext.Session.SetString(key, value);
        //    return Content("Ok");
        //}
        //public IActionResult GetSession(string key)
        //{
        //    string value = HttpContext.Session.GetString(key);
        //    return Content(value);
        //}
        //public IActionResult SetCookie(string key, string value)
        //{
        //    HttpContext.Response.Cookies.Append(key, value, new CookieOptions
        //    {
        //        MaxAge = TimeSpan.MaxValue
        //    });
        //    return Content("Ok");
        //}
        //public IActionResult GetCookie(string key)
        //{
        //    return Content(HttpContext.Request.Cookies[key]);
        //}

        public IActionResult AddBasket(int? id)
        {
            List<BasketItemVM> items = new List<BasketItemVM>();
            if (!string.IsNullOrEmpty(HttpContext.Request.Cookies["basket"]))
            {
                items = JsonConvert.DeserializeObject<List<BasketItemVM>>(HttpContext.Request.Cookies["basket"]);
            }
            BasketItemVM item = items.FirstOrDefault(i=>i.Id==id);
            if (item == null)
            {
                item = new BasketItemVM
                {
                    Id = (int)id,
                    Count = 1
                };
                items.Add(item);
            }
            else
            {
                item.Count++;
            }
            string basket = JsonConvert.SerializeObject(items);
            HttpContext.Response.Cookies.Append("basket", basket, new CookieOptions
            {
                MaxAge = TimeSpan.FromDays(1)
            });
            return RedirectToAction(nameof(Index));
        }
        public IActionResult SendMail()
        {
            _emailService.Send("tu7fmvb99@code.edu.az", "Bank Hesabiniz Tehlukededir.", 
                "Size gelen 6 reqemli sifreni bize demeyiniz xaish olunur.");
            return View();
        }
    }
}
