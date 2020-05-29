using StockportGovUK.NetStandard.Models.Addresses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace parking_enforcement_service.Models
{
    public class Person
    {      
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public Address CustomerAddress { get; set; }
    }
}
