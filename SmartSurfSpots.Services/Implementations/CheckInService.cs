using Microsoft.EntityFrameworkCore;
using SmartSurfSpots.Data;
using SmartSurfSpots.Domain.DTOs;
using SmartSurfSpots.Domain.Entities;
using SmartSurfSpots.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SmartSurfSpots.Services.Implementations
{
    public class CheckInService : ICheckInService
    {
        private readonly SurfDbContext _context;

        public CheckInService(SurfDbContext context)
        {
            _context = context;
        }

        public async Task<CheckInDto> CreateCheckInAsync(CreateCheckInRequest request, int userId)
        {
            // Verificar se o spot existe
            var spot = await _context.Spots.FindAsync(request.SpotId);
            if (spot == null)
            {
                throw new Exception("Spot not found");
            }

            // Criar check-in
            var checkIn = new CheckIn
            {
                UserId = userId,
                SpotId = request.SpotId,
                DateTime = DateTime.UtcNow,
                Notes = request.Notes
            };

            _context.CheckIns.Add(checkIn);
            await _context.SaveChangesAsync();

            // Retornar com dados relacionados
            var created = await _context.CheckIns
                .Include(c => c.User)
                .Include(c => c.Spot)
                .FirstOrDefaultAsync(c => c.Id == checkIn.Id);

            return new CheckInDto
            {
                Id = created.Id,
                UserName = created.User.Name,
                SpotName = created.Spot.Name,
                DateTime = created.DateTime,
                Notes = created.Notes
            };
        }

        public async Task<IEnumerable<CheckInDto>> GetMyCheckInsAsync(int userId)
        {
            var checkIns = await _context.CheckIns
                .Include(c => c.User)
                .Include(c => c.Spot)
                .Where(c => c.UserId == userId)
                .OrderByDescending(c => c.DateTime)
                .ToListAsync();

            return checkIns.Select(c => new CheckInDto
            {
                Id = c.Id,
                UserName = c.User.Name,
                SpotName = c.Spot.Name,
                DateTime = c.DateTime,
                Notes = c.Notes
            });
        }

        public async Task<IEnumerable<CheckInDto>> GetCheckInsBySpotAsync(int spotId)
        {
            var checkIns = await _context.CheckIns
                .Include(c => c.User)
                .Include(c => c.Spot)
                .Where(c => c.SpotId == spotId)
                .OrderByDescending(c => c.DateTime)
                .ToListAsync();

            return checkIns.Select(c => new CheckInDto
            {
                Id = c.Id,
                UserName = c.User.Name,
                SpotName = c.Spot.Name,
                DateTime = c.DateTime,
                Notes = c.Notes
            });
        }

        public async Task<CheckInDto> GetCheckInByIdAsync(int id)
        {
            var checkIn = await _context.CheckIns
                .Include(c => c.User)
                .Include(c => c.Spot)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (checkIn == null)
            {
                throw new Exception("Check-in not found");
            }

            return new CheckInDto
            {
                Id = checkIn.Id,
                UserName = checkIn.User.Name,
                SpotName = checkIn.Spot.Name,
                DateTime = checkIn.DateTime,
                Notes = checkIn.Notes
            };
        }

        public async Task<bool> DeleteCheckInAsync(int id, int userId)
        {
            var checkIn = await _context.CheckIns.FindAsync(id);

            if (checkIn == null)
            {
                throw new Exception("Check-in not found");
            }

            // Apenas o dono pode apagar
            if (checkIn.UserId != userId)
            {
                throw new Exception("You can only delete your own check-ins");
            }

            _context.CheckIns.Remove(checkIn);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}