using Domain.Models.Relations;
using Domain.Models.Shared;

namespace Domain.Models
{
    public class Category : BaseModel
    {
        public string Name { get; set; } = string.Empty;
        //public ICollection<Product> Products { get; set; } = new List<Product>();
        public IEnumerable<ProductCategory> ProductCategories { get; set; }
    }
}
