using System;
using System.Threading.Tasks;
using StockportGovUK.NetStandard.Gateways.VerintServiceGateway;
using StockportGovUK.NetStandard.Models.Verint;
using parking_enforcement_service.Models;

namespace parking_enforcement_service.Services
{
    public class ParkingEnforcementService : IParkingEnforcementService
    {
        private readonly IVerintServiceGateway _VerintServiceGateway;

        private readonly EventCodeConfiguration _EventCodeConfiguration;

        public ParkingEnforcementService(IVerintServiceGateway verintServiceGateway, EventCodeConfiguration eventCodeConfiguration)
        {
            _VerintServiceGateway = verintServiceGateway;
            _EventCodeConfiguration = eventCodeConfiguration;
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
                EventCode = 4000031,
                EventTitle = "Basic Verint Case",
                Description = GenerateDescription(parkingEnforcementRequest),
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

            return crmCase;
        }

        private string GenerateDescription(ParkingEnforcementRequest parkingEnforcementRequest)
        {
            var description = $@"FirstName: {parkingEnforcementRequest.FirstName}
                                LastName: { parkingEnforcementRequest.LastName}
                                Email: {parkingEnforcementRequest.Email}
                                Phone: {parkingEnforcementRequest.Phone}
                                MoreDetails {parkingEnforcementRequest.MoreDetails}
                                FurtherInformation {parkingEnforcementRequest.FurtherInformation}";

            if (parkingEnforcementRequest.CustomersAddress != null)
            {
                description += $@"AddressLine1: {parkingEnforcementRequest.CustomersAddress.AddressLine1}
                                AddressLine2: {parkingEnforcementRequest.CustomersAddress.AddressLine2}
                                Town: {parkingEnforcementRequest.CustomersAddress.Town}
                                Postcode; {parkingEnforcementRequest.CustomersAddress.Postcode}
                                SelectedAddress: {parkingEnforcementRequest.CustomersAddress.SelectedAddress}";
            }
            return description;
        }
    }
}
