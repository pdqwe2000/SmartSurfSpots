using SmartSurfSpots.Domain.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SmartSurfSpots.Data.Repositories
{
    /// <summary>
    /// Repositório especializado para operações relacionadas com Spots de Surf.
    /// </summary>
    public interface ISpotRepository : IRepository<Spot>
    {
        /// <summary>
        /// Obtém todos os spots incluindo os dados do utilizador que os criou (Eager Loading).
        /// </summary>
        /// <returns>Lista de spots com o objeto 'Creator' preenchido.</returns>
        Task<IEnumerable<Spot>> GetSpotsWithCreatorAsync();

        /// <summary>
        /// Obtém um spot específico com todos os detalhes relacionados (Criador, CheckIns, etc.).
        /// </summary>
        /// <param name="id">ID do spot.</param>
        /// <returns>O spot completo ou null.</returns>
        Task<Spot> GetSpotWithDetailsAsync(int id);
    }
}