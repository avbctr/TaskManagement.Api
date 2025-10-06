using TaskManagement.Domain.Entities;

namespace TaskManagement.Application.Interfaces.Persistence
{
    /// <summary>
    /// Repositório para gerenciamento de projetos.
    /// </summary>
    public interface IProjetoRepository
    {
        /// <summary>
        /// Obtém um projeto pelo seu identificador.
        /// </summary>
        Task<Projeto?> ObterPorIdAsync(Guid projetoId);

        /// <summary>
        /// Adiciona um novo projeto ao repositório.
        /// </summary>
        Task AdicionarAsync(Projeto projeto);

        /// <summary>
        /// Atualiza os dados de um projeto existente.
        /// </summary>
        Task AtualizarAsync(Projeto projeto);

        /// <summary>
        /// Remove um projeto do repositório.
        /// </summary>
        Task RemoverAsync(Guid projetoId);

        /// <summary>
        /// Obtém todos os projetos vinculados a um usuário.
        /// </summary>
        Task<IEnumerable<Projeto>> ObterTodosPorUserIdAsync(Guid UserId);

        /// <summary>
        /// Verifica se existe um projeto com o nome informado para o usuário.
        /// </summary>
        Task<bool> ValidaConflitoProjetoNomeAsync(string nome, Guid UserId);
    }
}
