using System.ComponentModel.DataAnnotations.Schema;

namespace AdminPanal.Models
{
    public class ProductImage
    {
        public int Id { get; set; }

        public int ProductId { get; set; }

        [ForeignKey("ProductId")]
        public virtual Product Product { get; set; }

        public string ImageUrl { get; set; } 
    }
}
