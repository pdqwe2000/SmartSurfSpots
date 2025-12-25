using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmartSurfSpots.Domain.DTOs;
using SmartSurfSpots.Services.Interfaces;
using System.Security.Claims;

namespace SmartSurfSpots.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class CheckInsController : ControllerBase
    {
        private readonly ICheckInService _checkInService;

        public CheckInsController(ICheckInService checkInService)
        {
            _checkInService = checkInService;
        }

        /// <summary>
        /// Fazer check-in num spot
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> CreateCheckIn([FromBody] CreateCheckInRequest request)
        {
            try
            {
                var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
                var checkIn = await _checkInService.CreateCheckInAsync(request, userId);
                return CreatedAtAction(nameof(GetCheckInById), new { id = checkIn.Id }, checkIn);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        /// <summary>
        /// Obter os meus check-ins
        /// </summary>
        [HttpGet("my")]
        public async Task<IActionResult> GetMyCheckIns()
        {
            try
            {
                var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
                var checkIns = await _checkInService.GetMyCheckInsAsync(userId);
                return Ok(checkIns);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        /// <summary>
        /// Obter check-ins de um spot específico
        /// </summary>
        [HttpGet("spot/{spotId}")]
        public async Task<IActionResult> GetCheckInsBySpot(int spotId)
        {
            try
            {
                var checkIns = await _checkInService.GetCheckInsBySpotAsync(spotId);
                return Ok(checkIns);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        /// <summary>
        /// Obter detalhes de um check-in
        /// </summary>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetCheckInById(int id)
        {
            try
            {
                var checkIn = await _checkInService.GetCheckInByIdAsync(id);
                return Ok(checkIn);
            }
            catch (Exception ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }

        /// <summary>
        /// Apagar check-in (apenas o próprio)
        /// </summary>
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCheckIn(int id)
        {
            try
            {
                var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
                await _checkInService.DeleteCheckInAsync(id, userId);
                return Ok(new { message = "Check-in deleted successfully" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}