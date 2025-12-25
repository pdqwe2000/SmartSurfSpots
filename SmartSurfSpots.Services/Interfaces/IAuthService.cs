using SmartSurfSpots.Domain.DTOs;
using System.Threading.Tasks;

namespace SmartSurfSpots.Services.Interfaces
{
    public interface IAuthService
    {
        Task<LoginResponse> RegisterAsync(RegisterRequest request);
        Task<LoginResponse> LoginAsync(LoginRequest request);
        Task<UserDto> GetUserByIdAsync(int userId);
    }
}