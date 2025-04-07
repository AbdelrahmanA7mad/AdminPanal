using AdminPanal.Data;
using AdminPanal.Models;
using AdminPanal.Services;
using AdminPanal.ViewModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace AdminPanal.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        private readonly AppDbContext _db;
        private readonly IProductServices _IProductServices;
        private readonly ICategoryServices _ICategoryServices;

        public HomeController(AppDbContext db, IProductServices IProductServices, ICategoryServices ICategoryServices)
        {
            _db = db;
            _IProductServices = IProductServices;
            _ICategoryServices = ICategoryServices;
        }


        string number = string.Empty;
        public IActionResult Index()
        {
            var categoriesWithProducts = _db.Categories
                .Select(category => new ViewModelOfIdnex
                {
                    Id = category.Id,
                    Name = category.Category_Name,
                    Products = _db.Products.Where(product => product.CategoryId == category.Id).ToList()


                })
                .ToList();

            return View(categoriesWithProducts);
        }
        [HttpGet]
        public IActionResult CreateProduct()
        {
            var categories = _db.Categories.ToList();
            CreateProductViewModel viewModel = new()
            {
                Categories = _db.Categories.
                Select(c => new Microsoft.AspNetCore.Mvc.Rendering.SelectListItem { Value = c.Id.ToString(), Text = c.Category_Name }).ToList()

            };

            return View(viewModel);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateProduct(CreateProductViewModel viewModel)
        {
            if (!ModelState.IsValid)
            {
                viewModel.Categories = _db.Categories.
                Select(c => new Microsoft.AspNetCore.Mvc.Rendering.SelectListItem { Value = c.Id.ToString(), Text = c.Category_Name }).ToList();

                return View(viewModel);

            }
            await _IProductServices.Create(viewModel);

            return RedirectToAction(nameof(Index));
        }




        [HttpGet]
        public async Task<IActionResult> GetProductDescription(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                return BadRequest(new { description = "Product name is required." });
            }

            try
            {
                var description = await _IProductServices.GenerateProductDescription(name);
                return Ok(new { description = description });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to generate product description for: {ProductName}", name);
                return StatusCode(500, new { description = "Failed to generate description." });
            }
        }




        [HttpGet]
        public IActionResult CreateCategory()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateCategory(CreateCategoryViewModel viewModel)
        {
            if (!ModelState.IsValid)
            {
                return View(viewModel);

            }
            await _ICategoryServices.Create(viewModel);

            return RedirectToAction(nameof(Index));
        }

        [HttpDelete]
        public IActionResult DeleteCategory(int id)
        {
            var isdeleted = _ICategoryServices.DeleteCategory(id);
            return isdeleted ? Ok() : BadRequest();
        }
        
        [HttpDelete]
        public IActionResult DeleteProduct(int id)
        {
            var isdeleted = _IProductServices.DeleteProduct(id);
            return isdeleted ? Ok() : BadRequest();
        }



        [HttpGet]
        public async Task<IActionResult> UpdateCategory(int id)
        {

            var cat = await _ICategoryServices.GetByIDAsync(id);

            if (cat == null)
            {

                return NotFound();
            }

            CreateCategoryViewModel viewModel = new()
            {
                id = cat.Id,
                Name = cat.Category_Name,
            };

            return View(viewModel);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateCategory(CreateCategoryViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var cat = await _ICategoryServices.UpdateAsync(model);
            if (cat == null)
            {
                return BadRequest();
            }

            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> UpdateProduct(int id)
        {
            var product = await _IProductServices.GetByIDAsync(id);

            if (product == null)
            {

                return NotFound();
            }

            CreateProductViewModel viewModel = new()
            {
                id = product.Id,
                Name = product.product_Name,
                CategoryId = product.CategoryId,
                Price = product.Price,
                Description = product.Description,
                currentcover = product.Image_Url,
                Categories = _db.Categories.
                Select(c => new Microsoft.AspNetCore.Mvc.Rendering.SelectListItem { Value = c.Id.ToString(), Text = c.Category_Name }).ToList()

            };

            return View(viewModel);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateProduct(CreateProductViewModel model)
        {
            if (!ModelState.IsValid)
            {
                model.Categories = _db.Categories.
                Select(c => new Microsoft.AspNetCore.Mvc.Rendering.SelectListItem { Value = c.Id.ToString(), Text = c.Category_Name }).ToList();
                return View(model);
            }

            var product = await _IProductServices.UpdateAsync(model);
            if (product == null)
            {
                return BadRequest();
            }

            return RedirectToAction(nameof(Index));
        }





    }
}
