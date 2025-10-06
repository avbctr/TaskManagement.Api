using TaskManagement.Application.Interfaces.Persistence;

namespace TaskManagement.Infrastructure.Persistence.Repositories
{
    /// <summary>
    /// Implementação concreta da unidade de trabalho, coordenando múltiplos repositórios e persistência.
    /// </summary>
    /// <remarks>
    /// Esta classe encapsula o contexto de banco de dados e expõe os repositórios necessários para manipulação das entidades.
    /// Também fornece métodos para commit e rollback de operações.
    /// </remarks>
    public class UnitOfWork : IUnitOfWork
    {
        /// <summary>
        /// Instância do contexto de banco de dados utilizado para persistência.
        /// </summary>
        private readonly ProjetoDbContext _context;

        /// <summary>
        /// Inicializa uma nova instância da <see cref="UnitOfWork"/>, configurando os repositórios com base no contexto fornecido.
        /// </summary>
        /// <param name="context">Contexto de banco de dados compartilhado entre os repositórios.</param>
        public UnitOfWork(ProjetoDbContext context)
        {
            _context = context;
            Projetos = new ProjetoRepository(_context);
            Tarefas = new TarefaRepository(_context);
            Historicos = new TarefaHistoricoRepository(_context);
            Comentarios = new TarefaComentarioRepository(_context);
        }

        /// <summary>
        /// Repositório de acesso à entidade <see cref="Projeto"/>.
        /// </summary>
        public IProjetoRepository Projetos { get; }

        /// <summary>
        /// Repositório de acesso à entidade <see cref="Tarefa"/>.
        /// </summary>
        public ITarefaRepository Tarefas { get; }

        /// <summary>
        /// Repositório de acesso à entidade <see cref="TarefaHistorico"/>.
        /// </summary>
        public ITarefaHistoricoRepository Historicos { get; }

        /// <summary>
        /// Repositório de acesso à entidade <see cref="TarefaComentario"/>.
        /// </summary>
        public ITarefaComentarioRepository Comentarios { get; }

        /// <summary>
        /// Persiste todas as alterações realizadas no contexto atual.
        /// </summary>
        /// <returns>Número de registros afetados.</returns>
        public async Task<int> CommitAsync() => await _context.SaveChangesAsync();

        /// <summary>
        /// Simula um rollback de transações. No EF Core, rollback explícito só é possível dentro de transações reais.
        /// </summary>
        /// <returns>Tarefa concluída imediatamente.</returns>
        public Task RollbackAsync() => Task.CompletedTask;

        /// <summary>
        /// Libera os recursos utilizados pelo contexto de banco de dados.
        /// </summary>
        public void Dispose() => _context.Dispose();
    }
}