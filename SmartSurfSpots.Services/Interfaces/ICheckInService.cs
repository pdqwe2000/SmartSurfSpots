using SmartSurfSpots.Domain.DTOs;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SmartSurfSpots.Services.Interfaces
{
    public interface ICheckInService
    {
        Task<CheckInDto> CreateCheckInAsync(CreateCheckInRequest request, int userId);
        Task<IEnumerable<CheckInDto>> GetMyCheckInsAsync(int userId);
        Task<IEnumerable<CheckInDto>> GetCheckInsBySpotAsync(int spotId);
        Task<CheckInDto> GetCheckInByIdAsync(int id);
        Task<bool> DeleteCheckInAsync(int id, int userId);
    }
}