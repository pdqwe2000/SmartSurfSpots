using SmartSurfSpots.Domain.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SmartSurfSpots.Data.Repositories
{
    public interface ISpotRepository : IRepository<Spot>
    {
        Task<IEnumerable<Spot>> GetSpotsWithCreatorAsync();
        Task<Spot> GetSpotWithDetailsAsync(int id);
    }
}