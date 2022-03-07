using Domain.Models;
using Domain.Repositories.Interfaces;
using Mysql.Context;
using Mysql.Repositories.Shared;

namespace Mysql.Repositories;

public class CategoryRepository : BaseRepository<Category>, ICategoryRepository
{
    public CategoryRepository(DataContext dataContext) : base(dataContext) { }
}

