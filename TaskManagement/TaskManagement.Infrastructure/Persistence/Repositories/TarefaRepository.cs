using Microsoft.EntityFrameworkCore;
using TaskManagement.Application.Interfaces.Persistence;
using TaskManagement.Domain.Entities;
using TaskManagement.Domain.Entities.Views;
using TaskManagement.Domain.Enums;

namespace TaskManagement.Infrastructure.Persistence.Repositories
{

    /// <summary>
    /// Implementação concreta do repositório de tarefas utilizando o <see cref="ProjetoDbContext"/>.
    /// </summary>
    /// <remarks>
    /// Este repositório fornece operações de leitura, escrita, atualização e exclusão para a entidade <see cref="Tarefa"/>.
    /// Também permite consultas por projeto, usuário e tarefas elegíveis para exclusão.
    /// </remarks>
    public class TarefaRepository : ITarefaRepository
    {
        /// <summary>
        /// Instância do contexto de dados utilizado para acesso ao banco.
        /// </summary>
        private readonly ProjetoDbContext _context;

        /// <summary>
        /// Inicializa uma nova instância de <see cref="TarefaRepository"/>.
        /// </summary>
        /// <param name="context">Instância do contexto de dados.</param>
        public TarefaRepository(ProjetoDbContext context) => _context = context;

        /// <summary>
        /// Obtém uma tarefa pelo seu identificador.
        /// </summary>
        /// <param name="tarefaId">Identificador da tarefa.</param>
        /// <returns>Instância da tarefa, se encontrada; caso contrário, <c>null</c>.</returns>
        /// <exception cref="ArgumentException">Lançada se o <paramref name="tarefaId"/> for inválido.</exception>
        public async Task<Tarefa?> ObterPorIdAsync(Guid tarefaId)
        {
            if (tarefaId == Guid.Empty)
                throw new ArgumentException("O identificador da tarefa não pode ser vazio.", nameof(tarefaId));

            return await _context.Set<Tarefa>()
                .Include(t => t.ComentariosInternos)
                .FirstOrDefaultAsync(t => t.TarefaId == tarefaId);
        }

        /// <summary>
        /// Adiciona uma nova tarefa ao banco de dados.
        /// </summary>
        /// <param name="tarefa">Instância da tarefa a ser adicionada.</param>
        /// <returns>Tarefa assíncrona de operação.</returns>
        /// <exception cref="ArgumentNullException">Lançada se a tarefa for nula.</exception>
        public async Task AdicionarAsync(Tarefa tarefa)
        {
            if (tarefa is null)
                throw new ArgumentNullException(nameof(tarefa), "A tarefa não pode ser nula.");
            
            await _context.Set<Tarefa>().AddAsync(tarefa);
            await _context.SaveChangesAsync();
        }

        /// <summary>
        /// Atualiza os dados de uma tarefa existente.
        /// </summary>
        /// <param name="tarefa">Instância da tarefa com dados atualizados.</param>
        /// <returns>Tarefa assíncrona de operação.</returns>
        /// <exception cref="ArgumentNullException">Lançada se a tarefa for nula.</exception>
        public async Task AtualizarAsync(Tarefa tarefa)
        {
            if (tarefa is null)
                throw new ArgumentNullException(nameof(tarefa), "A tarefa não pode ser nula.");

            _context.Set<Tarefa>().Update(tarefa);
            await _context.SaveChangesAsync();
        }

        /// <summary>
        /// Remove uma tarefa com base no seu identificador.
        /// </summary>
        /// <param name="tarefaId">Identificador da tarefa a ser removida.</param>
        /// <returns>Tarefa assíncrona de operação.</returns>
        /// <exception cref="ArgumentException">Lançada se o <paramref name="tarefaId"/> for inválido.</exception>
        public async Task RemoverAsync(Guid tarefaId)
        {
            if (tarefaId == Guid.Empty)
                throw new ArgumentException("O identificador da tarefa não pode ser vazio.", nameof(tarefaId));

            var tarefa = await ObterPorIdAsync(tarefaId);
            if (tarefa is not null)
            {
                _context.Set<Tarefa>().Remove(tarefa);
                await _context.SaveChangesAsync();
            }
        }

        /// <summary>
        /// Obtém todas as tarefas vinculadas a um projeto específico.
        /// </summary>
        /// <param name="projetoId">Identificador do projeto.</param>
        /// <returns>Lista de tarefas associadas ao projeto.</returns>
        /// <exception cref="ArgumentException">Lançada se o <paramref name="projetoId"/> for inválido.</exception>
        public async Task<IEnumerable<Tarefa>> ObterTodasPorProjetoIdAsync(Guid projetoId)
        {
            if (projetoId == Guid.Empty)
                throw new ArgumentException("O identificador do projeto não pode ser vazio.", nameof(projetoId));

            return await _context.Set<Tarefa>()
                .Where(t => t.ProjectId == projetoId)
                .ToListAsync();
        }

        /// <summary>
        /// Obtém todas as tarefas associadas a projetos de um usuário específico.
        /// </summary>
        /// <param name="UserId">Identificador do usuário.</param>
        /// <returns>Lista de tarefas vinculadas aos projetos do usuário.</returns>
        /// <exception cref="ArgumentException">Lançada se o <paramref name="UserId"/> for inválido.</exception>
        public async Task<IEnumerable<Tarefa>> ObterTodasPorUserIdAsync(Guid UserId)
        {
            if (UserId == Guid.Empty)
                throw new ArgumentException("O identificador do usuário não pode ser vazio.", nameof(UserId));

            return await _context.Set<Tarefa>()
                .Where(t => t.Projeto.UserId == UserId)
                .Include(t => t.Projeto)
                .ToListAsync();
        }

        /// <summary>
        /// Obtém todas as tarefas de um projeto que estão elegíveis para exclusão.
        /// </summary>
        /// <param name="projetoId">Identificador do projeto.</param>
        /// <returns>Lista de tarefas com status diferente de <see cref="TarefaStatus.Pendente"/>.</returns>
        /// <exception cref="ArgumentException">Lançada se o <paramref name="projetoId"/> for inválido.</exception>
        public async Task<IEnumerable<Tarefa>> ObterElegiveisParaExclusaoAsync(Guid projetoId)
        {
            if (projetoId == Guid.Empty)
                throw new ArgumentException("O identificador do projeto não pode ser vazio.", nameof(projetoId));

            return await _context.Set<Tarefa>()
                .Where(t => t.ProjectId == projetoId && t.Status != TarefaStatus.Pendente)
                .ToListAsync();
        }

        /// <summary>
        /// Obtém o número médio de tarefas concluídas por usuário nos últimos 30 dias.
        /// </summary>
        /// <returns>Lista com o UserId e média de tarefas concluídas.</returns>
        public async Task<IEnumerable<RelatorioDesempenhoViewModel>> ObterMediaTarefasConcluidasPorUsuarioAsync()
        {
            var dataLimite = DateTime.UtcNow.AddDays(-30);

            return await _context.Set<Tarefa>()
                .Where(t => t.Status == TarefaStatus.Concluida && t.DataConclusao >= dataLimite)
                .GroupBy(t => t.Projeto.UserId)
                .Select(g => new RelatorioDesempenhoViewModel
                {
                    UserId = g.Key,
                    NomeUsuario = $"Usuário {g.Key}",
                    TotalConcluidas = g.Count(),
                })
                .ToListAsync();
        }
    }
}
