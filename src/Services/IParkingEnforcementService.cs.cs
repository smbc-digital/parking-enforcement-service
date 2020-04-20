using parking_enforcement_service.Models;
using System.Threading.Tasks;

namespace parking_enforcement_service.Services
{
    public interface IParkingEnforcementService
    {
        Task<string> CreateCase(ParkingEnforcementRequest formData);
    }
}
