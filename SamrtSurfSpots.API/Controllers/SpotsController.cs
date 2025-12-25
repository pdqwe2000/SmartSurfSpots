using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmartSurfSpots.Domain.DTOs;
using SmartSurfSpots.Services.Interfaces;
using System.Security.Claims;

namespace SmartSurfSpots.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize] // Protege todos os endpoints deste controller
    [Produces("application/json")]
    public class SpotsController : ControllerBase
    {
        private readonly ISpotService _spotService;

        public SpotsController(ISpotService spotService)
        {
            _spotService = spotService;
        }

        /// <summary>
        /// Lista todos os spots de surf disponíveis.
        /// </summary>
        /// <remarks>
        /// Retorna uma lista completa de spots registados na plataforma.
        /// Requer autenticação.
        /// </remarks>
        /// <returns>Lista de spots.</returns>
        /// <response code="200">Lista obtida com sucesso.</response>
        /// <response code="401">Utilizador não autenticado.</response>
        /// <response code="400">Erro ao processar o pedido.</response>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetAllSpots()
        {
            try
            {
                var spots = await _spotService.GetAllSpotsAsync();
                return Ok(spots);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        /// <summary>
        /// Obtém os detalhes de um spot específico pelo ID.
        /// </summary>
        /// <param name="id">O identificador único do spot.</param>
        /// <returns>Dados detalhados do spot.</returns>
        /// <response code="200">Spot encontrado com sucesso.</response>
        /// <response code="404">Spot não encontrado.</response>
        /// <response code="401">Utilizador não autenticado.</response>
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> GetSpotById(int id)
        {
            try
            {
                var spot = await _spotService.GetSpotByIdAsync(id);
                return Ok(spot);
            }
            catch (Exception ex)
            {
                // Assumindo que o serviço lança exceção quando não encontra, 
                // ou podes verificar se spot é null.
                return NotFound(new { message = ex.Message });
            }
        }

        /// <summary>
        /// Cria um novo spot de surf.
        /// </summary>
        /// <remarks>
        /// O utilizador autenticado será associado como criador do spot.
        /// </remarks>
        /// <param name="request">Dados do novo spot (Nome, Localização, Dificuldade, etc.).</param>
        /// <returns>O spot criado.</returns>
        /// <response code="201">Spot criado com sucesso.</response>
        /// <response code="400">Dados inválidos ou erro na criação.</response>
        /// <response code="401">Utilizador não autenticado.</response>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> CreateSpot([FromBody] CreateSpotRequest request)
        {
            try
            {
                var claimId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(claimId)) return Unauthorized();

                var userId = int.Parse(claimId);
                var spot = await _spotService.CreateSpotAsync(request, userId);

                return CreatedAtAction(nameof(GetSpotById), new { id = spot.Id }, spot);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        /// <summary>
        /// Atualiza um spot existente.
        /// </summary>
        /// <remarks>
        /// Apenas o criador do spot (ou admin) deve conseguir realizar esta ação (validado no serviço).
        /// </remarks>
        /// <param name="id">ID do spot a atualizar.</param>
        /// <param name="request">Novos dados do spot.</param>
        /// <returns>O spot atualizado.</returns>
        /// <response code="200">Spot atualizado com sucesso.</response>
        /// <response code="400">Erro de validação ou permissão negada.</response>
        /// <response code="401">Utilizador não autenticado.</response>
        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> UpdateSpot(int id, [FromBody] UpdateSpotRequest request)
        {
            try
            {
                var claimId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(claimId)) return Unauthorized();

                var userId = int.Parse(claimId);
                var spot = await _spotService.UpdateSpotAsync(id, request, userId);
                return Ok(spot);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        /// <summary>
        /// Apaga um spot do sistema.
        /// </summary>
        /// <remarks>
        /// Apenas o criador ou admin podem apagar o spot.
        /// </remarks>
        /// <param name="id">ID do spot a remover.</param>
        /// <returns>Mensagem de sucesso.</returns>
        /// <response code="200">Spot apagado com sucesso.</response>
        /// <response code="400">Erro ao apagar (ex: spot não existe ou sem permissão).</response>
        /// <response code="401">Utilizador não autenticado.</response>
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> DeleteSpot(int id)
        {
            try
            {
                var claimId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(claimId)) return Unauthorized();

                var userId = int.Parse(claimId);
                await _spotService.DeleteSpotAsync(id, userId);
                return Ok(new { message = "Spot deleted successfully" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}