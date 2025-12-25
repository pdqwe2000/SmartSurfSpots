using Microsoft.EntityFrameworkCore;
using SmartSurfSpots.Domain.Entities;
using System;

namespace SmartSurfSpots.Data
{
    public class SurfDbContext : DbContext
    {
        public SurfDbContext(DbContextOptions<SurfDbContext> options) : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Spot> Spots { get; set; }
        public DbSet<CheckIn> CheckIns { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // User configuration
            modelBuilder.Entity<User>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
                entity.Property(e => e.Email).IsRequired().HasMaxLength(100);
                entity.HasIndex(e => e.Email).IsUnique();
                entity.Property(e => e.PasswordHash).IsRequired();
                entity.Property(e => e.CreatedAt).HasDefaultValueSql("GETDATE()");
            });

            // Spot configuration
            modelBuilder.Entity<Spot>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Name).IsRequired().HasMaxLength(200);
                entity.Property(e => e.Latitude).IsRequired();
                entity.Property(e => e.Longitude).IsRequired();
                entity.Property(e => e.Description).HasMaxLength(1000);
                entity.Property(e => e.Level).IsRequired();
                entity.Property(e => e.CreatedAt).HasDefaultValueSql("GETDATE()");

                entity.HasOne(e => e.Creator)
                    .WithMany(u => u.Spots)
                    .HasForeignKey(e => e.CreatedBy)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            // CheckIn configuration
            modelBuilder.Entity<CheckIn>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.DateTime).HasDefaultValueSql("GETDATE()");
                entity.Property(e => e.Notes).HasMaxLength(500);

                entity.HasOne(e => e.User)
                    .WithMany(u => u.CheckIns)
                    .HasForeignKey(e => e.UserId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(e => e.Spot)
                    .WithMany(s => s.CheckIns)
                    .HasForeignKey(e => e.SpotId)
                    .OnDelete(DeleteBehavior.Cascade);
            });
        }
    }
}