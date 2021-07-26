using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using srm_repositories.Models;

namespace srm_repositories
{
    public class ServiceRepository : IServiceRepository
    {
        public async Task<ServiceRequest> GetById(Guid id)
        {
            return new ServiceRequest();
        }

        public Task<IEnumerable<ServiceRequest>> Get()
        {
            throw new NotImplementedException();
        }

        public Task Update(Guid id)
        {
            throw new NotImplementedException();
        }

        public Task Create(ServiceRequest serviceRequest)
        {
            throw new NotImplementedException();
        }

        public Task Delete(Guid id)
        {
            throw new NotImplementedException();
        }
    }
}