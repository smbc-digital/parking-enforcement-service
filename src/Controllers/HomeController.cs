using parking_enforcement_service.Models;
using parking_enforcement_service.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using StockportGovUK.AspNetCore.Attributes.TokenAuthentication;
using System;
using System.Threading.Tasks;


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
            _logger.LogDebug(JsonConvert.SerializeObject(parkingEnforcementRequest));

            try
            {
                var result = await _parkingEnforcementService.CreateCase(parkingEnforcementRequest);
                _logger.LogWarning($"Case result: { result }");
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogWarning($"Case an exception has occurred while calling CreateCase, ex: {ex}");
                return StatusCode(500, ex);
            }
        }
    }
}