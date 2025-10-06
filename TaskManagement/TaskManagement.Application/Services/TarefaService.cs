using AutoMapper;
using TaskManagement.Application.Interfaces.Persistence;
using TaskManagement.Application.Models.Payloads;
using TaskManagement.Domain.Entities;
using TaskManagement.Domain.Entities.Views;
using TaskManagement.Domain.Enums;

namespace TaskManagement.Application.Services
{
    /// <summary>
    /// Serviço responsável pela orquestração de operações relacionadas a tarefas,
    /// incluindo criação, atualização, consulta e comentários.
    /// </summary>
    public class TarefaService : ITarefaService
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
        /// Inicializa uma nova instância do <see cref="TarefaService"/>.
        /// </summary>
        /// <param name="unitOfWork">Unidade de trabalho para acesso aos repositórios.</param>
        /// <param name="mapper">Instância do AutoMapper.</param>
        public TarefaService(IUnitOfWork unitOfWork, IMapper mapper)
            => (_unitOfWork, _mapper) = (unitOfWork, mapper);

        /// <summary>
        /// Obtém os dados completos de uma tarefa, incluindo comentários.
        /// </summary>
        /// <param name="tarefaId">Identificador da tarefa.</param>
        /// <returns>ViewModel da tarefa, se encontrada; caso contrário, lança exceção.</returns>
        /// <exception cref="KeyNotFoundException">Lançada se a tarefa não for encontrada.</exception>
        public async Task<TarefaViewModel?> ObterTarefaCompletaAsync(Guid tarefaId)
        {
            var tarefa = await _unitOfWork.Tarefas.ObterPorIdAsync(tarefaId);
            if (tarefa is null)
                throw new KeyNotFoundException("Tarefa não encontrada.");

            return _mapper.Map<TarefaViewModel>(tarefa);
        }

        /// <summary>
        /// Adiciona uma nova tarefa a um projeto existente, respeitando regras de negócio.
        /// </summary>
        /// <param name="payload">Dados necessários para criação da tarefa.</param>
        /// <returns>Tarefa assíncrona de operação.</returns>
        /// <exception cref="UnauthorizedAccessException">Lançada se o projeto não pertencer ao usuário.</exception>
        /// <exception cref="InvalidOperationException">Lançada se o limite de tarefas for atingido.</exception>
        public async Task AdicionarTarefaAsync(NovaTarefaPayload payload)
        {
            var projeto = await _unitOfWork.Projetos.ObterPorIdAsync(payload.ProjetoId);
            if (projeto == null || projeto.UserId != payload.UserId)
                throw new UnauthorizedAccessException("Projeto inválido ou não pertence ao usuário.");

            var tarefas = await _unitOfWork.Tarefas.ObterTodasPorProjetoIdAsync(payload.ProjetoId);
            if (tarefas.Count() >= 20)
                throw new InvalidOperationException("Limite máximo de 20 tarefas por projeto atingido.");

            var tarefa = new Tarefa(
                titulo: payload.Titulo,
                descricao: payload.Descricao,
                dataVencimento: payload.DataVencimento,
                prioridade: payload.Prioridade,
                projectId: payload.ProjetoId
            );

            await _unitOfWork.Tarefas.AdicionarAsync(tarefa);

            var historico = new TarefaHistorico(
                tarefa.TarefaId,
                "Tarefa criada.",
                tarefa.Status,
                tarefa.Prioridade,
                payload.UserId
            );

            await _unitOfWork.Historicos.RegistrarAsync(historico);
            await _unitOfWork.CommitAsync();
        }

