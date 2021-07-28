using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using srm_repositories.Models;

namespace srm_repositories
{
    /// <summary>
    /// Repository class that uses ServicesRequests dictionary to store data.
    /// </summary>
    public class ServiceRepository : IRepository<ServiceRequest>
    {
        private ConcurrentDictionary<Guid,ServiceRequest> _serviceRequests;

        /// <summary>
        /// All the service requests.
        /// </summary>
        private ConcurrentDictionary<Guid, ServiceRequest> ServiceRequests 
            => _serviceRequests ??= new ConcurrentDictionary<Guid, ServiceRequest>();

        /// <summary>
        /// Returns <see cref="ServiceRequest"/> item by id.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<ServiceRequest> GetById(Guid id)
        {
            return await Task.Run(() =>
            {
                ServiceRequests.TryGetValue(id, out var serviceRequest);
                return serviceRequest;
            });
        }

        /// <summary>
        /// Returns all the <see cref="ServiceRequest"/> items.
        /// </summary>
        /// <returns></returns>
        public async Task<IEnumerable<ServiceRequest>> Get()
        {
            return await Task.Run(() =>
            { 
                // assumption: we do not want to return completed (soft-deleted) requests
                // note: we need to think about implementing paging and filtering here 
                return ServiceRequests.Values.Where(_ => _.CurrentStatus != CurrentStatus.Complete);
            });
        }

        /// <summary>
        /// Updates <see cref="ServiceRequest"/> item by id.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="updatedItem"></param>
        /// <param name="existingItemById"></param>
        public async Task Update(
            Guid id,
            ServiceRequest updatedItem,
            ServiceRequest existingItemById)
        {
            await Task.Run(() =>
            {
                // we need to make sure these properties won't be erased
                updatedItem.Id = id;
                updatedItem.CreatedBy ??= existingItemById.CreatedBy;
                updatedItem.CreatedDate = existingItemById.CreatedDate;
                
                updatedItem.LastModifiedBy ??= "System admin";
                updatedItem.LastModifiedDate = DateTime.UtcNow;
                ServiceRequests.TryUpdate(id, updatedItem, existingItemById);
            });
        }

        /// <summary>
        /// Creates <see cref="ServiceRequest"/> item.
        /// </summary>
        /// <param name="serviceRequest"></param>
        public async Task Create(ServiceRequest serviceRequest)
        {
            if (serviceRequest.Id == Guid.Empty)
            {
                serviceRequest.Id = Guid.NewGuid();
            }
            
            await Task.Run(() =>
            {
                serviceRequest.CreatedBy ??= "System admin";
                serviceRequest.CreatedDate = DateTime.UtcNow;
                
                serviceRequest.CurrentStatus = CurrentStatus.Created;
                ServiceRequests.TryAdd(serviceRequest.Id, serviceRequest);
            });
        }

        /// <summary>
        /// Soft deletes <see cref="ServiceRequest"/> item.
        /// </summary>
        /// <param name="id"></param>
        public async Task Delete(Guid id)
        {
            await Task.Run(() =>
            {
                ServiceRequests.TryGetValue(id, out var serviceRequest);
                if (serviceRequest == null) return;
                
                serviceRequest.CurrentStatus = CurrentStatus.Complete;
                
                // assumption: we soft delete items, in case we need to actually hard delete them - TryRemove() should be used instead.
                var serviceRequestCompleted = ServiceRequests.TryUpdate(id, serviceRequest, new ServiceRequest {Id = id});

                if (serviceRequestCompleted)
                {
                    
                }
            });
        }
    }
}