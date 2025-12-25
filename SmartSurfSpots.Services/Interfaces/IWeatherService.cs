using SmartSurfSpots.Domain.DTOs;
using System.Threading.Tasks;

namespace SmartSurfSpots.Services.Interfaces
{
    /// <summary>
    /// Contrato para o serviço de meteorologia.
    /// Responsável por comunicar com APIs externas (Open-Meteo).
    /// </summary>
    public interface IWeatherService
    {
        /// <summary>
        /// Obtém a previsão meteorológica e dados marinhos para um spot específico.
        /// Usa as coordenadas (Lat/Long) do spot para consultar a API externa.
        /// </summary>
        /// <param name="spotId">ID do spot na base de dados.</param>
        /// <returns>Objeto complexo com dados atuais e previsão horária.</returns>
        Task<WeatherDto> GetWeatherForSpotAsync(int spotId);
    }
}