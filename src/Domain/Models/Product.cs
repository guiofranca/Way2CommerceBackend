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
        public IEnumerable<Category> Categories { get; set; } = new List<Category>();
        public Product(string code, string name, string description, decimal price)
        {
            Code = code;
            Name = name;
            Description = description;
            Price = price;
        }

        public Product(int id, string code, string name, string description, decimal price)
        {
            Id = id;
            Code = code;
            Name = name;
            Description = description;
            Price = price;
        }

        public Product() { }
    }
}
