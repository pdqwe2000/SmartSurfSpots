using SmartSurfSpots.Data.Repositories;
using SmartSurfSpots.Domain.DTOs;
using SmartSurfSpots.Domain.Entities;
using SmartSurfSpots.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SmartSurfSpots.Services.Implementations
{
    public class SpotService : ISpotService
    {
        private readonly ISpotRepository _spotRepository;

        public SpotService(ISpotRepository spotRepository)
        {
            _spotRepository = spotRepository;
        }

        public async Task<IEnumerable<SpotDto>> GetAllSpotsAsync()
        {
            var spots = await _spotRepository.GetSpotsWithCreatorAsync();

            // Mapeamento manual Entity -> DTO
            return spots.Select(s => new SpotDto
            {
                Id = s.Id,
                Name = s.Name,
                Latitude = s.Latitude,
                Longitude = s.Longitude,
                Description = s.Description,
                Level = s.Level.ToString(),
                CreatedBy = s.Creator?.Name // Null check seguro
            });
        }

        public async Task<SpotDto> GetSpotByIdAsync(int id)
        {
            var spot = await _spotRepository.GetSpotWithDetailsAsync(id);

            if (spot == null) throw new Exception("Spot not found");

            return new SpotDto
            {
                Id = spot.Id,
                Name = spot.Name,
                Latitude = spot.Latitude,
                Longitude = spot.Longitude,
                Description = spot.Description,
                Level = spot.Level.ToString(),
                CreatedBy = spot.Creator?.Name
            };
        }

        public async Task<SpotDto> CreateSpotAsync(CreateSpotRequest request, int userId)
        {
            var spot = new Spot
            {
                Name = request.Name,
                Latitude = request.Latitude,
                Longitude = request.Longitude,
                Description = request.Description,
                Level = (SpotLevel)request.Level, // Cast int -> Enum
                CreatedBy = userId,
                CreatedAt = DateTime.Now
            };

            await _spotRepository.AddAsync(spot);

            // Recarregar para obter os dados completos (incluindo relacionamentos se necessário)
            var createdSpot = await _spotRepository.GetSpotWithDetailsAsync(spot.Id);

            return new SpotDto
            {
                Id = createdSpot.Id,
                Name = createdSpot.Name,
                Latitude = createdSpot.Latitude,
                Longitude = createdSpot.Longitude,
                Description = createdSpot.Description,
                Level = createdSpot.Level.ToString(),
                CreatedBy = createdSpot.Creator?.Name
            };
        }

        public async Task<SpotDto> UpdateSpotAsync(int id, UpdateSpotRequest request, int userId)
        {
            var spot = await _spotRepository.GetByIdAsync(id);
            if (spot == null) throw new Exception("Spot not found");

            // Validação de Segurança: Apenas o dono pode editar
            if (spot.CreatedBy != userId)
            {
                throw new Exception("You can only update your own spots");
            }

            spot.Name = request.Name;
            spot.Latitude = request.Latitude;
            spot.Longitude = request.Longitude;
            spot.Description = request.Description;
            spot.Level = (SpotLevel)request.Level;

            await _spotRepository.UpdateAsync(spot);

            return new SpotDto
            {
                Id = spot.Id,
                Name = spot.Name,
                Latitude = spot.Latitude,
                Longitude = spot.Longitude,
                Description = spot.Description,
                Level = spot.Level.ToString(),
                CreatedBy = spot.Creator?.Name
            };
        }

        public async Task<bool> DeleteSpotAsync(int id, int userId)
        {
            var spot = await _spotRepository.GetByIdAsync(id);
            if (spot == null) throw new Exception("Spot not found");

            // Validação de Segurança: Apenas o dono pode apagar
            if (spot.CreatedBy != userId)
            {
                throw new Exception("You can only delete your own spots");
            }

            await _spotRepository.DeleteAsync(spot);
            return true;
        }
    }
}