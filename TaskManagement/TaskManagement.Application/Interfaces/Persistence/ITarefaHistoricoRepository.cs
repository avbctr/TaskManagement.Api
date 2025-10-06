using TaskManagement.Domain.Entities;

namespace TaskManagement.Application.Interfaces.Persistence
{
    /// <summary>
    /// Interface responsável pelo acesso e manipulação dos registros históricos de tarefas.
    /// </summary>
    /// <remarks>
    /// Permite registrar alterações realizadas em tarefas e consultar o histórico completo vinculado a uma tarefa específica.
    /// </remarks>
    public interface ITarefaHistoricoRepository
    {
        /// <summary>
        /// Registra um novo histórico de alteração para uma tarefa.
        /// </summary>
        /// <param name="historico">Instância do histórico contendo os dados da alteração.</param>
        /// <returns>Tarefa assíncrona de operação.</returns>
        Task RegistrarAsync(TarefaHistorico historico);

        /// <summary>
        /// Obtém todos os registros de histórico vinculados a uma tarefa.
        /// </summary>
        /// <param name="tarefaId">Identificador da tarefa.</param>
        /// <returns>Lista de registros históricos ordenados por data.</returns>
        Task<IEnumerable<TarefaHistorico>> ObterPorTarefaIdAsync(Guid tarefaId);
    }
}
