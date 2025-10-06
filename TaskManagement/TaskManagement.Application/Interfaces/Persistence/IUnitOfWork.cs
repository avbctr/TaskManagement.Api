namespace TaskManagement.Application.Interfaces.Persistence
{
    /// <summary>
    /// Interface que representa uma unidade de trabalho, agrupando múltiplos repositórios e controle transacional.
    /// </summary>
    /// <remarks>
    /// A unidade de trabalho permite coordenar operações entre diferentes repositórios,
    /// garantindo consistência e controle sobre o ciclo de persistência.
    /// </remarks>
    public interface IUnitOfWork
    {
        /// <summary>
        /// Repositório de acesso à entidade <see cref="Projeto"/>.
        /// </summary>
        IProjetoRepository Projetos { get; }

        /// <summary>
        /// Repositório de acesso à entidade <see cref="Tarefa"/>.
        /// </summary>
        ITarefaRepository Tarefas { get; }

        /// <summary>
        /// Repositório de acesso à entidade <see cref="TarefaHistorico"/>.
        /// </summary>
        ITarefaHistoricoRepository Historicos { get; }

        /// <summary>
        /// Repositório de acesso à entidade <see cref="TarefaComentario"/>.
        /// </summary>
        ITarefaComentarioRepository Comentarios { get; }

        /// <summary>
        /// Salva todas as alterações pendentes no contexto.
        /// </summary>
        /// <returns>Número de registros afetados.</returns>
        Task<int> CommitAsync();

        /// <summary>
        /// Cancela alterações pendentes (simulado).
        /// </summary>
        /// <remarks>
        /// O Entity Framework Core não possui suporte nativo para rollback fora de transações explícitas.
        /// Este método é utilizado para compatibilidade e controle lógico.
        /// </remarks>
        Task RollbackAsync();
    }
}
