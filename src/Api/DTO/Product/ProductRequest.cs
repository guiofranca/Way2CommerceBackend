namespace Api.DTO.Product;

using Domain.Models;
using Domain.Repositories.Interfaces;

public class ProductRequest
{
    public string Code { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public decimal Price { get; set; }
    public ICollection<int> CategoriesIds { get; set; }

    public ProductRequest(string code, string name, string description, decimal price, ICollection<int> categoriesIds)
    {
        Code = code;
        Name = name;
        Description = description;
        Price = price;
        CategoriesIds = categoriesIds;
    }

    public Product MakeProductFromRequest()
    {
        var product = new Product(Code, Name, Description, Price);

        return product;
    }

    public Product MakeProductFromRequest(int id)
    {
        var product = new Product(id, Code, Name, Description, Price);

        return product;
    }

}
