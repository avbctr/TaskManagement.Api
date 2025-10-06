using Microsoft.EntityFrameworkCore;
using TaskManagement.Application.Interfaces.Persistence;
using TaskManagement.Domain.Entities;

namespace TaskManagement.Infrastructure.Persistence.Repositories
{
    /// <summary>
    /// Implementação concreta do repositório de projetos utilizando o <see cref="ProjetoDbContext"/>.
    /// </summary>
    /// <remarks>
    /// Este repositório permite operações de leitura, escrita, atualização e exclusão de projetos,
    /// além de consultas por usuário e validação de conflitos de nome.
    /// </remarks>
    public class ProjetoRepository : IProjetoRepository
    {
        /// <summary>
        /// Instância do contexto de dados utilizado para acesso ao banco.
        /// </summary>
        private readonly ProjetoDbContext _context;

        /// <summary>
        /// Inicializa uma nova instância de <see cref="ProjetoRepository"/>.
        /// </summary>
        /// <param name="context">Instância do contexto de dados.</param>
        public ProjetoRepository(ProjetoDbContext context) => _context = context;

        /// <summary>
        /// Obtém um projeto pelo seu identificador.
        /// </summary>
        /// <param name="projetoId">Identificador do projeto.</param>
        /// <returns>Instância do projeto, se encontrado; caso contrário, <c>null</c>.</returns>
        /// <exception cref="ArgumentException">Lançada se o <paramref name="projetoId"/> for inválido.</exception>
        public async Task<Projeto?> ObterPorIdAsync(Guid projetoId)
        {
            if (projetoId == Guid.Empty)
                throw new ArgumentException("O identificador do projeto não pode ser vazio.", nameof(projetoId));

            return await _context.Projetos
                .Include(p => p.TarefasInternas)
                .ThenInclude(t => t.ComentariosInternos)
                .FirstOrDefaultAsync(p => p.ProjetoId == projetoId);
        }

        /// <summary>
        /// Adiciona um novo projeto ao banco de dados.
        /// </summary>
        /// <param name="projeto">Instância do projeto a ser adicionado.</param>
        /// <returns>Tarefa assíncrona de operação.</returns>
        /// <exception cref="ArgumentNullException">Lançada se o projeto for nulo.</exception>
        public async Task AdicionarAsync(Projeto projeto)
        {
            if (projeto is null)
                throw new ArgumentNullException(nameof(projeto), "O projeto não pode ser nulo.");

            await _context.Projetos.AddAsync(projeto);
            await _context.SaveChangesAsync();
        }

        /// <summary>
        /// Atualiza os dados de um projeto existente.
        /// </summary>
        /// <param name="projeto">Instância do projeto com dados atualizados.</param>
        /// <returns>Tarefa assíncrona de operação.</returns>
        /// <exception cref="ArgumentNullException">Lançada se o projeto for nulo.</exception>
        public async Task AtualizarAsync(Projeto projeto)
        {
            if (projeto is null)
                throw new ArgumentNullException(nameof(projeto), "O projeto não pode ser nulo.");

            _context.Projetos.Update(projeto);
            await _context.SaveChangesAsync();
        }

        /// <summary>
        /// Remove um projeto com base no seu identificador.
        /// </summary>
        /// <param name="projetoId">Identificador do projeto a ser removido.</param>
        /// <returns>Tarefa assíncrona de operação.</returns>
        /// <exception cref="ArgumentException">Lançada se o <paramref name="projetoId"/> for inválido.</exception>
        public async Task RemoverAsync(Guid projetoId)
        {
            if (projetoId == Guid.Empty)
                throw new ArgumentException("O identificador do projeto não pode ser vazio.", nameof(projetoId));

            var projeto = await ObterPorIdAsync(projetoId);
            if (projeto is not null)
            {
                _context.Projetos.Remove(projeto);
                await _context.SaveChangesAsync();
            }
        }

        /// <summary>
        /// Obtém todos os projetos vinculados a um usuário específico.
        /// </summary>
        /// <param name="UserId">Identificador do usuário.</param>
        /// <returns>Lista de projetos associados ao usuário.</returns>
        /// <exception cref="ArgumentException">Lançada se o <paramref name="UserId"/> for inválido.</exception>
        public async Task<IEnumerable<Projeto>> ObterTodosPorUserIdAsync(Guid UserId)
        {
            if (UserId == Guid.Empty)
                throw new ArgumentException("O identificador do usuário não pode ser vazio.", nameof(UserId));

            return await _context.Projetos
                .Where(p => p.UserId == UserId)
                .ToListAsync();
        }

        /// <summary>
        /// Verifica se já existe um projeto com o mesmo nome para o mesmo usuário.
        /// </summary>
        /// <param name="nome">Nome do projeto.</param>
        /// <param name="UserId">Identificador do usuário.</param>
        /// <returns><c>true</c> se houver conflito; caso contrário, <c>false</c>.</returns>
        /// <exception cref="ArgumentException">Lançada se o <paramref name="UserId"/> for inválido.</exception>
        public async Task<bool> ValidaConflitoProjetoNomeAsync(string nome, Guid UserId)
        {
            if (UserId == Guid.Empty)
                throw new ArgumentException("O identificador do usuário não pode ser vazio.", nameof(UserId));

            return await _context.Projetos
                .AnyAsync(p => p.Nome == nome && p.UserId == UserId);
        }
    }
}
