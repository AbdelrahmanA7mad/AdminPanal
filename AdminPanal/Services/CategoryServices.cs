using AdminPanal.Data;
using AdminPanal.Models;
using AdminPanal.ViewModel;
using Microsoft.EntityFrameworkCore;

namespace AdminPanal.Services
{
    public class CategoryServices : ICategoryServices
    {
        private readonly AppDbContext _db;
        private readonly string _imagesPath;
        private readonly IWebHostEnvironment _WebHostEnvironment;

        public CategoryServices(AppDbContext db, IWebHostEnvironment WebHostEnvironment)
        {
            _db = db;
            _WebHostEnvironment = WebHostEnvironment;
            _imagesPath = "D:\\Images\\";
        }


        public CategoryServices(AppDbContext db)
        {
            _db = db;
        }
        public async Task Create(CreateCategoryViewModel category)
        {
            Category cat = new()
            {
                Category_Name = category.Name,
            };
            _db.Categories.Add(cat);
            _db.SaveChanges();

        }

        public bool DeleteCategory(int id)
        {
            var isdeleted = false;
            var cat = _db.Categories.Find(id);

            if (cat is null)
            {
                return isdeleted;
            }
            _db.Categories.Remove(cat);
            handleDelete(id);
            var effectedrows = _db.SaveChanges();

            if (effectedrows > 0)
            {
                isdeleted = true;
            }

            return isdeleted;


        }

        public void handleDelete(int id)
        {
            var cat = _db.Categories.Include(a => a.Products).FirstOrDefault(a => a.Id == id);
            if (cat is null)
            {
                Console.WriteLine("error");
            }
            foreach (var product in cat.Products)
            {
                File.Delete(Path.Combine(_imagesPath, product.Image_Url));
            }

        }



        public async Task<Category?> GetByIDAsync(int id)
        {
            var cat =
                _db.Categories
                .AsNoTracking()
                .SingleOrDefaultAsync(i => i.Id == id);

            return await cat;
        }

        public async Task<Category?> UpdateAsync(CreateCategoryViewModel model)
        {
            var cat = await _db.Categories.SingleOrDefaultAsync(c => c.Id == model.id);
            if (cat == null)
            {
                return null;
            }

            cat.Category_Name = model.Name;
            await _db.SaveChangesAsync();

            return cat;
        }
    }

}
