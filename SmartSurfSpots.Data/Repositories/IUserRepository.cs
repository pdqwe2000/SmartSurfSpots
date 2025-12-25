using SmartSurfSpots.Domain.Entities;
using System.Threading.Tasks;

namespace SmartSurfSpots.Data.Repositories
{
    /// <summary>
    /// Repositório especializado para operações relacionadas com Utilizadores.
    /// </summary>
    public interface IUserRepository : IRepository<User>
    {
        /// <summary>
        /// Procura um utilizador através do endereço de email.
        /// Útil para validações de login e registo.
        /// </summary>
        /// <param name="email">O email a pesquisar.</param>
        /// <returns>O utilizador encontrado ou null.</returns>
        Task<User> GetByEmailAsync(string email);
    }
}