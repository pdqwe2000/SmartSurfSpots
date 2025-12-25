using Microsoft.EntityFrameworkCore;
using SmartSurfSpots.Data;
using SmartSurfSpots.Domain.Entities;
using SmartSurfSpots.SoapService.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SmartSurfSpots.SoapService.Services
{
    public class SoapDataService : ISoapDataService
    {
        private readonly SurfDbContext _context;

        public SoapDataService(SurfDbContext context)
        {
            _context = context;
        }

        // User Operations
        public async Task<SoapUserResponse> GetUserById(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null) return null;

            return new SoapUserResponse
            {
                Id = user.Id,
                Name = user.Name,
                Email = user.Email
            };
        }

        public async Task<SoapUserResponse> GetUserByEmail(string email)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
            if (user == null) return null;

            return new SoapUserResponse
            {
                Id = user.Id,
                Name = user.Name,
                Email = user.Email
            };
        }

        public async Task<List<SoapUserResponse>> GetAllUsers()
        {
            var users = await _context.Users.ToListAsync();
            return users.Select(u => new SoapUserResponse
            {
                Id = u.Id,
                Name = u.Name,
                Email = u.Email
            }).ToList();
        }

        // Spot Operations
        public async Task<SoapSpotResponse> GetSpotById(int id)
        {
            var spot = await _context.Spots
                .Include(s => s.Creator)
                .FirstOrDefaultAsync(s => s.Id == id);

            if (spot == null) return null;

            return new SoapSpotResponse
            {
                Id = spot.Id,
                Name = spot.Name,
                Latitude = spot.Latitude,
                Longitude = spot.Longitude,
                Description = spot.Description,
                Level = spot.Level.ToString(),
                CreatedBy = spot.CreatedBy,
                CreatedByName = spot.Creator?.Name
            };
        }

        public async Task<List<SoapSpotResponse>> GetAllSpots()
        {
            var spots = await _context.Spots
                .Include(s => s.Creator)
                .ToListAsync();

            return spots.Select(s => new SoapSpotResponse
            {
                Id = s.Id,
                Name = s.Name,
                Latitude = s.Latitude,
                Longitude = s.Longitude,
                Description = s.Description,
                Level = s.Level.ToString(),
                CreatedBy = s.CreatedBy,
                CreatedByName = s.Creator?.Name
            }).ToList();
        }

        public async Task<SoapSpotResponse> CreateSpot(SoapCreateSpotRequest request)
        {
            var spot = new Spot
            {
                Name = request.Name,
                Latitude = request.Latitude,
                Longitude = request.Longitude,
                Description = request.Description,
                Level = (SpotLevel)request.Level,
                CreatedBy = request.CreatedBy,
                CreatedAt = DateTime.Now
            };

            _context.Spots.Add(spot);
            await _context.SaveChangesAsync();

            return await GetSpotById(spot.Id);
        }

        public async Task<SoapSpotResponse> UpdateSpot(SoapUpdateSpotRequest request)
        {
            var spot = await _context.Spots.FindAsync(request.Id);
            if (spot == null) return null;

            spot.Name = request.Name;
            spot.Latitude = request.Latitude;
            spot.Longitude = request.Longitude;
            spot.Description = request.Description;
            spot.Level = (SpotLevel)request.Level;

            await _context.SaveChangesAsync();

            return await GetSpotById(spot.Id);
        }

        public async Task<bool> DeleteSpot(int id)
        {
            var spot = await _context.Spots.FindAsync(id);
            if (spot == null) return false;

            _context.Spots.Remove(spot);
            await _context.SaveChangesAsync();
            return true;
        }

        // CheckIn Operations
        public async Task<SoapCheckInResponse> CreateCheckIn(SoapCreateCheckInRequest request)
        {
            var checkIn = new CheckIn
            {
                UserId = request.UserId,
                SpotId = request.SpotId,
                DateTime = DateTime.Now,
                Notes = request.Notes
            };

            _context.CheckIns.Add(checkIn);
            await _context.SaveChangesAsync();

            var created = await _context.CheckIns
                .Include(c => c.User)
                .Include(c => c.Spot)
                .FirstOrDefaultAsync(c => c.Id == checkIn.Id);

            return new SoapCheckInResponse
            {
                Id = created.Id,
                UserId = created.UserId,
                UserName = created.User.Name,
                SpotId = created.SpotId,
                SpotName = created.Spot.Name,
                DateTime = created.DateTime.ToString("yyyy-MM-dd HH:mm:ss"),
                Notes = created.Notes
            };
        }

        public async Task<List<SoapCheckInResponse>> GetCheckInsBySpot(int spotId)
        {
            var checkIns = await _context.CheckIns
                .Include(c => c.User)
                .Include(c => c.Spot)
                .Where(c => c.SpotId == spotId)
                .OrderByDescending(c => c.DateTime)
                .ToListAsync();

            return checkIns.Select(c => new SoapCheckInResponse
            {
                Id = c.Id,
                UserId = c.UserId,
                UserName = c.User.Name,
                SpotId = c.SpotId,
                SpotName = c.Spot.Name,
                DateTime = c.DateTime.ToString("yyyy-MM-dd HH:mm:ss"),
                Notes = c.Notes
            }).ToList();
        }

        public async Task<List<SoapCheckInResponse>> GetCheckInsByUser(int userId)
        {
            var checkIns = await _context.CheckIns
                .Include(c => c.User)
                .Include(c => c.Spot)
                .Where(c => c.UserId == userId)
                .OrderByDescending(c => c.DateTime)
                .ToListAsync();

            return checkIns.Select(c => new SoapCheckInResponse
            {
                Id = c.Id,
                UserId = c.UserId,
                UserName = c.User.Name,
                SpotId = c.SpotId,
                SpotName = c.Spot.Name,
                DateTime = c.DateTime.ToString("yyyy-MM-dd HH:mm:ss"),
                Notes = c.Notes
            }).ToList();
        }
    }
}