using SmartSurfSpots.Domain.DTOs;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SmartSurfSpots.Services.Interfaces
{
    public interface ISpotService
    {
        Task<IEnumerable<SpotDto>> GetAllSpotsAsync();
        Task<SpotDto> GetSpotByIdAsync(int id);
        Task<SpotDto> CreateSpotAsync(CreateSpotRequest request, int userId);
        Task<SpotDto> UpdateSpotAsync(int id, UpdateSpotRequest request, int userId);
        Task<bool> DeleteSpotAsync(int id, int userId);
    }
}   