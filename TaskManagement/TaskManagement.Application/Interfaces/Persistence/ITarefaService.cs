using TaskManagement.Application.Models.Payloads;
using TaskManagement.Domain.Entities.Views;

namespace TaskManagement.Application.Interfaces.Persistence
{
    /// <summary>
    /// Interface que define os serviços de negócio relacionados à entidade Tarefa.
    /// </summary>
    public interface ITarefaService
    {
        Task<TarefaViewModel?> ObterTarefaCompletaAsync(Guid tarefaId);

        /// <summary>
        /// Adiciona uma nova tarefa a um projeto, respeitando regras de negócio como limite de tarefas e vínculo com o usuário.
        /// </summary>
        /// <param name="payload">Dados necessários para criação da tarefa.</param>
        Task AdicionarTarefaAsync(NovaTarefaPayload payload);

        /// <summary>
        /// Atualiza os dados de uma tarefa existente, registrando histórico de alterações.
        /// </summary>
        /// <param name="payload">Dados de atualização da tarefa.</param>
        Task AtualizarTarefaAsync(AtualizaTarefaPayload payload);

        /// <summary>
        /// Adiciona um comentário à tarefa e registra essa ação no histórico.
        /// </summary>
        /// <param name="payload">Dados do comentário a ser adicionado.</param>
        Task AdicionarComentarioAsync(NovoComentarioPayload payload);

        /// <summary>
        /// Deleta uma tarefa específica, se estiver concluída, junto com seus comentários e histórico.
        /// </summary>
        /// <param name="tarefaId"></param>
        /// <returns></returns>
        Task DeletarTarefaAsync(Guid tarefaId);

        /// <summary>
        /// Deleta um comentário específico de uma tarefa.
        /// </summary>
        /// <param name="comentarioId"></param>
        /// <returns></returns>
        Task DeletaComentarioAsync(Guid comentarioId);

        /// <summary>
        /// Gera um relatório de desempenho dos usuários, incluindo total de tarefas concluídas e média diária.
        /// </summary>
        /// <returns>Relatório de desempenho dos usuários.</returns>
        Task<IEnumerable<RelatorioDesempenhoViewModel>> GerarRelatorioDesempenhoAsync();
    }
}
