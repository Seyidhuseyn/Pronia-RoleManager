using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebApplication1.DAL;
using WebApplication1.Models;
using WebApplication1.ViewModels;

namespace WebApplication1.Areas.Manage.Controllers
{
    [Area("Manage")]
    [Authorize(Roles = "Moderator , Admin")]
    public class SliderController : Controller
    {
        readonly AppDbContext _context;
        readonly IWebHostEnvironment _env;
        public SliderController(AppDbContext context, IWebHostEnvironment env = null)
        {
            _context = context;
            _env = env;
        }
        public IActionResult Index()
        {
            return View(_context.Sliders.ToList());
        }
        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        public IActionResult Create(CreateSliderVM  sliderVM)
        {
            if (!ModelState.IsValid) return View();
            IFormFile file = sliderVM.Image;
            if (!file.ContentType.Contains("image/"))
            {
                ModelState.AddModelError("Image", "Yuklediyiniz fal shekil deyil");
                return View();
            }
            if (file.Length > 200 * 1024)
            {
                ModelState.AddModelError("Image" , "Shekilin olcusu 200 kb-dan artiq ola bilmez.");
                return View();
            }
            string fileName = Guid.NewGuid().ToString() + file.FileName;
            using (var stream = new FileStream(Path.Combine(_env.WebRootPath, "assets", "images", "slider", fileName), FileMode.Create))
            {
                file.CopyTo(stream);
            }
            Slider slider = new Slider { 
                Description = sliderVM.Description, 
                Order = sliderVM.Order, 
                PrimaryTitle = sliderVM.PrimaryTitle, 
                SecondaryTitle = sliderVM.SecondaryTitle, 
                ImageUrl=fileName 
            };
            if (_context.Sliders.Any(s => s.Order == slider.Order))
            {
                ModelState.AddModelError("Order", $"{slider.Order} sirasinda artiq slider movcuddur.");
                return View();
            }
            _context.Sliders.Add(slider);
            _context.SaveChanges();
            return RedirectToAction("Index");
        }
        public IActionResult Update(int? id)
        {
            if (id == null || id == 0) return BadRequest();
            Slider slider = _context.Sliders.Find(id);
            if (slider is null) return NotFound();
            //UpdateSliderVM updateSlider = new UpdateSliderVM
            //{
            //    Id= slider.Id,
            //    PrimaryTitle = slider.PrimaryTitle,
            //    SecondaryTitle = slider.SecondaryTitle,
            //    Description = slider.Description,
            //    Order = slider.Order
            //};
            return View(slider);
        }
        [HttpPost]
        public IActionResult Update(int? id, UpdateSliderVM updateSlider)
        {
            if (id == null || id == 0 || id!= updateSlider.Id || updateSlider is null) return BadRequest();
            if (!ModelState.IsValid) return View();
            Slider anotherSlider = _context.Sliders.FirstOrDefault(s=>s.Order== updateSlider.Order);
            if (anotherSlider != null)
            {
                anotherSlider.Order = _context.Sliders.Find(id).Order;
            }
            Slider exist = _context.Sliders.Find(updateSlider.Id);
            exist.Order = updateSlider.Order;
            exist.Description = updateSlider.Description;
            exist.PrimaryTitle= updateSlider.PrimaryTitle;
            exist.SecondaryTitle= updateSlider.SecondaryTitle;
            exist.ImageUrl= updateSlider.ImageUrl;
            _context.SaveChanges();
            return RedirectToAction(nameof(Index));
        }
        public IActionResult Delete(int? id)
        {
            if (id is null) return BadRequest();

            Slider slider = _context.Sliders.Find(id);
            if (slider is null) return NotFound();
            _context.Sliders.Remove(slider);
            _context.SaveChanges();
            return RedirectToAction(nameof(Index));
        }
    }
}
