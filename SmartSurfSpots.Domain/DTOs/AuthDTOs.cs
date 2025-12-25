using System.ComponentModel.DataAnnotations;

namespace SmartSurfSpots.Domain.DTOs
{
    /// <summary>
    /// Dados necessários para registar um novo utilizador.
    /// </summary>
    public class RegisterRequest
    {
        /// <summary>
        /// Nome completo do utilizador.
        /// </summary>
        /// <example>João Silva</example>
        [Required(ErrorMessage = "O nome é obrigatório.")]
        [StringLength(100, MinimumLength = 3, ErrorMessage = "O nome deve ter entre 3 e 100 caracteres.")]
        public string Name { get; set; }

        /// <summary>
        /// Endereço de email válido.
        /// </summary>
        /// <example>joao.silva@exemplo.com</example>
        [Required(ErrorMessage = "O email é obrigatório.")]
        [EmailAddress(ErrorMessage = "O formato do email é inválido.")]
        public string Email { get; set; }

        /// <summary>
        /// Palavra-passe segura (min. 6 caracteres).
        /// </summary>
        /// <example>SenhaSegura123!</example>
        [Required(ErrorMessage = "A password é obrigatória.")]
        [MinLength(6, ErrorMessage = "A password deve ter pelo menos 6 caracteres.")]
        public string Password { get; set; }
    }

    /// <summary>
    /// Credenciais para autenticação no sistema.
    /// </summary>
    public class LoginRequest
    {
        /// <summary>
        /// O email registado.
        /// </summary>
        /// <example>joao.silva@exemplo.com</example>
        [Required(ErrorMessage = "O email é obrigatório.")]
        [EmailAddress(ErrorMessage = "O formato do email é inválido.")]
        public string Email { get; set; }

        /// <summary>
        /// A palavra-passe do utilizador.
        /// </summary>
        /// <example>SenhaSegura123!</example>
        [Required(ErrorMessage = "A password é obrigatória.")]
        public string Password { get; set; }
    }

    /// <summary>
    /// Resposta devolvida após um login com sucesso.
    /// </summary>
    public class LoginResponse
    {
        /// <summary>
        /// Token JWT para ser usado no cabeçalho Authorization (Bearer).
        /// </summary>
        public string Token { get; set; }

        /// <summary>
        /// Detalhes do utilizador autenticado.
        /// </summary>
        public UserDto User { get; set; }
    }

    /// <summary>
    /// Objeto de transferência de dados do utilizador (sem dados sensíveis).
    /// </summary>
    public class UserDto
    {
        /// <summary>
        /// Identificador único do utilizador.
        /// </summary>
        /// <example>1</example>
        public int Id { get; set; }

        /// <summary>
        /// Nome do utilizador.
        /// </summary>
        /// <example>João Silva</example>
        public string Name { get; set; }

        /// <summary>
        /// Email do utilizador.
        /// </summary>
        /// <example>joao.silva@exemplo.com</example>
        public string Email { get; set; }
    }
}