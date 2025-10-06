using TaskManagement.Domain.Entities;

namespace TaskManagement.Application.Interfaces.Persistence
{
    /// <summary>
    /// Repositório para gerenciamento de tarefas.
    /// </summary>
    public interface ITarefaRepository
    {
        /// <summary>
        /// Obtém uma tarefa pelo seu identificador.
        /// </summary>
        Task<Tarefa?> ObterPorIdAsync(Guid tarefaId);

        /// <summary>
        /// Adiciona uma nova tarefa ao repositório.
        /// </summary>
        Task AdicionarAsync(Tarefa tarefa);

        /// <summary>
        /// Atualiza os dados de uma tarefa existente.
        /// </summary>
        Task AtualizarAsync(Tarefa tarefa);

        /// <summary>
        /// Remove uma tarefa do repositório.
        /// </summary>
        Task RemoverAsync(Guid tarefaId);

        /// <summary>
        /// Obtém todas as tarefas vinculadas a um projeto.
        /// </summary>
        Task<IEnumerable<Tarefa>> ObterTodasPorProjetoIdAsync(Guid projetoId);

        /// <summary>
        /// Obtém todas as tarefas de um usuário, agrupadas por projeto.
        /// </summary>
        Task<IEnumerable<Tarefa>> ObterTodasPorUserIdAsync(Guid UserId);

        /// <summary>
        /// Obtém todas as tarefas elegíveis para exclusão de um projeto.
        /// </summary>
        Task<IEnumerable<Tarefa>> ObterElegiveisParaExclusaoAsync(Guid projetoId);
    }
}
