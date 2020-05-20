using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Moq;
using parking_enforcement_service.Controllers;
using parking_enforcement_service.Models;
using parking_enforcement_service.Services;
using StockportGovUK.NetStandard.Models.Addresses;
using Xunit;

namespace parking_enforcement_service_tests.Controllers
{
    public class HomeControllerTest
    {
        private readonly HomeController _homeController;
        private readonly Mock<IParkingEnforcementService> _mockParkingEnforcementService = new Mock<IParkingEnforcementService>();

        public HomeControllerTest()
        {
            _homeController = new HomeController(Mock.Of<ILogger<HomeController>>(), _mockParkingEnforcementService.Object);
        }

        [Fact]
        public async Task Post_ShouldCallCreateCase()
        {
            _mockParkingEnforcementService
                .Setup(_ => _.CreateCase(It.IsAny<ParkingEnforcementRequest>()))
                .ReturnsAsync("test");

            var result = await _homeController.Post(null);

            _mockParkingEnforcementService
                .Verify(_ => _.CreateCase(null), Times.Once);
        }

        [Fact]
        public async Task Post_ReturnOkActionResult()
        {
            _mockParkingEnforcementService
                .Setup(_ => _.CreateCase(It.IsAny<ParkingEnforcementRequest>()))
                .ReturnsAsync("test");

            var result = await _homeController.Post(null);

            Assert.Equal("OkObjectResult", result.GetType().Name);
        }
    }
}
