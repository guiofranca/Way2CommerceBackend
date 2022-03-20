using Domain.Models.Relations;
using Domain.Models.Shared;

namespace Domain.Models
{
    public class Category : BaseModel
    {
        public string Name { get; set; } = string.Empty;
        public IEnumerable<Product> Products { get; set; } = new List<Product>();
    }
}
