using System;
namespace parking_enforcement_service.Controllers.HealthCheck.Models
{
    public class HealthCheckModel
    {
        public string AppVersion { get; set; }

        public string Name { get; set; }
    }
}