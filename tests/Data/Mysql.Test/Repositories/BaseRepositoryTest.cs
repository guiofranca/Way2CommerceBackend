using Domain.Models;
using Mysql.Repositories;
using Mysql.Test.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace Mysql.Test.Repositories;

public class BaseRepositoryTest
{
    private readonly CategoryRepository _categoryRepository;

    public BaseRepositoryTest()
    {
        var dataContext = new DataContextInMemory().GetContext();
        _categoryRepository = new CategoryRepository(dataContext);
    }

    public async void Seed(int amount = 3)
    {
        for(int i = 0; i < amount; i++)
        {
            await _categoryRepository.CreateAsync(new Category() { Name = $"Category{i}" });
        }

        return;
    }

    [Fact]
    public async void CanCreateAndRetrieve()
    {
        int category1Id = await _categoryRepository.CreateAsync(new Category() { Name = "Category1" });
        int category2Id = await _categoryRepository.CreateAsync(new Category() { Name = "Category2" });

        Category category1 = await _categoryRepository.GetByIdAsync(1);
        Category category2 = await _categoryRepository.GetByIdAsync(2);

        Assert.Equal(1, category1Id);
        Assert.Equal(2, category2Id);
        Assert.Equal("Category1", category1.Name);
        Assert.Equal("Category2", category2.Name);
    }

    [Fact]
    public async void CanUpdate()
    {
        Seed();

        Category category = await _categoryRepository.GetByIdAsync(1);
        category.Name = "Changed";
        await _categoryRepository.UpdateAsync(category);
        category = await _categoryRepository.GetByIdAsync(1);

        Assert.Equal("Changed", category.Name);
    }

    [Fact]
    public async void GetAllMustHaveTotalCountAfterSeed()
    {
        int amount = new Random().Next(5, 20);
        Seed(amount);

        IEnumerable<Category> categories = await _categoryRepository.GetAllAsync();

        Assert.Equal(amount, categories.Count());
    }

    [Fact]
    public async void GetByIdsMustBringAList()
    {
        Seed();

        List<int> ids = new List<int>() { 1, 2 };

        var categories = await _categoryRepository.GetByIdAsync(ids);

        Assert.Equal(2, categories.Count());
    }

    [Fact]
    public void GetAnInexistingEntityThrowsException()
    {
        Assert.ThrowsAsync<Exception>(() => _categoryRepository.GetByIdAsync(1));
    }

    [Fact]
    public async void DeleteMustReturnTrueAndSetDeletetAt()
    {
        Seed();

        var category = await _categoryRepository.GetByIdAsync(1);
        bool deleted = await _categoryRepository.DeleteAsync(category);

        Assert.True(deleted);
        Assert.NotNull(category.DeletedAt);
    }

    [Fact]
    public async void DeletedMustReturnFalseWhenDeletingAgain()
    {
        Seed();

        var category = await _categoryRepository.GetByIdAsync(1);
        await _categoryRepository.DeleteAsync(category);
        bool deleted = await _categoryRepository.DeleteAsync(category);

        Assert.False(deleted);
    }

    [Fact]
    public async void DeletedMustBeRestored()
    {
        Seed();

        var category = await _categoryRepository.GetByIdAsync(1);
        await _categoryRepository.DeleteAsync(category);
        bool restored = await _categoryRepository.RestoreAsync(category.Id);

        Assert.True(restored);
    }

    [Fact]
    public void RestorationMustThrowExceptionWhenNotFound()
    {
        Assert.ThrowsAsync<Exception>(() => _categoryRepository.RestoreAsync(1));
    }
}
