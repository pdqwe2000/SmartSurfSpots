using Microsoft.EntityFrameworkCore;
using SmartSurfSpots.Domain.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SmartSurfSpots.Data.Repositories
{
    /// <summary>
    /// Implementação concreta do repositório de Spots.
    /// Contém lógica específica para carregar relacionamentos (Eager Loading).
    /// </summary>
    public class SpotRepository : Repository<Spot>, ISpotRepository
    {
        public SpotRepository(SurfDbContext context) : base(context)
        {
        }

        /// <summary>
        /// Obtém a lista de spots e faz o JOIN com a tabela Users para preencher o Criador.
        /// </summary>
        public async Task<IEnumerable<Spot>> GetSpotsWithCreatorAsync()
        {
            return await _dbSet
                .Include(s => s.Creator) // Eager Loading: Traz os dados do User criador
                .ToListAsync();
        }

        /// <summary>
        /// Obtém um spot específico e carrega toda a árvore de dependências (Criador e CheckIns).
        /// </summary>
        public async Task<Spot> GetSpotWithDetailsAsync(int id)
        {
            return await _dbSet
                .Include(s => s.Creator)   // Inclui quem criou
                .Include(s => s.CheckIns)  // Inclui a lista de check-ins feitos no spot
                .FirstOrDefaultAsync(s => s.Id == id);
        }
    }
}