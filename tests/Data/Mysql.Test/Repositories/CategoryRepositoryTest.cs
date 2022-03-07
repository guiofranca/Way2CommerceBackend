using Domain.Models;
using Mysql.Repositories;
using Mysql.Test.Context;
using Xunit;

namespace Mysql.Test.Repositories;

public class CategoryRepositoryTest
{
    private readonly CategoryRepository _categoryRepository;

    public CategoryRepositoryTest()
    {
        _categoryRepository = new CategoryRepository(new DataContextInMemory().GetContext());
    }

    [Fact]
    public async void CanInsertCategory()
    {
        await _categoryRepository.CreateAsync(new Category() { Name = "Category1" });
        Assert.Equal("Category1", _categoryRepository.GetByIdAsync(1).GetAwaiter().GetResult().Name);
    }
}
