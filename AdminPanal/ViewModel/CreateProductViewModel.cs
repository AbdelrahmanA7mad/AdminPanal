using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace AdminPanal.ViewModel
{
    public class CreateProductViewModel
    {
        public int ? id { get; set; }
        public string Name { get; set; } = string.Empty;

        public string Description { get; set; }
        public string? currentcover { get; set; }

        public decimal Price { get; set; }
        [Display(Name = "Category")]
        public int CategoryId { get; set; }
        public IEnumerable<SelectListItem> Categories { get; set; } = Enumerable.Empty<SelectListItem>();
        public IFormFile ? Image { get; set; } = default!;


    }
}
