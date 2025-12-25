using SmartSurfSpots.Domain.DTOs;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SmartSurfSpots.Services.Interfaces
{
    /// <summary>
    /// Contrato para o serviço de gestão de Spots.
    /// Contém a lógica de negócio para criar, editar, listar e remover spots.
    /// </summary>
    public interface ISpotService
    {
        /// <summary>
        /// Obtém todos os spots registados.
        /// </summary>
        /// <returns>Lista de DTOs de spots.</returns>
        Task<IEnumerable<SpotDto>> GetAllSpotsAsync();

        /// <summary>
        /// Obtém um spot específico pelo ID.
        /// </summary>
        /// <param name="id">ID do spot.</param>
        /// <returns>O spot encontrado.</returns>
        Task<SpotDto> GetSpotByIdAsync(int id);

        /// <summary>
        /// Cria um novo spot associado a um utilizador.
        /// </summary>
        /// <param name="request">Dados do novo spot.</param>
        /// <param name="userId">ID do utilizador que está a criar (extraído do token).</param>
        /// <returns>O spot criado.</returns>
        Task<SpotDto> CreateSpotAsync(CreateSpotRequest request, int userId);

        /// <summary>
        /// Atualiza um spot existente.
        /// Deve verificar se o utilizador a editar é o dono do spot.
        /// </summary>
        /// <param name="id">ID do spot a editar.</param>
        /// <param name="request">Novos dados.</param>
        /// <param name="userId">ID do utilizador que tenta editar.</param>
        /// <returns>O spot atualizado.</returns>
        Task<SpotDto> UpdateSpotAsync(int id, UpdateSpotRequest request, int userId);

        /// <summary>
        /// Remove um spot do sistema.
        /// Deve verificar se o utilizador a apagar é o dono do spot.
        /// </summary>
        /// <param name="id">ID do spot.</param>
        /// <param name="userId">ID do utilizador que tenta apagar.</param>
        /// <returns>True se apagado com sucesso.</returns>
        Task<bool> DeleteSpotAsync(int id, int userId);
    }
}