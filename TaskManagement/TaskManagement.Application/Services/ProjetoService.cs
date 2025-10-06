using AutoMapper;
using TaskManagement.Application.Interfaces.Persistence;
using TaskManagement.Application.Models.Payloads;
using TaskManagement.Domain.Entities;
using TaskManagement.Domain.Entities.Views;
using TaskManagement.Domain.Enums;

namespace TaskManagement.Application.Services
{
    /// <summary>
    /// Serviço responsável pela orquestração de operações relacionadas a projetos,
    /// incluindo criação, atualização, consulta e exclusão.
    /// </summary>
    public class ProjetoService : IProjetoService
    {
        /// <summary>
        /// Unidade de trabalho que coordena os repositórios e persistência.
        /// </summary>
        private readonly IUnitOfWork _unitOfWork;

        /// <summary>
        /// Instância do AutoMapper utilizada para conversão entre entidades e view models.
        /// </summary>
        private readonly IMapper _mapper;

        /// <summary>
        /// Inicializa uma nova instância do <see cref="ProjetoService"/>.
        /// </summary>
        /// <param name="unitOfWork">Unidade de trabalho para acesso aos repositórios.</param>
        /// <param name="mapper">Instância do AutoMapper.</param>
        public ProjetoService(IUnitOfWork unitOfWork, IMapper mapper)
            => (_unitOfWork, _mapper) = (unitOfWork, mapper);

        /// <summary>
        /// Obtém os dados completos de um projeto, incluindo suas tarefas e comentários.
        /// </summary>
        /// <param name="projetoId">Identificador do projeto.</param>
        /// <returns>ViewModel do projeto, se encontrado; caso contrário, lança exceção.</returns>
        /// <exception cref="KeyNotFoundException">Lançada se o projeto não for encontrado.</exception>
        public async Task<ProjetoViewModel?> ObterProjetoCompletoAsync(Guid projetoId)
        {
            var projeto = await _unitOfWork.Projetos.ObterPorIdAsync(projetoId);
            if (projeto is null)
                throw new KeyNotFoundException("Projeto não encontrado.");

            return _mapper.Map<ProjetoViewModel>(projeto);
        }

        /// <summary>
        /// Obtém todos os projetos vinculados a um usuário específico.
        /// </summary>
        /// <param name="UserId">Identificador do usuário.</param>
        /// <returns>Lista de projetos resumidos associados ao usuário.</returns>
        public async Task<IEnumerable<ProjetoResumoViewModel>> ObterProjetosPorUsuarioAsync(Guid UserId)
        {
            var projetos = await _unitOfWork.Projetos.ObterTodosPorUserIdAsync(UserId);
            return _mapper.Map<IEnumerable<ProjetoResumoViewModel>>(projetos);
        }

        /// <summary>
        /// Verifica se um projeto pode ser removido com base nas tarefas pendentes.
        /// </summary>
        /// <param name="projetoId">Identificador do projeto.</param>
        /// <returns><c>true</c> se todas as tarefas estiverem concluídas; caso contrário, <c>false</c>.</returns>
        public async Task<bool> PodeRemoverProjetoAsync(Guid projetoId)
        {
            var tarefasPendentes = await _unitOfWork.Tarefas.ObterTodasPorProjetoIdAsync(projetoId);
            return tarefasPendentes.All(t => t.Status != TarefaStatus.Pendente);
        }

        /// <summary>
        /// Remove um projeto, desde que não possua tarefas pendentes.
        /// </summary>
        /// <param name="projetoId">Identificador do projeto.</param>
        /// <returns>Tarefa assíncrona de operação.</returns>
        /// <exception cref="InvalidOperationException">Lançada se houver tarefas pendentes no projeto.</exception>
        public async Task RemoverProjetoAsync(Guid projetoId)
        {
            if (!await PodeRemoverProjetoAsync(projetoId))
                throw new InvalidOperationException("Projeto possui tarefas pendentes. Conclua ou remova as tarefas antes de excluir o projeto.");

            await _unitOfWork.Projetos.RemoverAsync(projetoId);
            await _unitOfWork.CommitAsync();
        }

        /// <summary>
        /// Cria um novo projeto para o usuário, validando conflitos de nome.
        /// </summary>
        /// <param name="payload">Dados necessários para criação do projeto.</param>
        /// <returns>Tarefa assíncrona de operação.</returns>
        /// <exception cref="InvalidOperationException">Lançada se já existir um projeto com o mesmo nome para o usuário.</exception>
        public async Task CriarProjetoAsync(NovoProjetoPayload payload)
        {
            var existeConflito = await _unitOfWork.Projetos.ValidaConflitoProjetoNomeAsync(payload.Nome, payload.UserId);
            if (existeConflito)
                throw new InvalidOperationException("Já existe um projeto com esse nome para o usuário.");

            var projeto = new Projeto(payload.Nome, payload.UserId);
            await _unitOfWork.Projetos.AdicionarAsync(projeto);
            await _unitOfWork.CommitAsync();
        }

        /// <summary>
        /// Atualiza o nome de um projeto existente, validando a propriedade do usuário.
        /// </summary>
        /// <param name="payload">Dados atualizados do projeto.</param>
        /// <returns>Tarefa assíncrona de operação.</returns>
        /// <exception cref="UnauthorizedAccessException">Lançada se o projeto não pertencer ao usuário.</exception>
        public async Task AtualizarProjetoAsync(AtualizaProjetoPayload payload)
        {
            var projeto = await _unitOfWork.Projetos.ObterPorIdAsync(payload.ProjetoId);
            if (projeto == null || projeto.UserId != payload.UserId)
                throw new UnauthorizedAccessException("Projeto não encontrado ou não pertence ao usuário.");

            var existeConflito = await _unitOfWork.Projetos.ValidaConflitoProjetoNomeAsync(payload.Nome, payload.UserId);
            if (existeConflito)
                throw new InvalidOperationException("Já existe um projeto com esse nome para o usuário.");

            projeto.AtualizarNome(payload.Nome);
            await _unitOfWork.Projetos.AtualizarAsync(projeto);
            await _unitOfWork.CommitAsync();
        }
    }
}
