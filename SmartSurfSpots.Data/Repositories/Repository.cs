using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace SmartSurfSpots.Data.Repositories
{
    /// <summary>
    /// Implementação base genérica do padrão Repository usando Entity Framework Core.
    /// Trata das operações CRUD padrão para qualquer entidade.
    /// </summary>
    /// <typeparam name="T">O tipo da entidade.</typeparam>
    public class Repository<T> : IRepository<T> where T : class
    {
        protected readonly SurfDbContext _context;
        protected readonly DbSet<T> _dbSet;

        public Repository(SurfDbContext context)
        {
            _context = context;
            _dbSet = context.Set<T>();
        }

        /// <summary>
        /// Busca uma entidade pela sua Chave Primária (PK).
        /// </summary>
        public async Task<T> GetByIdAsync(int id)
        {
            return await _dbSet.FindAsync(id);
        }

        /// <summary>
        /// Retorna todos os registos da tabela.
        /// </summary>
        public async Task<IEnumerable<T>> GetAllAsync()
        {
            return await _dbSet.ToListAsync();
        }

        /// <summary>
        /// Filtra registos com base numa expressão Lambda (ex: x => x.Age > 18).
        /// </summary>
        public async Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate)
        {
            return await _dbSet.Where(predicate).ToListAsync();
        }

        /// <summary>
        /// Adiciona uma nova entidade e guarda as alterações imediatamente na BD.
        /// </summary>
        public async Task<T> AddAsync(T entity)
        {
            await _dbSet.AddAsync(entity);
            await _context.SaveChangesAsync(); // Persiste logo na BD
            return entity;
        }

        /// <summary>
        /// Atualiza uma entidade e guarda as alterações.
        /// </summary>
        public async Task UpdateAsync(T entity)
        {
            _dbSet.Update(entity);
            await _context.SaveChangesAsync();
        }

        /// <summary>
        /// Remove uma entidade e guarda as alterações.
        /// </summary>
        public async Task DeleteAsync(T entity)
        {
            _dbSet.Remove(entity);
            await _context.SaveChangesAsync();
        }

        /// <summary>
        /// Verifica se existe algum registo que cumpra a condição (mais eficiente que fazer Count() > 0).
        /// </summary>
        public async Task<bool> ExistsAsync(Expression<Func<T, bool>> predicate)
        {
            return await _dbSet.AnyAsync(predicate);
        }
    }
}