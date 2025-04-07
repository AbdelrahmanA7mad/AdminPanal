using AdminPanal.Models;
using AdminPanal.ViewModel;

namespace AdminPanal.Services
{
    public interface IProductServices
    {
        Task Create(CreateProductViewModel product);

        bool DeleteProduct(int id);

        Task<Product?> GetByIDAsync(int id);

        Task<Product?> UpdateAsync(CreateProductViewModel model);

        Task<string> GenerateProductDescription(string productName);

    }
}
