using System.ComponentModel.DataAnnotations;

namespace SmartSurfSpots.Domain.DTOs
{
    /// <summary>
    /// Detalhes completos de um Spot de Surf.
    /// </summary>
    public class SpotDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public string Description { get; set; }
        public string Level { get; set; }
        public string CreatedBy { get; set; }
    }

    /// <summary>
    /// Dados para registar um novo Spot.
    /// </summary>
    public class CreateSpotRequest
    {
        /// <summary>
        /// Nome do spot.
        /// </summary>
        /// <example>Praia do Guincho</example>
        [Required(ErrorMessage = "O nome é obrigatório.")]
        [StringLength(100, MinimumLength = 3)]
        public string Name { get; set; }

        /// <summary>
        /// Latitude geográfica (-90 a 90).
        /// </summary>
        /// <example>38.73</example>
        [Required]
        [Range(-90, 90, ErrorMessage = "A latitude deve estar entre -90 e 90.")]
        public double Latitude { get; set; }

        /// <summary>
        /// Longitude geográfica (-180 a 180).
        /// </summary>
        /// <example>-9.47</example>
        [Required]
        [Range(-180, 180, ErrorMessage = "A longitude deve estar entre -180 e 180.")]
        public double Longitude { get; set; }

        /// <summary>
        /// Descrição do local e acesso.
        /// </summary>
        /// <example>Praia com fundo de areia, ventosa.</example>
        [StringLength(1000)]
        public string Description { get; set; }

        /// <summary>
        /// Nível de dificuldade: 1 (Iniciante), 2 (Intermédio), 3 (Avançado).
        /// </summary>
        /// <example>2</example>
        [Required]
        [Range(1, 3, ErrorMessage = "O nível deve ser: 1 (Iniciante), 2 (Intermédio) ou 3 (Avançado).")]
        public int Level { get; set; }
    }

    /// <summary>
    /// Dados para atualizar um Spot existente.
    /// </summary>
    public class UpdateSpotRequest
    {
        /// <summary>
        /// Novo nome do spot.
        /// </summary>
        /// <example>Praia do Guincho (Norte)</example>
        [Required]
        [StringLength(100, MinimumLength = 3)]
        public string Name { get; set; }

        /// <summary>
        /// Latitude geográfica.
        /// </summary>
        /// <example>38.73</example>
        [Range(-90, 90)]
        public double Latitude { get; set; }

        /// <summary>
        /// Longitude geográfica.
        /// </summary>
        /// <example>-9.47</example>
        [Range(-180, 180)]
        public double Longitude { get; set; }

        /// <summary>
        /// Nova descrição.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Novo nível de dificuldade (1-3).
        /// </summary>
        /// <example>3</example>
        [Range(1, 3)]
        public int Level { get; set; }
    }
}