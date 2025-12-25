using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace SmartSurfSpots.Data.Repositories
{
    /// <summary>
    /// Interface genérica que define o contrato padrão para operações de acesso a dados (CRUD).
    /// </summary>
    /// <typeparam name="T">O tipo da entidade (ex: User, Spot).</typeparam>
    public interface IRepository<T> where T : class
    {
        /// <summary>
        /// Obtém uma entidade específica pelo seu identificador único.
        /// </summary>
        /// <param name="id">O ID da entidade.</param>
        /// <returns>A entidade encontrada ou null.</returns>
        Task<T> GetByIdAsync(int id);

        /// <summary>
        /// Obtém todas as entidades registadas na base de dados.
        /// </summary>
        /// <returns>Uma lista de todas as entidades.</returns>
        Task<IEnumerable<T>> GetAllAsync();

        /// <summary>
        /// Procura entidades que correspondam a uma condição específica.
        /// </summary>
        /// <param name="predicate">Expressão Lambda com a condição (ex: x => x.Name == "Joao").</param>
        /// <returns>Lista de entidades que cumprem a condição.</returns>
        Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate);

        /// <summary>
        /// Adiciona uma nova entidade à base de dados.
        /// </summary>
        /// <param name="entity">O objeto a ser criado.</param>
        /// <returns>A entidade criada (geralmente com o ID atualizado).</returns>
        Task<T> AddAsync(T entity);

        /// <summary>
        /// Atualiza os dados de uma entidade existente.
        /// </summary>
        /// <param name="entity">O objeto com os dados atualizados.</param>
        Task UpdateAsync(T entity);

        /// <summary>
        /// Remove uma entidade da base de dados.
        /// </summary>
        /// <param name="entity">O objeto a ser removido.</param>
        Task DeleteAsync(T entity);

        /// <summary>
        /// Verifica se existe alguma entidade que cumpra a condição fornecida.
        /// </summary>
        /// <param name="predicate">A condição a verificar.</param>
        /// <returns>Verdadeiro se existir pelo menos um registo, Falso caso contrário.</returns>
        Task<bool> ExistsAsync(Expression<Func<T, bool>> predicate);
    }
}