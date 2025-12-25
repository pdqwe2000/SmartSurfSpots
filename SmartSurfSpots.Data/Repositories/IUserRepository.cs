using SmartSurfSpots.Domain.Entities;
using System.Threading.Tasks;

namespace SmartSurfSpots.Data.Repositories
{
    public interface IUserRepository : IRepository<User>
    {
        Task<User> GetByEmailAsync(string email);
    }
}