using TaskManagement.Application.Models.Payloads;
using TaskManagement.Domain.Entities.Views;

namespace TaskManagement.Application.Interfaces.Persistence
{
    /// <summary>
    /// Interface responsável pela orquestração das operações relacionadas a projetos,
    /// incluindo criação, atualização, consulta e exclusão.
    /// </summary>
    /// <remarks>
    /// Esta interface define os contratos de serviço que encapsulam regras de negócio
    /// e validações para manipulação de projetos na aplicação.
    /// </remarks>
    public interface IProjetoService
    {
        /// <summary>
        /// Obtém os dados completos de um projeto, incluindo suas tarefas e comentários.
        /// </summary>
        /// <param name="projetoId">Identificador do projeto.</param>
        /// <returns>ViewModel do projeto, se encontrado; caso contrário, <c>null</c>.</returns>
        Task<ProjetoViewModel?> ObterProjetoCompletoAsync(Guid projetoId);

        /// <summary>
        /// Obtém todos os projetos vinculados a um usuário específico.
        /// </summary>
        /// <param name="UserId">Identificador do usuário.</param>
        /// <returns>Lista de projetos resumidos associados ao usuário.</returns>
        Task<IEnumerable<ProjetoResumoViewModel>> ObterProjetosPorUsuarioAsync(Guid UserId);

        /// <summary>
        /// Verifica se um projeto pode ser removido com base nas tarefas pendentes.
        /// </summary>
        /// <param name="projetoId">Identificador do projeto.</param>
        /// <returns><c>true</c> se todas as tarefas estiverem concluídas; caso contrário, <c>false</c>.</returns>
        Task<bool> PodeRemoverProjetoAsync(Guid projetoId);

        /// <summary>
        /// Remove um projeto, desde que não possua tarefas pendentes.
        /// </summary>
        /// <param name="projetoId">Identificador do projeto.</param>
        /// <returns>Tarefa assíncrona de operação.</returns>
        Task RemoverProjetoAsync(Guid projetoId);

        /// <summary>
        /// Cria um novo projeto para o usuário, validando conflitos de nome.
        /// </summary>
        /// <param name="payload">Dados necessários para criação do projeto.</param>
        /// <returns>Tarefa assíncrona de operação.</returns>
        Task CriarProjetoAsync(NovoProjetoPayload payload);

        /// <summary>
        /// Atualiza o nome de um projeto existente, validando a propriedade do usuário.
        /// </summary>
        /// <param name="payload">Dados atualizados do projeto.</param>
        /// <returns>Tarefa assíncrona de operação.</returns>
        Task AtualizarProjetoAsync(AtualizaProjetoPayload payload);
    }
}
