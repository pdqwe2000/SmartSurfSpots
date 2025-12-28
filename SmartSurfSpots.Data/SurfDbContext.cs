using Microsoft.EntityFrameworkCore;
using SmartSurfSpots.Domain.Entities;
using System;

namespace SmartSurfSpots.Data
{
    /// <summary>
    /// Contexto da Base de Dados. Responsável por mapear as classes (Entities) para as tabelas da BD.
    /// É aqui que ocorre a comunicação direta com o SQL Server.
    /// </summary>
    public class SurfDbContext : DbContext
    {
        // Construtor que recebe as opções de configuração (ex: Connection String) injetadas pelo Program.cs
        public SurfDbContext(DbContextOptions<SurfDbContext> options) : base(options)
        {
        }

        // Definição das Tabelas na Base de Dados
        public DbSet<User> Users { get; set; }
        public DbSet<Spot> Spots { get; set; }
        public DbSet<CheckIn> CheckIns { get; set; }

        /// <summary>
        /// Método onde usamos a Fluent API para configurar o esquema da base de dados com precisão.
        /// </summary>
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // ==============================================================================
            // CONFIGURAÇÃO DE USER
            // ==============================================================================
            modelBuilder.Entity<User>(entity =>
            {
                entity.HasKey(e => e.Id); // Chave Primária

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.Property(e => e.Email)
                    .IsRequired()
                    .HasMaxLength(100);

                // IMPORTANTE: Cria um índice UNIQUE no email.
                // Isto garante ao nível da Base de Dados que não existem dois utilizadores com o mesmo email.
                // As Data Annotations [EmailAddress] validam formato, mas não validam duplicados. Isto sim.
                entity.HasIndex(e => e.Email).IsUnique();

                entity.Property(e => e.PasswordHash).IsRequired();

                // Define que a data é gerada pelo SQL Server (função GETDATE()) ao inserir
                entity.Property(e => e.CreatedAt);
            });

            // ==============================================================================
            // CONFIGURAÇÃO DE SPOT
            // ==============================================================================
            modelBuilder.Entity<Spot>(entity =>
            {
                entity.HasKey(e => e.Id);

                entity.Property(e => e.Name).IsRequired().HasMaxLength(200);
                entity.Property(e => e.Latitude).IsRequired();
                entity.Property(e => e.Longitude).IsRequired();
                entity.Property(e => e.Description).HasMaxLength(1000);

                // O Enum 'Level' será gravado como int (1, 2, 3)
                entity.Property(e => e.Level).IsRequired();

                entity.Property(e => e.CreatedAt);

                // Relação: Um Spot tem UM Criador (User), Um User tem MUITOS Spots.
                entity.HasOne(e => e.Creator)
                    .WithMany(u => u.Spots)
                    .HasForeignKey(e => e.CreatedBy)
                    // Restrict: Impede que se apague um User se ele tiver Spots criados.
                    // Evita deixar Spots órfãos na base de dados.
                    .OnDelete(DeleteBehavior.Restrict);
            });

            // ==============================================================================
            // CONFIGURAÇÃO DE CHECKIN
            // ==============================================================================
            modelBuilder.Entity<CheckIn>(entity =>
            {
                entity.HasKey(e => e.Id);

                entity.Property(e => e.DateTime);
                entity.Property(e => e.Notes).HasMaxLength(500);

                // Relação: CheckIn -> User
                entity.HasOne(e => e.User)
                    .WithMany(u => u.CheckIns)
                    .HasForeignKey(e => e.UserId)
                    // Cascade: Se apagar o Utilizador, apaga todos os seus CheckIns (limpeza automática).
                    .OnDelete(DeleteBehavior.Cascade);

                // Relação: CheckIn -> Spot
                entity.HasOne(e => e.Spot)
                    .WithMany(s => s.CheckIns)
                    .HasForeignKey(e => e.SpotId)
                    // Cascade: Se apagar o Spot, apaga todos os CheckIns feitos lá.
                    .OnDelete(DeleteBehavior.Cascade);
            });
        }
    }
}