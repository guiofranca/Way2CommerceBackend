using Domain.Models;
using Domain.Repositories.Interfaces;
using Domain.Repositories.Interfaces.Shared;
using Mysql.Context;
using Mysql.Repositories.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mysql.Repositories;

public class CategoryRepository : BaseRepository<Category>, ICategoryRepository
{
    public CategoryRepository(DataContext dataContext) : base(dataContext) { }
}

