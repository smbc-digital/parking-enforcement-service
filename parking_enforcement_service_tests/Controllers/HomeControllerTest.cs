using Microsoft.Extensions.Logging;
using Moq;
using parking_enforcement_service.Controllers;
using parking_enforcement_service.Models;
using parking_enforcement_service.Services;
using StockportGovUK.NetStandard.Models.Addresses;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace parking_enforcement_service_tests.Controllers
{
    public class HomeControllerTest
    {
        private readonly HomeController _controller;
        private readonly Mock<ILogger<HomeController>> _mockLogger = new Mock<ILogger<HomeController>>();
        private readonly Mock<IParkingEnforcementService> _mockCrmService = new Mock<IParkingEnforcementService>();

        public HomeControllerTest()
        {
            _controller = new HomeController(_mockLogger.Object, _mockCrmService.Object);
        }
        [Fact]
        public async void PostCrmCase_ShouldCallParkingEnforcementService()
        {
            //Arrange
            _mockCrmService
                .Setup(_ => _.CreateCase(It.IsAny<ParkingEnforcementRequest>()))
                .ReturnsAsync(It.IsAny<string>());

            var Data = new ParkingEnforcementRequest
            {
                FirstName = "Joe",
                LastName = "Bloggs",
                Email = "joe@test.com",
                Phone = "0161 123 1234",
                MoreDetails = "details",
                FurtherInformation = "further info",

                StreetAddress = new Address
                {
                    SelectedAddress = "Test",
                    AddressLine1 = "100 Green road",
                    AddressLine2 = "",
                    Town = "Stockport",
                    Postcode = "SK2 9FT",
                    PlaceRef = "",
                },

                CustomersAddress = new Address
                {
                    SelectedAddress = "Test",
                    AddressLine1 = "100 Green road",
                    AddressLine2 = "",
                    Town = "Stockport",
                    Postcode = "SK2 9FT",
                    PlaceRef = "",
                }
            };

            // Act
            await _controller.Post(Data);

            // Assert
            _mockCrmService.Verify(_ => _.CreateCase(It.IsAny<ParkingEnforcementRequest>()), Times.Once);
        }
    }
}
