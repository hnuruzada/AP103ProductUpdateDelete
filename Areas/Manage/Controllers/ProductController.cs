using Ap103PartialView.DAL;
using Ap103PartialView.Extensions;
using Ap103PartialView.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;

namespace Ap103PartialView.Areas.Manage.Controllers
{
    [Area("Manage")]
    public class ProductController : Controller
    {
        private readonly AppDbContext _context;
        private readonly IWebHostEnvironment _env;
        public ProductController(AppDbContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }
        public IActionResult Index()
        {

            List<Product> products = _context.Products.Include(p => p.ProductCategories).ThenInclude(pc => pc.Category).ToList();
            return View(products);
        }
        public IActionResult Create()
        {
            ViewBag.Categories = _context.Categories.ToList();
            ViewBag.Tags = _context.Tags.ToList();

            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Product product)
        {
            ViewBag.Categories = _context.Categories.ToList();
            ViewBag.Tags = _context.Tags.ToList();

            if (!ModelState.IsValid) return View();
            if (product.PhotoFile != null)
            {
                if (!product.PhotoFile.IsImage())
                {
                    ModelState.AddModelError("PhotoFile", "Sekil formati duzgun deyil");
                    return View();
                }
                if (!product.PhotoFile.IsSizeOk(5))
                {
                    ModelState.AddModelError("PhotoFile", "Max 5mb ola biler");
                    return View();
                }
                product.Image = product.PhotoFile.SaveImg(_env.WebRootPath, "img");
            }
            else
            {
                ModelState.AddModelError("PhotoFile", "Sekil elave edin");
                return View();
            }

            product.ProductCategories = new List<ProductCategory>();
            if (product.CategoryIds != null)
            {
                foreach (var categoryId in product.CategoryIds)
                {
                    ProductCategory pCategory = new ProductCategory
                    {
                        Product = product,
                        CategoryId = categoryId
                    };
                    _context.ProductCategories.Add(pCategory);
                }
            }

            _context.Products.Add(product);
            _context.SaveChanges();

            return RedirectToAction("Index");
        }

        public IActionResult Edit(int id)
        {

            ViewBag.Categories = _context.Categories.ToList();
            ViewBag.Tags = _context.Tags.ToList();

            Product product = _context.Products.Include(p => p.ProductCategories).ThenInclude(pc => pc.Category).FirstOrDefault(p=>p.Id==id);
            if (product == null) return RedirectToAction("Index");
            return View(product);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int? id,Product product)
        {
            ViewBag.Categories = _context.Categories.ToList();
            ViewBag.Tags = _context.Tags.ToList();
            if (id == null) { return NotFound(); }
            Product existProduct = _context.Products.Include(p => p.ProductCategories).ThenInclude(pc => pc.Category).FirstOrDefault(p => p.Id == id);

            if (existProduct == null) return RedirectToAction("Index");

            if (product.PhotoFile != null)
            {
                if (!product.PhotoFile.IsImage())
                {
                    ModelState.AddModelError("PhotoFile", "Sekil duzgun formatda deyil");
                    return View();  
                }
                if (!product.PhotoFile.IsSizeOk(5))
                {
                    ModelState.AddModelError("PhotoFile", "Sekil max 5 mb ola biler");
                    return View();
                }
                Helpers.Helper.DeleteImg(_env.WebRootPath, "img", existProduct.Image);
                existProduct.Image = product.PhotoFile.SaveImg(_env.WebRootPath, "img");
            }

            if (!ModelState.IsValid)
            {
                return View(existProduct);
            }

            var existCategory = _context.ProductCategories.Where(pc => pc.ProductId == product.Id).ToList();
            if (product.CategoryIds != null)
            {
                foreach (var categoryId in product.CategoryIds)
                {
                    var excategory = existCategory.FirstOrDefault(c => c.CategoryId == categoryId);
                    if (excategory != null)
                    {
                        ProductCategory pc = new ProductCategory
                        {
                            ProductId=product.Id,
                            CategoryId=categoryId,
                        };
                        _context.ProductCategories.Add(pc);
                    }
                    else
                    {
                        existCategory.Remove(excategory);
                    }

                }
            }
            _context.ProductCategories.RemoveRange(existCategory);

            existProduct.Name = product.Name;
            existProduct.Price=product.Price;
            _context.SaveChanges();
            return RedirectToAction("Index");
        }
        public IActionResult Delete(int? id)
        {
            if (id == null) return NotFound();
            Product product = _context.Products.Include(p => p.ProductCategories).ThenInclude(pc => pc.Category).FirstOrDefault(p => p.Id == id);

            Helpers.Helper.DeleteImg(_env.WebRootPath, "img", product.Image);

            _context.Products.Remove(product);
            _context.SaveChanges();
            return RedirectToAction("Index");
        }

    }
}
