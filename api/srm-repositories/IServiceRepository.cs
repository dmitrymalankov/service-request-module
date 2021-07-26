using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using srm_repositories.Models;

namespace srm_repositories
{
    public interface IServiceRepository
    {
        Task<ServiceRequest> GetById(Guid id);
        Task<IEnumerable<ServiceRequest>> Get();
        Task Update(Guid id);
        Task Create(ServiceRequest serviceRequest);
        Task Delete(Guid id);
    }
}