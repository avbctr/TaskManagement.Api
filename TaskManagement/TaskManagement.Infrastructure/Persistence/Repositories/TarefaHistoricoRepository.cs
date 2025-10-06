using Microsoft.EntityFrameworkCore;
using TaskManagement.Application.Interfaces.Persistence;
using TaskManagement.Domain.Entities;

namespace TaskManagement.Infrastructure.Persistence.Repositories
{
    /// <summary>
    /// Implementação concreta do repositório de históricos de tarefa utilizando o <see cref="ProjetoDbContext"/>.
    /// </summary>
    /// <remarks>
    /// Este repositório permite registrar alterações realizadas em tarefas e consultar o histórico completo por tarefa.
    /// </remarks>
    public class TarefaHistoricoRepository : ITarefaHistoricoRepository
    {
        /// <summary>
        /// Instância do contexto de dados utilizado para acesso ao banco.
        /// </summary>
        private readonly ProjetoDbContext _context;

        /// <summary>
        /// Inicializa uma nova instância de <see cref="TarefaHistoricoRepository"/>.
        /// </summary>
        /// <param name="context">Instância do contexto de dados.</param>
        public TarefaHistoricoRepository(ProjetoDbContext context) => _context = context;

        /// <summary>
        /// Registra um novo histórico de alteração para uma tarefa.
        /// </summary>
        /// <param name="historico">Instância do histórico a ser registrado.</param>
        /// <returns>Tarefa assíncrona de operação.</returns>
        /// <exception cref="ArgumentNullException">Lançada se o histórico for nulo.</exception>
        public async Task RegistrarAsync(TarefaHistorico historico)
        {
            if (historico is null)
                throw new ArgumentNullException(nameof(historico), "O histórico não pode ser nulo.");

            await _context.Set<TarefaHistorico>().AddAsync(historico);
            await _context.SaveChangesAsync();
        }

        /// <summary>
        /// Obtém todos os registros de histórico vinculados a uma tarefa.
        /// </summary>
        /// <param name="tarefaId">Identificador da tarefa.</param>
        /// <returns>Lista de registros históricos ordenados por data.</returns>
        /// <exception cref="ArgumentException">Lançada se o <paramref name="tarefaId"/> for inválido.</exception>
        public async Task<IEnumerable<TarefaHistorico>> ObterPorTarefaIdAsync(Guid tarefaId)
        {
            if (tarefaId == Guid.Empty)
                throw new ArgumentException("O identificador da tarefa não pode ser vazio.", nameof(tarefaId));

            return await _context.Set<TarefaHistorico>()
                .Where(h => h.TarefaId == tarefaId)
                .OrderBy(h => h.DataRegistro)
                .ToListAsync();
        }
    }
}
