﻿using System;
using System.Diagnostics;
using System.IO;
using Microsoft.AspNetCore.Mvc;
using System.Reflection;
using parking_enforcement_service.Controllers.HealthCheck.Models;

namespace parking_enforcement_service.Controllers.HealthCheck
{
    [Produces("application/json")]
    [Route("api/v1/[Controller]")]
    [ApiController]
    public class HealthCheckController : ControllerBase
    {
        [HttpGet]
        public IActionResult Get()
        {
            var name = Assembly.GetEntryAssembly()?.GetName().Name;
            var assembly = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "parking_enforcement_service.dll");
            var version = FileVersionInfo.GetVersionInfo(assembly).FileVersion;

            return Ok(new HealthCheckModel
            {
                AppVersion = version,
                Name = name
            });
        }
    }
}