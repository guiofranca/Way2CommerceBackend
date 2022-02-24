using Domain.Models.Shared;
using Domain.Repositories.Interfaces.Shared;
using Microsoft.EntityFrameworkCore;
using Mysql.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mysql.Repositories.Shared;
public class BaseRepository<T> : IBaseRepository<T> where T : BaseModel
{
    protected readonly DataContext _db;
    public BaseRepository(DataContext dataContext)
    {
        _db = dataContext;
    }

    public virtual async Task<IEnumerable<T>> GetAllAsync()
    {
        return await _db.Set<T>()
            .Where(t => t.DeletedAt == null)
            .AsNoTracking()
            .ToListAsync();
    }

    public virtual async Task<T> GetByIdAsync(int id)
    {
        T? model = await _db.Set<T>()
            .Where(t => t.Id.Equals(id))
            .Where(t => t.DeletedAt == null)
            .AsNoTracking()
            .FirstOrDefaultAsync();

        if(model == null) throw new Exception($"Not found");

        return model;
    }

    public virtual async Task<IEnumerable<T>> GetByIdAsync(IEnumerable<int> ids)
    {
        IEnumerable<T> models = await _db.Set<T>()
            .Where(t => ids.Contains(t.Id))
            .Where(t => t.DeletedAt == null)
            .AsNoTracking()
            .ToListAsync();

        return models;
    }

    public virtual async Task UpdateAsync(T model)
    {
        model.UpdatedAt = DateTime.Now;
        _db.Set<T>().Update(model);
        _db.Entry(model).Property(m => m.CreatedAt).IsModified = false;
        await _db.SaveChangesAsync();
    }

    public virtual async Task<bool> DeleteAsync(int id)
    {
        T model = await GetByIdAsync(id);
        if (model == null) return false;
            
        model.DeletedAt = DateTime.Now;
        await UpdateAsync(model);

        return true;
    }

    public virtual async Task<bool> RestoreAsync(int id)
    {
        T? model = await _db.Set<T>()
            .Where(t => t.Id.Equals(id))
            .Where(t => t.DeletedAt != null)
            .FirstOrDefaultAsync();

        if (model == null) throw new Exception($"Not found");

        model.DeletedAt = null;
        model.UpdatedAt = DateTime.Now;

        await UpdateAsync(model);
        return true;
    }

    public virtual async Task<int> CreateAsync(T model)
    {
        model.CreatedAt = DateTime.Now;
        model.UpdatedAt= DateTime.Now;
        _db.Set<T>().Add(model);
        await _db.SaveChangesAsync();

        return model.Id;
    }
}
