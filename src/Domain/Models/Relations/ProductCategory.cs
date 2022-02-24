using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Models.Relations;

public class ProductCategory
{

    public Product Product { get; set; }
    public int ProductId { get; set; }
    public Category Category { get; set; }
    public int CategoryId { get; set; }

    public ProductCategory(Product product, Category category)
    {
        Product = product;
        Category = category;
    }

    public ProductCategory() { }
}
