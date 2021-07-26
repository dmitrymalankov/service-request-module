using System;
using System.Collections.Generic;
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
        private readonly IServiceRepository _serviceRepository;

        public ServiceRequestController(
            ILogger<ServiceRequestController> logger,
            IServiceRepository serviceRepository)
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
            return Ok(await _serviceRepository.Get());
        }
        
        /// <summary>
        /// Get <see cref="ServiceRequest"/> item by id.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{id:guid}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ServiceRequest))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
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
                _logger.LogError($"Method 'GetById' failed reading data by `{id}`", e);
                return InternalServerError();
            }
            
            return Ok(serviceRequest);
        }
        
        /// <summary>
        /// Modifies <see cref="ServiceRequest"/> item by id.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPut("{id:guid}")]
        public async Task<IActionResult> Put([FromRoute] Guid id)
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
                 await _serviceRepository.Update(id);
            }
            catch (Exception e)
            {
                _logger.LogError($"Method 'Put' failed modifying service request by id:`{id}`", e);
                return InternalServerError();
            }
            
            return Ok($"Service request id:'{id}' modified successfully.");
        }

        /// <summary>
        /// Creates new <see cref="ServiceRequest"/> item.
        /// </summary>
        /// <param name="serviceRequest"></param>
        /// <returns></returns>
        [HttpPost]
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
                _logger.LogError("Method 'Post' failed creating new service request.", e);
                return InternalServerError();
            }
            
            return Ok(serviceRequest);
        }
        
        /// <summary>
        /// Deletes <see cref="ServiceRequest"/> item by id.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete]
        public async Task<IActionResult> Delete([FromBody] Guid id)
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
                _logger.LogError("Method 'Post' failed creating new service request.", e);
                return InternalServerError();
            }
            
            return Ok($"Service request id:'{id}' got deleted successfully.");
        }
        
        private IActionResult InternalServerError() => Problem("Internal server error, please contact support.");
    }
}