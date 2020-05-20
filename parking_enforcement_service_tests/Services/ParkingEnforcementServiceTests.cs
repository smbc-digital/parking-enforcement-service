using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Moq;
using parking_enforcement_service.Models;
using parking_enforcement_service.Services;
using StockportGovUK.NetStandard.Gateways.VerintServiceGateway;
using StockportGovUK.NetStandard.Gateways.Response;
using StockportGovUK.NetStandard.Models.Verint;
using Xunit;

namespace parking_enforcement_service_tests.Services
{
    public class ParkingEnforcementServiceTests
    {
        private Mock<IVerintServiceGateway> _mockVerintServiceGateway = new Mock<IVerintServiceGateway>();
        private ParkingEnforcementService _service;

        ParkingEnforcementRequest _parkingEnforcementRequestData = new ParkingEnforcementRequest
        {
            FirstName = "Joe",
            LastName = "Bloggs",
            Email = "joe@test.com",
            Phone = "0161 123 1234",
            MoreDetails = "test details",
            FurtherInformation = "test info",
            StreetAddress = new StockportGovUK.NetStandard.Models.Addresses.Address
            {
                AddressLine1 = "100 Red road",
                AddressLine2 = "",
                Postcode = "SK2 9FT",
            },
            CustomersAddress = new StockportGovUK.NetStandard.Models.Addresses.Address
            {
                AddressLine1 = "100 Green road",
                AddressLine2 = "",
                Postcode = "SK2 9FT",
            }
        };

        //public ParkingEnforcementServiceTests()
        //{
        //    _service = new ParkingEnforcementService(_mockVerintServiceGateway.Object);
        //}

        [Fact]
        public async Task CreateCase_ShouldReThrowCreateCaseException_CaughtFromVerintGateway()
        {
            _mockVerintServiceGateway
                .Setup(_ => _.CreateCase(It.IsAny<Case>()))
                .Throws(new Exception("TestException"));

            var result = await Assert.ThrowsAsync<Exception>(() => _service.CreateCase(_parkingEnforcementRequestData));
            Assert.Contains($"CRMService CreateParkingEnforcementRequest: an exception has occured while creating the case in verint service", result.Message);
        }

        [Fact]
        public async Task CreateCase_ShouldThrowException_WhenIsNotSuccessStatusCode()
        {
            _mockVerintServiceGateway
                .Setup(_ => _.CreateCase(It.IsAny<Case>()))
                .ReturnsAsync(new HttpResponse<string>
                {
                    IsSuccessStatusCode = false
                });

            _ = await Assert.ThrowsAsync<Exception>(() => _service.CreateCase(_parkingEnforcementRequestData));
        }

        [Fact]
        public async Task CreateCase_ShouldReturnResponseContent()
        {
            
            _mockVerintServiceGateway
                .Setup(_ => _.CreateCase(It.IsAny<Case>()))
                .ReturnsAsync(new HttpResponse<string>
                {
                    IsSuccessStatusCode = true,
                    ResponseContent = "test"
                });

            var result = await _service.CreateCase(_parkingEnforcementRequestData);

            Assert.Contains("test", result);
        }

        [Fact]
        public async Task CreateCase_ShouldCallVerintGatewayWithCRMCase()
        {
            Case crmCaseParameter = null;

            _mockVerintServiceGateway
                .Setup(_ => _.CreateCase(It.IsAny<Case>()))
                .Callback<Case>(_ => crmCaseParameter = _)
                .ReturnsAsync(new HttpResponse<string>
                {
                    IsSuccessStatusCode = true,
                    ResponseContent = "test"
                });

            _ = await _service.CreateCase(_parkingEnforcementRequestData);
            
            _mockVerintServiceGateway.Verify(_ => _.CreateCase(It.IsAny<Case>()), Times.Once);

            Assert.NotNull(crmCaseParameter);
            //Assert.Equal(_parkingEnforcementRequestData.StreetAddress.AddressLine1, crmCaseParameter.Customer.Address.AddressLine1);
            //Assert.Equal(_parkingEnforcementRequestData.StreetAddress.AddressLine2, crmCaseParameter.Customer.Address.AddressLine2);
            //Assert.Equal(_parkingEnforcementRequestData.StreetAddress.Town, crmCaseParameter.Customer.Address.AddressLine3);
            //Assert.Equal(_parkingEnforcementRequestData.StreetAddress.Postcode, crmCaseParameter.Customer.Address.Postcode);
            Assert.Equal(_parkingEnforcementRequestData.CustomersAddress.AddressLine1, crmCaseParameter.Customer.Address.AddressLine1);
            Assert.Equal(_parkingEnforcementRequestData.CustomersAddress.AddressLine2, crmCaseParameter.Customer.Address.AddressLine2);
            Assert.Equal(_parkingEnforcementRequestData.CustomersAddress.Town, crmCaseParameter.Customer.Address.AddressLine3);
            Assert.Equal(_parkingEnforcementRequestData.CustomersAddress.Postcode, crmCaseParameter.Customer.Address.Postcode);
            Assert.Null(crmCaseParameter.Customer.Address.UPRN);
            Assert.Null(crmCaseParameter.Customer.Address.Reference);
        }

        [Fact]
        public async Task GenerateDescription_ShouldCallVerintGatewayAndGenerateDescription()
        {
            var moreDetails = _parkingEnforcementRequestData.MoreDetails;
            var furtherInformation = _parkingEnforcementRequestData.FurtherInformation;

            Case crmCaseParameter = null;

            _mockVerintServiceGateway
                .Setup(_ => _.CreateCase(It.IsAny<Case>()))
                .Callback<Case>(_ => crmCaseParameter = _)
                .ReturnsAsync(new HttpResponse<string>
                {
                    IsSuccessStatusCode = true,
                    ResponseContent = "test"
                });

            _ = await _service.CreateCase(_parkingEnforcementRequestData);

            _mockVerintServiceGateway.Verify(_ => _.CreateCase(It.IsAny<Case>()), Times.Once);

            Assert.NotNull(crmCaseParameter);

            Assert.Contains(moreDetails, crmCaseParameter.Description);
            Assert.Contains(furtherInformation, crmCaseParameter.Description);            
        }
    }
}

