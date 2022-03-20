namespace Api.Requests.Product;

using Api.Requests.Category;
using Domain.Models;
public class ProductResponse
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Code { get; set; }
    public string Description { get; set; }
    public decimal Price { get; set; }
    public List<CategoryResponse> Categories { get; set; }

    public ProductResponse(int id, string name, string code, string description, decimal price, List<CategoryResponse> categories)
    {
        Id = id;
        Name = name;
        Code = code;
        Description = description;
        Price = price;
        Categories = categories;
    }

    public ProductResponse(Product product)
    {
        Id = product.Id;
        Name = product.Name;
        Code = product.Code;
        Description = product.Description;
        Price = product.Price;
        //Categories = product.ProductCategories.Select(pc => new CategoryResponse(pc.Category)).ToList();
        Categories = product.Categories.Select(category => new CategoryResponse(category)).ToList();
    }
}
