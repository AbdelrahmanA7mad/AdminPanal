using AdminPanal.Data;
using AdminPanal.Models;
using AdminPanal.ViewModel;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;
using System.Text;
using System.Net.Http;

namespace AdminPanal.Services
{
    public class ProductServices : IProductServices
    {
        private readonly AppDbContext _db;
        private readonly string _imagesPath;
        private readonly IWebHostEnvironment _WebHostEnvironment;
        private readonly string _geminiApiKey = "AIzaSyCppY6n1op4hEH-NLfbTPyYlDeQVdXOIQ4";
        private readonly IHttpClientFactory _httpClientFactory;

        public ProductServices(AppDbContext db, IWebHostEnvironment WebHostEnvironment, IHttpClientFactory httpClientFactory, IConfiguration configuration)
        {
            _db = db;
            _WebHostEnvironment = WebHostEnvironment;
            _imagesPath = Path.Combine(_WebHostEnvironment.WebRootPath, "images/");
            _httpClientFactory = httpClientFactory;
        }

        public async Task Create(CreateProductViewModel product)
        {
            var CoverName = $"{Guid.NewGuid()}{Path.GetExtension(product.Image.FileName)}";
            var path = Path.Combine(_imagesPath, CoverName);

            using var stream = File.Create(path);
            await product.Image.CopyToAsync(stream);

            // استدعاء Gemini API لجلب الوصف
            string generatedDescription = await GenerateProductDescription(product.Name);

            Product model = new()
            {
                product_Name = product.Name,
                Price = product.Price,
                CategoryId = product.CategoryId,
                Image_Url = CoverName,
                Description = string.IsNullOrEmpty(product.Description) ? generatedDescription : product.Description,
            };

            _db.Products.Add(model);
            _db.SaveChanges();
        }

        public async Task<string> GenerateProductDescription(string productName)
        {
            using var httpClient = _httpClientFactory.CreateClient();

            var requestData = new
            {
                contents = new[]
                {
            new { parts = new[] { new { text = $"اكتب وصفًا احترافيًا ومقنعًا لمنتج باسم: {productName}. يجب أن يكون الوصف موجزًا ومفيدًا." } } }
        }
            };

            var jsonContent = new StringContent(JsonSerializer.Serialize(requestData), Encoding.UTF8, "application/json");
            var response = await httpClient.PostAsync($"https://generativelanguage.googleapis.com/v1beta/models/gemini-2.0-flash:generateContent?key={_geminiApiKey}", jsonContent);

            if (!response.IsSuccessStatusCode)
                return $"خطأ في الاتصال: {response.StatusCode}";

            var responseString = await response.Content.ReadAsStringAsync();

            try
            {
                using var jsonDoc = JsonDocument.Parse(responseString);
                return jsonDoc.RootElement
                    .GetProperty("candidates")[0]
                    .GetProperty("content")
                    .GetProperty("parts")[0]
                    .GetProperty("text")
                    .GetString() ?? "لم يتم توليد وصف.";
            }
            catch
            {
                return "خطأ في تحليل الاستجابة.";
            }
        }


        public bool DeleteProduct(int id)
        {
            var isdeleted = false;

            var product = _db.Products.Find(id);

            if (product is null)
            {
                return isdeleted;
            }
            _db.Products.Remove(product);

            var effectedrows = _db.SaveChanges();

            if (effectedrows > 0)
            {
                isdeleted = true;
                var cover = Path.Combine(_imagesPath, product.Image_Url);
                File.Delete(cover);
            }

            return isdeleted;

        }


        public async Task<Product?> GetByIDAsync(int id)
        {
            var product =
                _db.Products
                .Include(c => c.Category)
                .AsNoTracking()
                .SingleOrDefaultAsync(i => i.Id == id);

            return await product;
        }

        public async Task<Product?> UpdateAsync(CreateProductViewModel model)
        {
            // Fetch the existing product
            var product = await _db.Products.SingleOrDefaultAsync(c => c.Id == model.id);
            if (product == null)
            {
                return null;
            }

            // Update other fields
            product.product_Name = model.Name;
            product.CategoryId = model.CategoryId;
            product.Price = model.Price;
            product.Description = model.Description;

            // Only update the image if a new one is provided
            if (model.Image != null)
            {
                // Save the new image and update the product's image path
                var newCoverName = await SaveCoverAsync(model.Image!);
                if (!string.IsNullOrEmpty(newCoverName))
                {
                    // Delete the old cover if it exists and was replaced
                    var oldCoverPath = Path.Combine(_imagesPath, product.Image_Url);
                    if (File.Exists(oldCoverPath))
                    {
                        File.Delete(oldCoverPath);
                    }

                    product.Image_Url = newCoverName; // Assign the new image name
                }
            }

            // Save changes to the database
            var effected = await _db.SaveChangesAsync();

            return effected > 0 ? product : null;
        }
        private async Task<string> SaveCoverAsync(IFormFile cover)
        {
            var coverName = $"{Guid.NewGuid()}{Path.GetExtension(cover.FileName)}";
            var path = Path.Combine(_imagesPath, coverName);

            try
            {
                using var stream = File.Create(path);
                await cover.CopyToAsync(stream);
            }
            catch
            {
                // Handle the exception (log it, rethrow it, or return a specific error message)
                throw;
            }

            return coverName;
        }


    }
}