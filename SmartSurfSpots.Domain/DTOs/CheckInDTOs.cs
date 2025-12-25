using System.ComponentModel.DataAnnotations;

namespace SmartSurfSpots.Domain.DTOs
{
    /// <summary>
    /// Representa um Check-in já criado (Output).
    /// </summary>
    public class CheckInDto
    {
        public int Id { get; set; }

        /// <summary>
        /// Nome do utilizador que fez o check-in.
        /// </summary>
        public string UserName { get; set; }

        /// <summary>
        /// Nome do spot onde foi feito o check-in.
        /// </summary>
        public string SpotName { get; set; }

        /// <summary>
        /// Data e hora do check-in.
        /// </summary>
        public DateTime DateTime { get; set; }

        /// <summary>
        /// Notas ou observações deixadas pelo utilizador.
        /// </summary>
        public string Notes { get; set; }
    }

    /// <summary>
    /// Dados para criar um novo Check-in.
    /// </summary>
    public class CreateCheckInRequest
    {
        /// <summary>
        /// ID do spot onde o utilizador se encontra.
        /// </summary>
        /// <example>1</example>
        [Required(ErrorMessage = "O ID do spot é obrigatório.")]
        [Range(1, int.MaxValue, ErrorMessage = "ID do spot inválido.")]
        public int SpotId { get; set; }

        /// <summary>
        /// Comentário opcional sobre a sessão (ex: condições do mar).
        /// </summary>
        /// <example>O mar está incrível hoje!</example>
        [StringLength(500, ErrorMessage = "As notas não podem exceder 500 caracteres.")]
        public string Notes { get; set; }
    }
}