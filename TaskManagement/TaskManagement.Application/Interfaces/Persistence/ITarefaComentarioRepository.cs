using TaskManagement.Domain.Entities;

namespace TaskManagement.Application.Interfaces.Persistence
{
    /// <summary>
    /// Interface responsável pelo acesso e manipulação dos comentários vinculados a tarefas.
    /// </summary>
    /// <remarks>
    /// Permite adicionar novos comentários e consultar todos os comentários associados a uma tarefa específica.
    /// </remarks>
    public interface ITarefaComentarioRepository
    {
        /// <summary>
        /// Adiciona um novo comentário à tarefa.
        /// </summary>
        /// <param name="comentario">Instância do comentário a ser adicionado.</param>
        /// <returns>Tarefa assíncrona de operação.</returns>
        Task AdicionarAsync(TarefaComentario comentario);

        /// <summary>
        /// Obtém todos os comentários vinculados a uma tarefa.
        /// </summary>
        /// <param name="tarefaId">Identificador da tarefa.</param>
        /// <returns>Lista de comentários ordenados por data.</returns>
        Task<IEnumerable<TarefaComentario>> ObterPorTarefaIdAsync(Guid tarefaId);

        /// <summary>
        /// Exclui um comentário pelo seu identificador.
        /// </summary>
        /// <param name="comentarioId"></param>
        /// <returns></returns>
        Task ExcluirComentariosPoIdAsync(Guid comentarioId);
    }
}
