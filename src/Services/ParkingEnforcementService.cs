using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using parking_enforcement_service.Helpers;
using parking_enforcement_service.Models;
using StockportGovUK.NetStandard.Gateways.VerintServiceGateway;
using StockportGovUK.NetStandard.Models.Enums;
using StockportGovUK.NetStandard.Models.Verint;

namespace parking_enforcement_service.Services
{
    public class ParkingEnforcementService : IParkingEnforcementService
    {
        private readonly IVerintServiceGateway _VerintServiceGateway;
        private readonly IConfiguration configuration;
        private readonly ILogger<ParkingEnforcementService> _logger;
        private readonly IMailHelper _mailHelper;

        public ParkingEnforcementService(IVerintServiceGateway verintServiceGateway
                                        , IConfiguration iConfig
                                        , ILogger<ParkingEnforcementService> logger
                                        , IMailHelper mailHelper)
        {
            _VerintServiceGateway = verintServiceGateway;
            configuration = iConfig;
            _logger = logger;
            _mailHelper = mailHelper;
        }

        public async Task<string> CreateCase(ParkingEnforcementRequest parkingEnforcementRequest)
        {
            var crmCase = CreateCrmCaseObject(parkingEnforcementRequest);

            try
            {
                var response = await _VerintServiceGateway.CreateCase(crmCase);

                if (!response.IsSuccessStatusCode)
                {
                    throw new Exception("Status code not successful");
                }

                _mailHelper.SendEmail(parkingEnforcementRequest.Reporter, EMailTemplate.ParkingEnforcementRequest, response.ResponseContent);
                return response.ResponseContent;
            }
            catch (Exception ex)
            {
                throw new Exception($"CRMService CreateParkingEnforcementService an exception has occured while creating the case in verint service", ex);
            }
        }

        private Case CreateCrmCaseObject(ParkingEnforcementRequest parkingEnforcementRequest)
        {
            var crmCase = new Case
            {
                EventCode = Int32.Parse(configuration.GetSection("CrmCaseSettings").GetSection("EventCode").Value),
                EventTitle = configuration.GetSection("CrmCaseSettings").GetSection("EventTitle").Value,
                Description = parkingEnforcementRequest.FurtherInformation,
                Classification = configuration.GetSection("CrmCaseSettings").GetSection("Classification").Value,
                Street = new Street
                {
                    Reference = parkingEnforcementRequest.StreetAddress.PlaceRef
                }
            };

            if (!string.IsNullOrEmpty(parkingEnforcementRequest.FirstName) && !string.IsNullOrEmpty(parkingEnforcementRequest.LastName))
            {
                crmCase.Customer = new Customer
                {
                    Forename = parkingEnforcementRequest.FirstName,
                    Surname = parkingEnforcementRequest.LastName
                };

                if (!string.IsNullOrEmpty(parkingEnforcementRequest.Email))
                {
                    crmCase.Customer.Email = parkingEnforcementRequest.Email;
                }

                if (!string.IsNullOrEmpty(parkingEnforcementRequest.Phone))
                {
                    crmCase.Customer.Telephone = parkingEnforcementRequest.Phone;
                }

                if (string.IsNullOrEmpty(parkingEnforcementRequest.CustomersAddress.PlaceRef))
                {
                    crmCase.Customer.Address = new Address
                    {
                        AddressLine1 = parkingEnforcementRequest.CustomersAddress.AddressLine1,
                        AddressLine2 = parkingEnforcementRequest.CustomersAddress.AddressLine2,
                        AddressLine3 = parkingEnforcementRequest.CustomersAddress.Town,
                        Postcode = parkingEnforcementRequest.CustomersAddress.Postcode,
                    };
                }
                else
                {
                    crmCase.Customer.Address = new Address
                    {
                        Reference = parkingEnforcementRequest.CustomersAddress.PlaceRef,
                        UPRN = parkingEnforcementRequest.CustomersAddress.PlaceRef
                    };
                }
            }

            _logger.LogInformation(JsonConvert.SerializeObject(crmCase));
            return crmCase;
        }
    }
}
