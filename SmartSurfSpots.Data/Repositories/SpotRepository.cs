using Microsoft.EntityFrameworkCore;
using SmartSurfSpots.Domain.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SmartSurfSpots.Data.Repositories
{
    public class SpotRepository : Repository<Spot>, ISpotRepository
    {
        public SpotRepository(SurfDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<Spot>> GetSpotsWithCreatorAsync()
        {
            return await _dbSet
                .Include(s => s.Creator)
                .ToListAsync();
        }

        public async Task<Spot> GetSpotWithDetailsAsync(int id)
        {
            return await _dbSet
                .Include(s => s.Creator)
                .Include(s => s.CheckIns)
                .FirstOrDefaultAsync(s => s.Id == id);
        }
    }
}