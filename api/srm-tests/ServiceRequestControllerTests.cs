using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using srm_repositories;
using srm_repositories.Models;
using srm_rest_service.Controllers;
using Xunit;

namespace srm_tests
{
    public class ServiceRequestControllerTests
    {
        [Fact]
        public void Given_No_ServiceRequests_Persisted_GetAll_Returns_NoContent()
        {
            Given.NoServiceRequestsPersisted();
            When.GetAllRequest();
            Then.NoContentReturned();
        }
        
        [Fact]
        public void Given_ServiceRequests_Persisted_GetAll_Returns_Them()
        {
            Given.ServiceRequestsPersisted();
            When.GetAllRequest();
            Then.OkReturned();
            Then.AllTheExistingItemsRetrieved();
        }
        
        [Fact]
        public void Given_ServiceRequest_Persisted_GetByExistingId_Returns_One()
        {
            Given.ServiceRequestPersisted();
            When.GetServiceRequestById();
            Then.OkReturned();
            Then.OneExistingItemRetrieved();
        }
        
        [Fact]
        public void Given_RequestPosted_New_ServiceRequest_Created()
        {
            Given.NewServiceRequest();
            When.PostRequest();
            Then.NewServiceRequestCreated();
        }

        public ServiceRequestControllerTests()
        {
            var logger = new Mock<ILogger<ServiceRequestController>>();
            var serviceRepository = new Mock<IServiceRepository>();
            
            var serviceRequestController = new ServiceRequestController(
                logger.Object,
                serviceRepository.Object);

            var data = new TestData();

            Given = new GivenStructure(serviceRepository);
            When = new WhenStructure(serviceRequestController, data);
            Then = new ThenStructure(serviceRepository, data);
        }

        // ReSharper disable once InconsistentNaming
        private readonly WhenStructure When;
        // ReSharper disable once InconsistentNaming
        private readonly ThenStructure Then;
        // ReSharper disable once InconsistentNaming
        private readonly GivenStructure Given;
    }

    public class TestData
    {
        public Task<IActionResult> ActionResult;
    }

    public class WhenStructure
    {
        private readonly ServiceRequestController _sut;
        private readonly TestData _testData;

        public WhenStructure(
            ServiceRequestController sut,
            TestData testData)
        {
            _sut = sut;
            _testData = testData;
        }

        public void PostRequest()
        {
            _sut.Post(new ServiceRequest());
        }

        public void GetAllRequest()
        {
            _testData.ActionResult = _sut.Get();
        }

        public void GetServiceRequestById()
        {
            _testData.ActionResult = _sut.Get(new Guid("0c0c4dae-b04e-4e43-9f4d-63c32c74df5b"));
        }
    }
    
    public class ThenStructure
    {
        private readonly Mock<IServiceRepository> _serviceRepository;
        private readonly TestData _testData;

        public ThenStructure(
            Mock<IServiceRepository> serviceRepository,
            TestData testData)
        {
            _serviceRepository = serviceRepository;
            _testData = testData;
        }

        public void NewServiceRequestCreated()
        {
        }

        public void NoContentReturned()
        {
            Assert.IsType<NoContentResult>(_testData.ActionResult.Result);
        }

        public void OkReturned()
        {
            Assert.IsType<OkObjectResult>(_testData.ActionResult.Result);
        }

        public void AllTheExistingItemsRetrieved()
        {
            var serviceRequests = (IEnumerable<ServiceRequest>) (_testData.ActionResult.Result as OkObjectResult)?.Value;
            Assert.Equal(2, serviceRequests?.Count());
        }

        public void OneExistingItemRetrieved()
        {
            var request = (ServiceRequest) (_testData.ActionResult.Result as OkObjectResult)?.Value;
            Assert.Equal("0c0c4dae-b04e-4e43-9f4d-63c32c74df5b", request?.Id.ToString());
        }
    }
    
    public class GivenStructure
    {
        private readonly Mock<IServiceRepository> _serviceRepository;

        public GivenStructure(Mock<IServiceRepository> serviceRepository)
        {
            _serviceRepository = serviceRepository;
        }

        public void NewServiceRequest()
        {
        }

        public void NoServiceRequestsPersisted()
        {
            _serviceRepository
                .Setup(_ => _.Get())
                .ReturnsAsync(() => new List<ServiceRequest>());
        }
        
        public void ServiceRequestsPersisted()
        {
            _serviceRepository
                .Setup(_ => _.Get())
                .ReturnsAsync(() 
                    => new List<ServiceRequest>
                    {
                        new ServiceRequest{ Id = new Guid("0c0c4dae-b04e-4e43-9f4d-63c32c74df5b") },
                        new ServiceRequest(),
                    });
        }
        
        public void ServiceRequestPersisted()
        {
            _serviceRepository
                .Setup(_ => _.GetById(It.IsAny<Guid>()))
                .ReturnsAsync(() => new ServiceRequest{ Id = new Guid("0c0c4dae-b04e-4e43-9f4d-63c32c74df5b") });
        }
    }
}