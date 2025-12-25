using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartSurfSpots.Domain.Entities
{
    public class CheckIn
    {
        [Key] // Chave Primária (PK)
        public int Id { get; set; }

        // Chave Estrangeira (FK) para User
        [ForeignKey("User")]
        public int UserId { get; set; }

        // Chave Estrangeira (FK) para Spot
        [ForeignKey("Spot")]
        public int SpotId { get; set; }

        [Required] 
        public DateTime DateTime { get; set; }

        [MaxLength(500)] 
        public string Notes { get; set; }

        // Navigation properties (Relações)
        public virtual User User { get; set; }
        public virtual Spot Spot { get; set; }
    }
}