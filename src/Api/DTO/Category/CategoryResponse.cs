namespace Api.DTO.Category;

using Domain.Models;

public class CategoryResponse
{
    public int Id { get; set; }
    public string Name { get; set; }

    public CategoryResponse(int id, string name)
    {
        Id = id;
        Name = name;
    }

    public CategoryResponse(Category category)
    {
        Id = category.Id;
        Name = category.Name;
    }
}
