using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartSurfSpots.Domain.Entities
{
    public class Spot
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string Name { get; set; }

        public double Latitude { get; set; }
        public double Longitude { get; set; }

        [MaxLength(1000)] 
        public string Description { get; set; }

        [Required]
        public SpotLevel Level { get; set; } 

        [ForeignKey("Creator")]
        public int CreatedBy { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public virtual User Creator { get; set; }
        public virtual ICollection<CheckIn> CheckIns { get; set; }
    }

    public enum SpotLevel
    {
        Beginner = 1,
        Intermediate = 2,
        Advanced = 3
    }
}