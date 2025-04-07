using AdminPanal.Models;
using AdminPanal.ViewModel;

namespace AdminPanal.Services
{
    public interface ICategoryServices
    {
        Task Create(CreateCategoryViewModel category);

        bool DeleteCategory(int id);

        void handleDelete(int id);

        Task<Category?> GetByIDAsync(int id);

        Task<Category?> UpdateAsync(CreateCategoryViewModel model);

    }
}
