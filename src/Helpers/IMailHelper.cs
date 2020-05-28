using parking_enforcement_service.Models;
using StockportGovUK.NetStandard.Models.Enums;

namespace parking_enforcement_service.Helpers
{
    public interface IMailHelper
    {
        void SendEmail(Person person, EMailTemplate template, string caseReference);
    }
}
