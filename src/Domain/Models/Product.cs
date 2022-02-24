using Domain.Models.Relations;
using Domain.Models.Shared;

namespace Domain.Models
{
    public class Product : BaseModel
    {

        public string Code { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public decimal Price { get; set; }
        //public virtual IEnumerable<Category> Categories { get; set; }
        public virtual IEnumerable<ProductCategory> ProductCategories { get; set; }
        public Product(string code, string name, string description, decimal price) //, IEnumerable<Category> categories)
        {
            Code = code;
            Name = name;
            Description = description;
            Price = price;
            //Categories = categories;
        }

        public Product(int id, string code, string name, string description, decimal price) //, IEnumerable<Category> categories)
        {
            Id = id;
            Code = code;
            Name = name;
            Description = description;
            Price = price;
            //Categories = categories;
        }

        public Product() { }
    }
}
