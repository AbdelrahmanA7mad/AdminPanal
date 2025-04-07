using AdminPanal.Models;

namespace AdminPanal.ViewModel
{
    public class ViewModelOfIdnex
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public IEnumerable<Product> Products { get; set; } = Enumerable.Empty<Product>();
    }
}
