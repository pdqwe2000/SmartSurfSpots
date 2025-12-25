using Microsoft.EntityFrameworkCore;
using SmartSurfSpots.Domain.Entities;
using System.Threading.Tasks;

namespace SmartSurfSpots.Data.Repositories
{
    /// <summary>
    /// Implementação concreta do repositório de Users.
    /// </summary>
    public class UserRepository : Repository<User>, IUserRepository
    {
        public UserRepository(SurfDbContext context) : base(context)
        {
        }

        /// <summary>
        /// Procura um utilizador especificamente pelo email (Case Insensitive dependendo da Collation da BD).
        /// </summary>
        public async Task<User> GetByEmailAsync(string email)
        {
            // FirstOrDefaultAsync retorna null se não encontrar, o que é ideal para verificar login
            return await _dbSet.FirstOrDefaultAsync(u => u.Email == email);
        }
    }
}