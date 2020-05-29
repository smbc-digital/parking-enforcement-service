using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using parking_enforcement_service.Utils.HealthChecks;
using parking_enforcement_service.Utils.ServiceCollectionExtensions;
using StockportGovUK.AspNetCore.Availability;
using StockportGovUK.AspNetCore.Availability.Middleware;
using StockportGovUK.AspNetCore.Middleware;
using StockportGovUK.NetStandard.Gateways;
using System.Diagnostics.CodeAnalysis;

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
            services.AddMvc().AddNewtonsoftJson();
            services.AddControllers().AddNewtonsoftJson();
            services.AddResilientHttpClients<IGateway, Gateway>(Configuration);
            services.RegisterServices();
            services.AddAvailability();
            services.AddSwagger();
            services.AddHealthChecks()
                .AddCheck<TestHealthCheck>("TestHealthCheck");

        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsEnvironment("local"))
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseHsts();

            }
            app.UseHttpsRedirection();
            app.UseRouting();
            app.UseEndpoints(endpoints => endpoints.MapControllers());

            app.UseMiddleware<Availability>();
            app.UseMiddleware<ApiExceptionHandling>();
            app.UseHealthChecks("/healthcheck", HealthCheckConfig.Options);
           
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("v1/swagger.json", "Parking Enforcement Service API");
            });
        }
    }
}
