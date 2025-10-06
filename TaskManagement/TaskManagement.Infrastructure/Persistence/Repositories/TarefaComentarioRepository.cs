using Microsoft.EntityFrameworkCore;
using TaskManagement.Application.Interfaces.Persistence;
using TaskManagement.Domain.Entities;

namespace TaskManagement.Infrastructure.Persistence.Repositories
{
    /// <summary>
    /// Implementação concreta do repositório de comentários de tarefa utilizando o <see cref="ProjetoDbContext"/>.
    /// </summary>
    /// <remarks>
    /// Este repositório permite adicionar novos comentários e consultar todos os comentários vinculados a uma tarefa específica.
    /// </remarks>
    public class TarefaComentarioRepository : ITarefaComentarioRepository
    {
        /// <summary>
        /// Instância do contexto de dados utilizado para acesso ao banco.
        /// </summary>
        private readonly ProjetoDbContext _context;

        /// <summary>
        /// Inicializa uma nova instância de <see cref="TarefaComentarioRepository"/>.
        /// </summary>
        /// <param name="context">Instância do contexto de dados.</param>
        public TarefaComentarioRepository(ProjetoDbContext context) => _context = context;

        /// <summary>
        /// Adiciona um novo comentário à tarefa.
        /// </summary>
        /// <param name="comentario">Instância do comentário a ser adicionado.</param>
        /// <returns>Tarefa assíncrona de operação.</returns>
        /// <exception cref="ArgumentNullException">Lançada se o comentário for nulo.</exception>
        public async Task AdicionarAsync(TarefaComentario comentario)
        {
            if (comentario is null)
                throw new ArgumentNullException(nameof(comentario), "O comentário não pode ser nulo.");

            await _context.Set<TarefaComentario>().AddAsync(comentario);
            await _context.SaveChangesAsync();
        }

        /// <summary>
        /// Obtém todos os comentários vinculados a uma tarefa.
        /// </summary>
        /// <param name="tarefaId">Identificador da tarefa.</param>
        /// <returns>Lista de comentários ordenados por data.</returns>
        /// <exception cref="ArgumentException">Lançada se o <paramref name="tarefaId"/> for inválido.</exception>
        public async Task<IEnumerable<TarefaComentario>> ObterPorTarefaIdAsync(Guid tarefaId)
        {
            if (tarefaId == Guid.Empty)
                throw new ArgumentException("O identificador da tarefa não pode ser vazio.", nameof(tarefaId));

            return await _context.Set<TarefaComentario>()
                .Where(c => c.TarefaId == tarefaId)
                .OrderBy(c => c.DataComentario)
                .ToListAsync();
        }

        /// <summary>
        /// Exclui um comentário pelo seu identificador.
        /// </summary>
        /// <param name="comentarioId"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        public async Task ExcluirComentariosPoIdAsync(Guid comentarioId)
        {
            if (comentarioId == Guid.Empty)
                throw new ArgumentException("O identificador da tarefa não pode ser vazio.", nameof(comentarioId));

            var comentarios = await _context.Set<TarefaComentario>()
                .FindAsync(comentarioId);

            if (comentarios != null)
            {
                _context.Set<TarefaComentario>().Remove(comentarios);
                await _context.SaveChangesAsync();
            }
        }
    }
}
