using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebApplication1.DAL;
using WebApplication1.Models;
using WebApplication1.ViewModels;

namespace WebApplication1.Areas.Manage.Controllers
{
    [Area("Manage")]
    [Authorize(Roles = "Moderator , Admin")]
    public class SponsorController : Controller
    {
        readonly AppDbContext _context;
        readonly IWebHostEnvironment _env;
        public SponsorController(AppDbContext context, IWebHostEnvironment env = null)
        {
            _context = context;
            _env = env;
        }
        public IActionResult Index()
        {
            return View(_context.Sponsors.ToList());
        }
        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        public IActionResult Create(CreateSponsorVM sponsorVM)
        {
            if (!ModelState.IsValid) return View();
            IFormFile file = sponsorVM.Image;
            if (!file.ContentType.Contains("image/"))
            {
                ModelState.AddModelError("Image", "Yuklediyiniz fal shekil deyil");
                return View();
            }
            if (file.Length > 200 * 1024)
            {
                ModelState.AddModelError("Image", "Shekilin olcusu 200 kb-dan artiq ola bilmez.");
                return View();
            }
            string fileName = Guid.NewGuid().ToString() + file.FileName;
            using (var stream = new FileStream(Path.Combine(_env.WebRootPath, "assets", "images", "brand", fileName), FileMode.Create))
            {
                file.CopyTo(stream);
            }
            Sponsor sponsor = new Sponsor { ImageUrl = fileName };
            _context.Sponsors.Add(sponsor);
            _context.SaveChanges();
            return RedirectToAction(nameof(Index));
        }
        public IActionResult Update(int? id)
        {
            if (id == null || id == 0) return BadRequest();
            Sponsor sponsor = _context.Sponsors.Find(id);
            if (sponsor is null) return NotFound();
            return View(sponsor);
        }
        [HttpPost]
        public IActionResult Update(int? id, Sponsor sponsor)
        {
            if (id == null || id == 0 || id != sponsor.Id || sponsor is null) return BadRequest();
            if (!ModelState.IsValid) return View();
            Sponsor exist = _context.Sponsors.Find(sponsor.Id);
            exist.ImageUrl = sponsor.ImageUrl;
            _context.SaveChanges();
            return RedirectToAction(nameof(Index));
        }
        public IActionResult Delete(int? id)
        {
            if (id is null) return BadRequest();

            Sponsor sponsor = _context.Sponsors.Find(id);
            if (sponsor is null) return NotFound();
            _context.Sponsors.Remove(sponsor);
            _context.SaveChanges();
            return RedirectToAction(nameof(Index));
        }
    }
}
