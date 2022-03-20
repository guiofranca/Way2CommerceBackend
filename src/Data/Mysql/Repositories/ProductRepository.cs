using Domain.Models;
using Domain.Models.Relations;
using Domain.Repositories.Interfaces;
using Domain.Repositories.Interfaces.Shared;
using Microsoft.EntityFrameworkCore;
using Mysql.Context;
using Mysql.Repositories.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mysql.Repositories;

public class ProductRepository : BaseRepository<Product>, IProductRepository
{
    public ProductRepository(DataContext dataContext) : base(dataContext) { }

    public override async Task<IEnumerable<Product>> GetAllAsync()
    {
        return await _db.Set<Product>()
            .Include(p => p.Categories)
            //.ThenInclude(pc => pc.Category)
            //.Where(p => p.DeletedAt == null)
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<Product?> GetByCodeAsync(string code, int? excludedId = null)
    {
        var query = _db.Set<Product>()
            .Where(p => p.Code == code);

        if (excludedId != null) query = query.Where(p => p.Id != excludedId);
            
        return await query.AsNoTracking()
            .FirstOrDefaultAsync();
    }

    public override async Task<Product> GetByIdAsync(int id)
    {
        Product? product = await _db.Set<Product>()
            .Where(p => p.Id == id)
            //.Where(p => p.DeletedAt == null)
            .Include(p => p.Categories)
            //.ThenInclude(pc => pc.Category)
            .AsNoTracking()
            .FirstOrDefaultAsync();

        if (product == null) throw new Exception("Product not found");

        return product;
    }

    public async Task SyncCategoriesAsync(Product product, IEnumerable<int> categoryIds)
    {
        var dettachProductCategories = await _db.ProductCategories
            .Where(pc => pc.ProductId == product.Id)
            .ToListAsync();

        _db.ProductCategories
            .RemoveRange(dettachProductCategories);

        List<ProductCategory> attachProductCategories = new List<ProductCategory>();

        categoryIds.ToList()
            .ForEach(c => attachProductCategories
                .Add(new ProductCategory { 
                    CategoryId = c,
                    ProductId = product.Id 
                })
            );

        await _db.ProductCategories.AddRangeAsync(attachProductCategories);
        await _db.SaveChangesAsync();

        return;
    }
}
