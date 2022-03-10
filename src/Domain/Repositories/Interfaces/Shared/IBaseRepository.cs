using Domain.Models.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Repositories.Interfaces.Shared
{
    public interface IBaseRepository<T> where T : BaseModel
    {
        public Task<T> GetByIdAsync(int id);
        public Task<IEnumerable<T>> GetByIdAsync(IEnumerable<int> ids);
        public Task<IEnumerable<T>> GetAllAsync();
        public Task<int> CreateAsync(T model);
        public Task UpdateAsync(T model);
        public Task<bool> DeleteAsync(T model);
        public Task<bool> RestoreAsync(int id);
    }
}
