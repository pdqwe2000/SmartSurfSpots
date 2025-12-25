using SmartSurfSpots.Domain.DTOs;
using System.Threading.Tasks;

namespace SmartSurfSpots.Services.Interfaces
{
    /// <summary>
    /// Contrato para o serviço de autenticação.
    /// Define as operações de registo, login e recuperação de perfil.
    /// </summary>
    public interface IAuthService
    {
        /// <summary>
        /// Regista um novo utilizador no sistema e retorna o token de acesso imediato.
        /// </summary>
        /// <param name="request">Dados de registo (nome, email, password).</param>
        /// <returns>Objeto contendo o Token JWT e dados do utilizador.</returns>
        Task<LoginResponse> RegisterAsync(RegisterRequest request);

        /// <summary>
        /// Valida as credenciais do utilizador e gera um token JWT.
        /// </summary>
        /// <param name="request">Email e Password.</param>
        /// <returns>Objeto contendo o Token JWT e dados do utilizador.</returns>
        Task<LoginResponse> LoginAsync(LoginRequest request);

        /// <summary>
        /// Obtém os dados de um utilizador (sem dados sensíveis) pelo seu ID.
        /// </summary>
        /// <param name="userId">ID do utilizador.</param>
        /// <returns>DTO do utilizador.</returns>
        Task<UserDto> GetUserByIdAsync(int userId);
    }
}