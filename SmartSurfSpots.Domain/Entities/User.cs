using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartSurfSpots.Domain.Entities
{
    public class User
    {
        [Key]
        public int Id { get; set; }

        [Required] 
        [MaxLength(100)] 
        public string Name { get; set; }

        [Required]
        [MaxLength(150)]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        public string PasswordHash { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow; 

        public virtual ICollection<Spot> Spots { get; set; }
        public virtual ICollection<CheckIn> CheckIns { get; set; }
    }
}