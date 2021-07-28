using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace srm_repositories
{
    public interface IRepository<T>
    {
        Task<T> GetById(Guid id);
        Task<IEnumerable<T>> Get();
        Task Update(Guid id, T updatedItem, T existingItemById);
        Task Create(T serviceRequest);
        Task Delete(Guid id);
    }
}