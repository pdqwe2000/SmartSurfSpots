using SmartSurfSpots.Domain.DTOs;
using System.Threading.Tasks;

namespace SmartSurfSpots.Services.Interfaces
{
    public interface IWeatherService
    {
        Task<WeatherDto> GetWeatherForSpotAsync(int spotId);
    }
}