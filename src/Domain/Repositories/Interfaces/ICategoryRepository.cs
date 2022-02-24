using Domain.Models;
using Domain.Repositories.Interfaces.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Repositories.Interfaces
{
    public interface ICategoryRepository : IBaseRepository<Category>
    {
    }
}
