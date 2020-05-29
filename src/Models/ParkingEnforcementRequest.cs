using StockportGovUK.NetStandard.Models.Addresses;

namespace parking_enforcement_service.Models
{
    public class ParkingEnforcementRequest
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string MoreDetails { get; set; }
        public string FurtherInformation { get; set; }
        public Address StreetAddress { get; set; }
        public Address CustomersAddress { get; set; }
    }
}
