namespace AdminPanal.Models
{
    public class Product
    {
        public int Id { get; set; }
        public string ?product_Name { get; set; }
        public Decimal Price { get; set; }
        public string ? Description { get; set; }

        // main image
        public string ?Image_Url { get; set; }

        // video 
        public string ? ProductVideoUrl { get; set; } 

        // images that ai will create 
        public virtual ICollection<ProductImage> ? Images { get; set; } = new List<ProductImage>();

        public int CategoryId { get; set; }

        public Category Category { get; set; }





    }
}
