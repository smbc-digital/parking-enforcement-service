using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using parking_enforcement_service.Models;
using parking_enforcement_service.Services;
using StockportGovUK.AspNetCore.Attributes.TokenAuthentication;

namespace parking_enforcement_service.Controllers
{
    [Produces("application/json")]
    [Route("api/v1/[Controller]")]
    [ApiController]
    [TokenAuthentication]
    public class HomeController : ControllerBase
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IParkingEnforcementService _parkingEnforcementService;

        public HomeController(ILogger<HomeController> logger, IParkingEnforcementService parkingEnforcementService)
        {
            _logger = logger;
            _parkingEnforcementService = parkingEnforcementService;
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody]ParkingEnforcementRequest parkingEnforcementRequest)
        {
            string result = await _parkingEnforcementService.CreateCase(parkingEnforcementRequest);

            return Ok(result);
        }
    }
}