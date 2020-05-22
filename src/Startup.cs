using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using parking_enforcement_service.Models;
using parking_enforcement_service.Services;
using parking_enforcement_service.Utils.HealthChecks;
using StockportGovUK.AspNetCore.Availability;
using StockportGovUK.AspNetCore.Middleware;
using StockportGovUK.NetStandard.Gateways;
using Swashbuckle.AspNetCore.Swagger;

namespace parking_enforcement_service
{
    [ExcludeFromCodeCoverage]
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);
            services.AddMvc().AddMvcOptions(_ => _.AllowEmptyInputInBodyModelBinding = true);

            services.AddHealthChecks()
                .AddCheck<TestHealthCheck>("TestHealthCheck");
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new Info { Title = "parking_enforcement_service API", Version = "v1" });
                c.AddSecurityDefinition("Bearer", new ApiKeyScheme
                {
                    Description = "Authorization using the Bearer scheme. Example: \"Authorization: Bearer {token}\"",
                    Name = "Authorization",
                    In = "header",
                    Type = "apiKey"
                });
                c.AddSecurityRequirement(new Dictionary<string, IEnumerable<string>>
                {
                    {"Bearer", new string[] { }},
                });
            });

            services.AddAvailability();
            services.AddTransient<EventCodeConfiguration>(_ => new EventCodeConfiguration(Convert.ToInt32(Configuration["ShieldedEventCode"]), Convert.ToInt32(Configuration["NonShieldedEventCode"])));
            services.AddResilientHttpClients<IGateway, Gateway>(Configuration);
            services.AddTransient<IParkingEnforcementService, ParkingEnforcementService>();
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseHsts();
                app.UseHttpsRedirection();
            }

            app.UseMiddleware<ExceptionHandling>();
            app.UseHealthChecks("/healthcheck", HealthCheckConfig.Options);
            app.UseMvc();
            app.UseSwagger();

            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("v1/swagger.json", "Parking Enforcement Service API");
            });
        }
    }
}
