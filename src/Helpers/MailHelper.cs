using parking_enforcement_service.Models;
using Newtonsoft.Json;
using StockportGovUK.NetStandard.Gateways.MailingServiceGateway;
using StockportGovUK.NetStandard.Models.ComplimentsComplaints;
using StockportGovUK.NetStandard.Models.Mail;
using StockportGovUK.NetStandard.Models.Enums;

namespace parking_enforcement_service.Helpers
{
    public class MailHelper : IMailHelper
    {
        private readonly IMailingServiceGateway _mailingServiceGateway;

        public MailHelper(IMailingServiceGateway mailingServiceGateway)
        {
            _mailingServiceGateway = mailingServiceGateway;
        }

        public void SendEmail(Person person, EMailTemplate template, string caseReference)
        {
            var submissionDetails = new ComplaintsMailModel
            {
                Subject = "Parking enforcement request form - submission",
                Reference = caseReference,
                FirstName = person.FirstName,
                LastName = person.LastName,
                RecipientAddress = person.Email
            };

            _mailingServiceGateway.Send(new Mail
            {
                Payload = JsonConvert.SerializeObject(submissionDetails),
                Template = template
            });
        }
    }
}