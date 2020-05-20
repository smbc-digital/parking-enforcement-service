using System.Threading.Tasks;
using parking_enforcement_service.Models;

namespace parking_enforcement_service.Services
{
    public interface IParkingEnforcementService
    {
        Task<string> CreateCase(ParkingEnforcementRequest formData);
    }
}
