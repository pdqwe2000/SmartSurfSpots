using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmartSurfSpots.Services.Interfaces;

namespace SmartSurfSpots.API.Controllers
{
    // A rota inclui {spotId}, o que significa que este recurso está dependente de um Spot
    [Route("api/spots/{spotId}/[controller]")]
    [ApiController]
    [Authorize]
    [Produces("application/json")]
    public class WeatherController : ControllerBase
    {
        private readonly IWeatherService _weatherService;

        public WeatherController(IWeatherService weatherService)
        {
            _weatherService = weatherService;
        }

        /// <summary>
        /// Obtém as condições meteorológicas e de surf para um spot específico.
        /// </summary>
        /// <remarks>
        /// Retorna dados como altura das ondas, direção do vento, temperatura e período das ondas.
        /// Útil para verificar se o spot está "surfável" no momento.
        /// </remarks>
        /// <param name="spotId">O ID do spot para o qual se pretende a previsão.</param>
        /// <returns>Dados meteorológicos atuais e previsão.</returns>
        /// <response code="200">Dados meteorológicos obtidos com sucesso.</response>
        /// <response code="400">Erro ao obter dados (ex: ID inválido ou falha no serviço externo).</response>
        /// <response code="401">Utilizador não autenticado.</response>
        /// <response code="404">Spot não encontrado (se o serviço suportar esta verificação).</response>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetWeather([FromRoute] int spotId)
        {
            try
            {
                var weather = await _weatherService.GetWeatherForSpotAsync(spotId);

                // Opcional: Se o serviço retornar null quando o spot não existe
                if (weather == null)
                    return NotFound(new { message = "Spot não encontrado ou sem dados de tempo." });

                return Ok(weather);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}