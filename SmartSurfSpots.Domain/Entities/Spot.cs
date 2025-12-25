using System;
using System.Collections.Generic;

namespace SmartSurfSpots.Domain.Entities
{
    public class Spot
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public string Description { get; set; }
        public SpotLevel Level { get; set; }
        public int CreatedBy { get; set; }
        public DateTime CreatedAt { get; set; }

        // Navigation properties
        public User Creator { get; set; }
        public ICollection<CheckIn> CheckIns { get; set; }
    }

    public enum SpotLevel
    {
        Beginner = 1,
        Intermediate = 2,
        Advanced = 3
    }
}