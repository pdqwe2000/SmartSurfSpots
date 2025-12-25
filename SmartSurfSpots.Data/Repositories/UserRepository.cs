using Microsoft.EntityFrameworkCore;
using SmartSurfSpots.Domain.Entities;
using System.Threading.Tasks;

namespace SmartSurfSpots.Data.Repositories
{
    public class UserRepository : Repository<User>, IUserRepository
    {
        public UserRepository(SurfDbContext context) : base(context)
        {
        }

        public async Task<User> GetByEmailAsync(string email)
        {
            return await _dbSet.FirstOrDefaultAsync(u => u.Email == email);
        }
    }
}