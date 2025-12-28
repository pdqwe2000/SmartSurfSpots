using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmartSurfSpots.Domain.DTOs;
using SmartSurfSpots.Services.Interfaces;
using System.Security.Claims;

namespace SmartSurfSpots.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Produces("application/json")] // Define que o controller retorna JSON
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        /// <summary>
        /// Regista um novo utilizador no sistema.
        /// </summary>
        /// <remarks>
        /// Cria uma nova conta de utilizador com os dados fornecidos e retorna a confirmação.
        /// </remarks>
        /// <param name="request">O objeto contendo os dados de registo (Nome, Email, Password).</param>
        /// <returns>Resultado da operação de registo.</returns>
        /// <response code="200">Utilizador registado com sucesso.</response>
        /// <response code="400">Falha na validação dos dados ou o utilizador já existe.</response>
        [HttpPost("register")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Register([FromBody] RegisterRequest request)
        {
            try
            {
                var response = await _authService.RegisterAsync(request);
                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        /// <summary>
        /// Autentica um utilizador e gera um token de acesso (JWT).
        /// </summary>
        /// <remarks>
        /// Verifica as credenciais (email e password). Se corretas, retorna um token para ser usado nos endpoints protegidos.
        /// </remarks>
        /// <param name="request">Credenciais de acesso do utilizador.</param>
        /// <returns>Token JWT e dados básicos do utilizador.</returns>
        /// <response code="200">Login efetuado com sucesso.</response>
        /// <response code="400">Credenciais inválidas (email ou password incorretos).</response>
        [HttpPost("login")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            try
            {
                var response = await _authService.LoginAsync(request);
                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        /// <summary>
        /// Obtém o perfil do utilizador atualmente autenticado.
        /// </summary>
        /// <remarks>
        /// Requer um token JWT válido no cabeçalho Authorization.
        /// Extrai o ID do utilizador das Claims do token.
        /// </remarks>
        /// <returns>Os detalhes do perfil do utilizador.</returns>
        /// <response code="200">Perfil obtido com sucesso.</response>
        /// <response code="401">Utilizador não autenticado ou token inválido.</response>
        /// <response code="400">Erro ao processar o pedido ou utilizador não encontrado.</response>
        [Authorize]
        [HttpGet("profile")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetProfile()
        {
            try
            {
                // É boa prática verificar se o Claim existe antes de fazer o parse
                var claimId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

                if (string.IsNullOrEmpty(claimId))
                    return Unauthorized(new { message = "Token inválido ou ID não encontrado." });

                var userId = int.Parse(claimId);
                var user = await _authService.GetUserByIdAsync(userId);

                return Ok(user);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}