using System;
using System.Collections.Generic;

namespace SmartSurfSpots.Domain.Entities
{
    public class User
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string PasswordHash { get; set; }
        public DateTime CreatedAt { get; set; }

        // Navigation properties
        public ICollection<Spot> Spots { get; set; }
        public ICollection<CheckIn> CheckIns { get; set; }
    }
}