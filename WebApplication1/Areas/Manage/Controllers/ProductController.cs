using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using WebApplication1.DAL;
using WebApplication1.Models;
using WebApplication1.Utilies.Extension;
using WebApplication1.ViewModels;
using WebApplication1.ViewModels;

namespace WebApplication1.Areas.Manage.Controllers
{
    [Area("Manage")]
    [Authorize(Roles = "Moderator , Admin")]
    public class ProductController : Controller
    {
        readonly AppDbContext _context;
        readonly IWebHostEnvironment _env;

        public ProductController(AppDbContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }

        public IActionResult Index()
        {
            return View(_context.Products.Include(p => p.ProductColors).ThenInclude(pc => pc.Color).Include(p => p.ProductSizes)
                .ThenInclude(ps => ps.Size).Include(p => p.ProductImages));
        }
        public IActionResult Delete(int? id)
        {
            if (id == null || id == 0) return BadRequest();
            Product existed = _context.Products.Include(p => p.ProductImages).FirstOrDefault(p => p.Id == id);
            if (existed == null) return NotFound();
            foreach (ProductImage image in existed.ProductImages)
            {
                image.ImageUrl.DeleteFile(_env.WebRootPath, "assets/images/product");
            }
            _context.ProductImages.RemoveRange(existed.ProductImages);
            existed.IsDeleted = true;
            _context.SaveChanges();
            return RedirectToAction(nameof(Index));
        }
        [Authorize]
        public IActionResult Create()
        {
            ViewBag.Colors = new SelectList(_context.Colors, nameof(Color.Id), nameof(Color.Name));
            ViewBag.Sizes = new SelectList(_context.Sizes, nameof(Size.Id), nameof(Size.Name));
            return View();
        }
        [HttpPost]
        public IActionResult Create(CreateProductVM cp)
        {
            var coverImg = cp.CoverImage;
            var hoverImg = cp.HoverImage;
            var otherImgs = cp.OtherImages ?? new List<IFormFile>();
            string result = coverImg?.CheckValidate("image/", 300);
            if (result?.Length > 0)
            {
                ModelState.AddModelError("CoverImage", result);
            }
            result = hoverImg?.CheckValidate("image/", 300);
            if (result?.Length > 0)
            {
                ModelState.AddModelError("HoverImage", result);
            }
            foreach (IFormFile image in otherImgs)
            {
                result = image.CheckValidate("image/", 300);
                if (result?.Length > 0)
                {
                    ModelState.AddModelError("OtherImages", result);
                }
            }
            foreach (int colorId in (cp.ColorIds ?? new List<int>()))
            {
                if (!_context.Colors.Any(c => c.Id == colorId))
                {
                    ModelState.AddModelError("ColorIds", "Alinmadi, birde.");
                    break;
                }
            }
            foreach (int sizeId in cp.SizeIds)
            {
                if (!_context.Sizes.Any(c => c.Id == sizeId))
                {
                    ModelState.AddModelError("SizeIds", "Alinmadi, birde.");
                    break;
                }
            }
            if (!ModelState.IsValid)
            {
                ViewBag.Colors = new SelectList(_context.Colors, nameof(Color.Id), nameof(Color.Name));
                ViewBag.Colors = new SelectList(_context.Sizes, nameof(Size.Id), nameof(Size.Name));
                return View();
            }
            var sizes = _context.Sizes.Where(s => cp.SizeIds.Contains(s.Id));
            var colors = _context.Colors.Where(c => cp.ColorIds.Contains(c.Id));
            Product newProduct = new Product
            {
                Name = cp.Name,
                CostPrice = cp.CostPrice,
                SellPrice = cp.SellPrice,
                Description = cp.Description,
                Discount = cp.Discount,
                IsDeleted = false,
                SKU = "1"
            };
            List<ProductImage> images = new List<ProductImage>();
            images.Add(new ProductImage { ImageUrl = coverImg.SaveFile(Path.Combine(_env.WebRootPath, "assets", "images", "product")), IsCover = true, Product = newProduct });
            images.Add(new ProductImage { ImageUrl = hoverImg.SaveFile(Path.Combine(_env.WebRootPath, "assets", "images", "product")), IsCover = false, Product = newProduct });
            foreach (var item in otherImgs)
            {
                images.Add(new ProductImage { ImageUrl = item.SaveFile(Path.Combine(_env.WebRootPath, "assets", "images", "product")), IsCover = null, Product = newProduct });
            }
            newProduct.ProductImages = images;
            _context.Products.Add(newProduct);
            foreach (var item in sizes)
            {
                _context.ProductSizes.Add(new ProductSize { Product = newProduct, SizeId = item.Id });
            }
            foreach (var item in colors)
            {
                _context.ProductColors.Add(new ProductColor { Product = newProduct, ColorId = item.Id });
            }
            _context.SaveChanges();
            return RedirectToAction(nameof(Index));
        }
        public IActionResult Update(int? id)
        {
            if (id == null) return BadRequest();
            Product product = _context.Products.Include(p=>p.ProductColors).Include(p=>p.ProductSizes).FirstOrDefault(p=>p.Id==id);
            if (product == null) return NotFound();
            UpdateProductVM updateProduct = new UpdateProductVM
            {
                Id = product.Id,
                Name = product.Name,
                Description = product.Description,
                SellPrice = product.SellPrice,
                CostPrice = product.CostPrice,
                ColorIds = new List<int>(),
                SizeIds = new List<int>()
            };
            foreach (ProductColor color in product.ProductColors)
            {
                updateProduct.ColorIds.Add(color.ColorId);
            }
            foreach (ProductSize size in product.ProductSizes)
            {
                updateProduct.SizeIds.Add(size.SizeId);
            }
            ViewBag.Colors= new SelectList(_context.Colors, nameof(Color.Id), nameof(Color.Name));
            ViewBag.Sizes= new SelectList(_context.Sizes, nameof(Size.Id), nameof(Size.Name));
            return View(updateProduct);
        }
        [HttpPost]
        public IActionResult Uppdate(int id, UpdateProductVM updateProduct)
        {
            if (id == null) return NotFound();
            foreach (int colorId in (updateProduct.ColorIds ?? new List<int>()))
            {
                if (!_context.Colors.Any(c=>c.Id==colorId))
                {
                    ModelState.AddModelError("ColorIds", "Alinmadi");
                    break;
                }
            }
            foreach (int sizeId in (updateProduct.SizeIds ?? new List<int>()))
            {
                if (!_context.Sizes.Any(s=>s.Id==sizeId))
                {
                    ModelState.AddModelError("SizeIds", "Alinmadi2");
                }
            }
            if (!ModelState.IsValid)
            {
                ViewBag.Colors = new SelectList(_context.Colors, nameof(Color.Id), nameof(Color.Name));
                ViewBag.Sizes = new SelectList(_context.Sizes, nameof(Size.Id), nameof(Size.Name));
                return View();
            }
            var prod = _context.Products.Include(p => p.ProductColors).Include(p=>p.ProductSizes).FirstOrDefault(p=>p.Id==id);
            if (prod != null) return NotFound();
            foreach (var item in prod.ProductColors)
            {
                if (updateProduct.ColorIds.Contains(item.ColorId))
                {
                    updateProduct.ColorIds.Remove(item.ColorId);
                }
                else
                {
                    _context.ProductColors.Remove(item);
                }
            }
            foreach (var colorId in updateProduct.ColorIds)
            {
                _context.ProductColors.Add(new ProductColor { Product = prod, ColorId = colorId });
            }
            prod.Name = updateProduct.Name;
            prod.Description = updateProduct.Description;
            prod.CostPrice = updateProduct.CostPrice;
            prod.SellPrice= updateProduct.SellPrice;
            prod.Discount= updateProduct.Discount;
            _context.SaveChanges();
            return RedirectToAction(nameof(Index));
        }
        public IActionResult UpdateImage(int? id)
        {
            if (id == null) return BadRequest();
            var productImage = _context.Products.Include(p=>p.ProductImages).FirstOrDefault(p=>p.Id==id);
            if (productImage == null) return NotFound();
            UpdateImageVM updateImage = new UpdateImageVM
            {
                ProductImages = productImage.ProductImages
            };
            return View(updateImage);
        }
        public IActionResult DeleteImage(int? id)
        {
            if(id==null) return BadRequest();
            var productImage = _context.ProductImages.Find();
            if (productImage == null) return NotFound();
            _context.ProductImages.Remove(productImage);
            _context.SaveChanges();
            return Ok();
        }
    }
}