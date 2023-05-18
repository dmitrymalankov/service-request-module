using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mime;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using srm_repositories;
using srm_repositories.Models;

namespace srm_rest_service.Controllers
{
    [ApiController]
    [Route("/api/servicerequest")]
    public class ServiceRequestController : ControllerBase
    {
        private readonly ILogger<ServiceRequestController> _logger;
        private readonly IRepository<ServiceRequest> _serviceRepository;

        public ServiceRequestController(
            ILogger<ServiceRequestController> logger,
            IRepository<ServiceRequest> serviceRepository)
        {
            _logger = logger;
            _serviceRepository = serviceRepository;
        }

        /// <summary>
        /// Get all <see cref="ServiceRequest"/> items.
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<ServiceRequest>))]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<IActionResult> Get()
        {
            var serviceRequests = await _serviceRepository.Get();

            return serviceRequests == null || !serviceRequests.Any()
                ? NoContent()
                : Ok(serviceRequests);
        }
        
        /// <summary>
        /// Get <see cref="ServiceRequest"/> item by id.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{id:guid}")]
        [ApiConventionMethod(typeof(DefaultApiConventions), nameof(DefaultApiConventions.Get))]
        public async Task<IActionResult> Get([FromRoute] Guid id)
        {
            if (id == Guid.Empty)
            {
                return BadRequest();
            }

            ServiceRequest serviceRequest;
            try
            {
                serviceRequest = await _serviceRepository.GetById(id);
            }
            catch (Exception e)
            {
                _logger.LogError($"Method '{nameof(Get)}' failed reading data by `{id}`", e);
                return InternalServerError();
            }

            return serviceRequest switch
            {
                null => NotFound(),
                _ => Ok(serviceRequest)
            };
        }

        /// <summary>
        /// Modifies <see cref="ServiceRequest"/> item by id.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="updatedServiceRequest"></param>
        /// <returns></returns>
        [HttpPut("{id:guid}")]
        [Consumes(MediaTypeNames.Application.Json)]
        [ApiConventionMethod(typeof(DefaultApiConventions), nameof(DefaultApiConventions.Put))]
        public async Task<IActionResult> Put(
            [FromRoute] Guid id,
            [FromBody] ServiceRequest updatedServiceRequest)
        {
            var serviceRequestId = id == Guid.Empty ? updatedServiceRequest.Id : id;
            if (serviceRequestId == Guid.Empty )
            {
                return BadRequest();
            }

            var existingServiceRequestById = await _serviceRepository.GetById(serviceRequestId);
            if (existingServiceRequestById == null)
            {
                return NotFound();
            }

            try
            {
                 await _serviceRepository.Update(serviceRequestId, updatedServiceRequest, existingServiceRequestById);
            }
            catch (Exception e)
            {
                _logger.LogError($"Method '{nameof(Put)}' failed modifying service request by id:`{id}`", e);
                return InternalServerError();
            }
            
            return NoContent();
        }

        /// <summary>
        /// Creates new <see cref="ServiceRequest"/> item.
        /// </summary>
        /// <param name="serviceRequest"></param>
        /// <returns></returns>
        [HttpPost]
        [Consumes(MediaTypeNames.Application.Json)]
        [ApiConventionMethod(typeof(DefaultApiConventions), nameof(DefaultApiConventions.Post))]
        public async Task<IActionResult> Post([FromBody] ServiceRequest serviceRequest)
        {
            if (serviceRequest == null)
            {
                return BadRequest();
            }

            try
            {
                await _serviceRepository.Create(serviceRequest);
            }
            catch (Exception e)
            {
                _logger.LogError($"Method '{nameof(Post)}' failed creating new service request.", e);
                return InternalServerError();
            }
            
            return CreatedAtAction(nameof(Post), new { id = serviceRequest.Id }, serviceRequest);
        }
        
        /// <summary>
        /// Deletes <see cref="ServiceRequest"/> item by id.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete("{id:guid}")]
        [ApiConventionMethod(typeof(DefaultApiConventions), nameof(DefaultApiConventions.Delete))]
        public async Task<IActionResult> Delete([FromRoute] Guid id)
        {
            if (id == Guid.Empty)
            {
                return BadRequest();
            }

            var serviceRequestById = _serviceRepository.GetById(id);
            if (serviceRequestById == null)
            {
                return NotFound();
            }
            
            try
            {
                await _serviceRepository.Delete(id);
            }
            catch (Exception e)
            {
                _logger.LogError($"Method '{nameof(Delete)}' failed deleting service request '{id}'.", e);
                return InternalServerError();
            }
            
            return Ok($"Service request id:'{id}' got deleted successfully.");
        }
        
        private IActionResult InternalServerError() => Problem("Internal server error, please contact support.");
    }
}