        /// <summary>
        /// Atualiza os dados de uma tarefa existente, registrando o histórico da alteração.
        /// </summary>
        /// <param name="payload">Dados atualizados da tarefa.</param>
        /// <returns>Tarefa assíncrona de operação.</returns>
        /// <exception cref="KeyNotFoundException">Lançada se a tarefa não for encontrada.</exception>
        /// <exception cref="InvalidOperationException">Lançada se houver tentativa de alterar a prioridade.</exception>
        public async Task AtualizarTarefaAsync(AtualizaTarefaPayload payload)
        {
            var tarefa = await _unitOfWork.Tarefas.ObterPorIdAsync(payload.TarefaId);
            if (tarefa == null)
                throw new KeyNotFoundException("Tarefa não encontrada.");

            if(tarefa.Prioridade != payload.Prioridade)
                throw new InvalidOperationException("Prioridade da tarefa não pode ser alterada.");

            string _descricaoTratada = TrataDescricaoAlteracao(tarefa.Descricao, payload?.Descricao);

            tarefa.Atualizar(payload.Titulo, _descricaoTratada, payload.Status);

            await _unitOfWork.Tarefas.AtualizarAsync(tarefa);

            var historico = new TarefaHistorico(
                tarefa.TarefaId,
                _descricaoTratada,
                tarefa.Status,
                tarefa.Prioridade,
                payload.UserId
            );

            await _unitOfWork.Historicos.RegistrarAsync(historico);
            await _unitOfWork.CommitAsync();
        }

        /// <summary>
        /// Trata a descrição para alteração, verificando se houve mudança significativa.
        /// </summary>
        /// <param name="descricaoAtual"></param>
        /// <param name="novaDescricao"></param>
        /// <returns>Descrição Validada</returns>
        private string TrataDescricaoAlteracao(string descricaoAtual, string? novaDescricao)
        {
            if (String.IsNullOrWhiteSpace(novaDescricao) || descricaoAtual.Equals(novaDescricao, StringComparison.OrdinalIgnoreCase))
                return descricaoAtual;
            return novaDescricao;
        }

        /// <summary>
        /// Adiciona um novo comentário à tarefa e registra o histórico da ação.
        /// </summary>
        /// <param name="payload">Dados do comentário a ser adicionado.</param>
        /// <returns>Tarefa assíncrona de operação.</returns>
        public async Task AdicionarComentarioAsync(NovoComentarioPayload payload)
        {
            var comentario = new TarefaComentario(
                tarefaId: payload.TarefaId,
                conteudo: payload.Conteudo,
                UserId: payload.UserId
            );

            await _unitOfWork.Comentarios.AdicionarAsync(comentario);

            var historico = new TarefaHistorico(
                payload.TarefaId,
                $"Comentário adicionado: {payload.Conteudo}",
                TarefaStatus.Pendente,
                TarefaPrioridades.Medium,
                payload.UserId
            );

            await _unitOfWork.Historicos.RegistrarAsync(historico);
            await _unitOfWork.CommitAsync();
        }

        /// <summary>
        /// Deleta uma tarefa existente pelo seu identificador.
        /// </summary>
        /// <param name="tarefaId"></param>
        /// <returns></returns>
        public async Task DeletarTarefaAsync(Guid tarefaId)
        {
            await _unitOfWork.Tarefas.RemoverAsync(tarefaId);
            await _unitOfWork.CommitAsync();
        }

        /// <summary>
        /// Deleta um comentário existente pelo seu identificador.
        /// </summary>
        /// <param name="comentarioId"></param>
        /// <returns></returns>
        public async Task DeletaComentarioAsync(Guid comentarioId)
        {
            await _unitOfWork.Comentarios.ExcluirComentariosPoIdAsync(comentarioId);
            await _unitOfWork.CommitAsync();
        }

        /// <summary>
        /// Gera um relatório de desempenho dos usuários com base na média de tarefas concluídas.
        /// </summary>
        /// <returns>Relatório de desempenho dos usuários.</returns>
        public async Task<IEnumerable<RelatorioDesempenhoViewModel>> GerarRelatorioDesempenhoAsync()
            => await _unitOfWork.Tarefas.ObterMediaTarefasConcluidasPorUsuarioAsync();
    }
}

