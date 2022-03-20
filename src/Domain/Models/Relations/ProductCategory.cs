using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Models.Relations;

public class ProductCategory
{
    public int ProductId { get; set; }
    public int CategoryId { get; set; }

    public ProductCategory(int productId, int categoryId)
    {
        ProductId = productId;
        CategoryId = categoryId;
    }

    public ProductCategory() { }
}
