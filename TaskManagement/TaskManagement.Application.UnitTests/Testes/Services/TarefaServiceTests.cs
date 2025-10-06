using AutoMapper;
using Moq;
using TaskManagement.Application.Interfaces.Persistence;
using TaskManagement.Application.Models.Payloads;
using TaskManagement.Application.Services;
using TaskManagement.Domain.Entities;
using TaskManagement.Domain.Entities.Views;
using TaskManagement.Domain.Enums;

namespace TaskManagement.Application.UnitTests.Testes.Services
{
    /// <summary>
    /// Teste unitário para a classe <see cref="TarefaService"/>.
    /// </summary>
    public class TarefaServiceTests
    {
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly TarefaService _service;

        /// <summary>
        /// Construtor que inicializa os mocks e o serviço a ser testado.
        /// </summary>
        public TarefaServiceTests()
        {
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _mapperMock = new Mock<IMapper>();
            _service = new TarefaService(_unitOfWorkMock.Object, _mapperMock.Object);
        }

        /// <summary>
        /// Adiciona uma nova tarefa e verifica se lança exceção ao exceder o limite de 20 tarefas por projeto.
        /// </summary>
        /// <returns>Resultado do teste.</returns>
        [Fact]
        public async Task AdicionarTarefa_DeveFalhar_SeLimiteExcedido()
        {
            var payload = new NovaTarefaPayload("Nova", "Desc", DateTime.Now.AddDays(1), TarefaPrioridades.Medium, Guid.NewGuid(), Guid.NewGuid());
            var tarefas = Enumerable.Range(1, 20).Select(i => new Tarefa($"Tarefa {i}", "Desc", DateTime.Now, TarefaPrioridades.Low, payload.ProjetoId)).ToList();

            _unitOfWorkMock.Setup(u => u.Projetos.ObterPorIdAsync(payload.ProjetoId)).ReturnsAsync(new Projeto("Projeto", payload.UserId));
            _unitOfWorkMock.Setup(u => u.Tarefas.ObterTodasPorProjetoIdAsync(payload.ProjetoId)).ReturnsAsync(tarefas);

            var ex = await Assert.ThrowsAsync<InvalidOperationException>(() => _service.AdicionarTarefaAsync(payload));
            Assert.Equal("Limite máximo de 20 tarefas por projeto atingido.", ex.Message);
        }

        /// <summary>
        /// Atualiza uma tarefa e verifica se lança exceção ao tentar alterar a prioridade.
        /// </summary>
        /// <returns>Resultado do teste.</returns>
        [Fact]
        public async Task AtualizarTarefa_DeveFalhar_SePrioridadeAlterada()
        {
            var tarefaId = Guid.NewGuid();
            var payload = new AtualizaTarefaPayload(tarefaId, "Novo Título", "Nova Desc", TarefaStatus.EmAndamento, TarefaPrioridades.High, "Alteração", Guid.NewGuid());
            var tarefaExistente = new Tarefa("Antigo", "Desc", DateTime.Now, TarefaPrioridades.Low, Guid.NewGuid());

            _unitOfWorkMock.Setup(u => u.Tarefas.ObterPorIdAsync(tarefaId)).ReturnsAsync(tarefaExistente);

            var ex = await Assert.ThrowsAsync<InvalidOperationException>(() => _service.AtualizarTarefaAsync(payload));
            Assert.Equal("Prioridade da tarefa não pode ser alterada.", ex.Message);
        }

        /// <summary>
        /// Adiciona um comentário a uma tarefa e verifica se o histórico é registrado corretamente.
        /// </summary>
        /// <returns>Resultado do teste.</returns>
        [Fact]
        public async Task AdicionarComentario_DeveRegistrarHistorico()
        {
            var payload = new NovoComentarioPayload(Guid.NewGuid(), "Comentário", Guid.NewGuid());

            _unitOfWorkMock.Setup(u => u.Comentarios.AdicionarAsync(It.IsAny<TarefaComentario>())).Returns(Task.CompletedTask);
            _unitOfWorkMock.Setup(u => u.Historicos.RegistrarAsync(It.Is<TarefaHistorico>(h => h.Descricao.Contains("Comentário adicionado")))).Returns(Task.CompletedTask);

            await _service.AdicionarComentarioAsync(payload);

            _unitOfWorkMock.Verify(u => u.Historicos.RegistrarAsync(It.IsAny<TarefaHistorico>()), Times.Once);
            _unitOfWorkMock.Verify(u => u.CommitAsync(), Times.Once);
        }

        /// <summary>
        /// Adiciona uma nova tarefa e verifica se funciona corretamente quando dentro do limite de 20 tarefas por projeto.
        /// </summary>
        /// <returns>Resultado do teste.</returns>
        [Fact]
        public async Task AdicionarTarefa_DeveFuncionar_SeDentroDoLimite()
        {
            var projetoId = Guid.NewGuid();
            var userId = Guid.NewGuid();
            var payload = new NovaTarefaPayload("Título", "Descrição", DateTime.UtcNow.AddDays(1), TarefaPrioridades.Medium, projetoId, userId);

            var projeto = new Projeto("Projeto", userId);
            var tarefas = new List<Tarefa>(); // menos de 20

            _unitOfWorkMock.Setup(u => u.Projetos.ObterPorIdAsync(projetoId)).ReturnsAsync(projeto);
            _unitOfWorkMock.Setup(u => u.Tarefas.ObterTodasPorProjetoIdAsync(projetoId)).ReturnsAsync(tarefas);
            _unitOfWorkMock.Setup(u => u.Tarefas.AdicionarAsync(It.IsAny<Tarefa>())).Returns(Task.CompletedTask);
            _unitOfWorkMock.Setup(u => u.Historicos.RegistrarAsync(It.IsAny<TarefaHistorico>())).Returns(Task.CompletedTask);
            _unitOfWorkMock.Setup(u => u.CommitAsync()).ReturnsAsync(1);

            await _service.AdicionarTarefaAsync(payload);

            _unitOfWorkMock.Verify(u => u.Tarefas.AdicionarAsync(It.IsAny<Tarefa>()), Times.Once);
            _unitOfWorkMock.Verify(u => u.Historicos.RegistrarAsync(It.IsAny<TarefaHistorico>()), Times.Once);
            _unitOfWorkMock.Verify(u => u.CommitAsync(), Times.Once);
        }

        /// <summary>
        /// Atualiza uma tarefa e verifica se funciona corretamente quando a prioridade não é alterada.
        /// </summary>
        /// <returns>Resultado do teste.</returns>
        [Fact]
        public async Task AtualizarTarefa_DeveFuncionar_SePrioridadeNaoAlterada()
        {
            var tarefaId = Guid.NewGuid();
            var userId = Guid.NewGuid();
            var payload = new AtualizaTarefaPayload(tarefaId, "Novo Título", "Nova Desc", TarefaStatus.EmAndamento, TarefaPrioridades.Low, "Alteração", userId);
            var tarefaExistente = new Tarefa("Título", "Desc", DateTime.Now, TarefaPrioridades.Low, Guid.NewGuid());

            _unitOfWorkMock.Setup(u => u.Tarefas.ObterPorIdAsync(tarefaId)).ReturnsAsync(tarefaExistente);
            _unitOfWorkMock.Setup(u => u.Tarefas.AtualizarAsync(It.IsAny<Tarefa>())).Returns(Task.CompletedTask);
            _unitOfWorkMock.Setup(u => u.Historicos.RegistrarAsync(It.IsAny<TarefaHistorico>())).Returns(Task.CompletedTask);
            _unitOfWorkMock.Setup(u => u.CommitAsync()).ReturnsAsync(1);

            await _service.AtualizarTarefaAsync(payload);

            _unitOfWorkMock.Verify(u => u.Tarefas.AtualizarAsync(It.IsAny<Tarefa>()), Times.Once);
            _unitOfWorkMock.Verify(u => u.Historicos.RegistrarAsync(It.IsAny<TarefaHistorico>()), Times.Once);
            _unitOfWorkMock.Verify(u => u.CommitAsync(), Times.Once);
        }

        /// <summary>
        /// Obtém uma tarefa completa e verifica se retorna nulo quando a tarefa não existe.
        /// </summary>
        /// <returns>Resultado do teste.</returns>
        [Fact]
        public async Task ObterTarefaCompleta_DeveRetornarTarefa_SeExistente()
        {
            var tarefaId = Guid.NewGuid();
            var tarefa = new Tarefa("Título", "Desc", DateTime.Now, TarefaPrioridades.Low, Guid.NewGuid());

            _unitOfWorkMock.Setup(u => u.Tarefas.ObterPorIdAsync(tarefaId)).ReturnsAsync(tarefa);
            _mapperMock.Setup(m => m.Map<TarefaViewModel>(tarefa)).Returns(new TarefaViewModel());

            var resultado = await _service.ObterTarefaCompletaAsync(tarefaId);

            Assert.NotNull(resultado);
        }

        /// <summary>
        /// Gera um relatório de desempenho e verifica se retorna dados corretamente.
        /// </summary>
        /// <returns>Resultado do teste.</returns>
        [Fact]
        public async Task GerarRelatorioDesempenho_DeveRetornarDados()
        {
            var relatorio = new List<RelatorioDesempenhoViewModel>
            {
                new() { UserId = Guid.NewGuid(), NomeUsuario = "Usuário A", TotalConcluidas = 10 }
            };

            _unitOfWorkMock.Setup(u => u.Tarefas.ObterMediaTarefasConcluidasPorUsuarioAsync()).ReturnsAsync(relatorio);

            var resultado = await _service.GerarRelatorioDesempenhoAsync();

            Assert.NotEmpty(resultado);
            Assert.Equal(10, resultado.First().TotalConcluidas);
        }

        /// <summary>
        /// Deleta uma tarefa e verifica se funciona corretamente quando a tarefa existe.
        /// </summary>
        /// <returns>Resultado do teste.</returns>
        [Fact]
        public async Task DeletarTarefa_DeveRemover_SeExistente()
        {
            var tarefaId = Guid.NewGuid();
            var tarefa = new Tarefa("Título", "Desc", DateTime.Now, TarefaPrioridades.Low, Guid.NewGuid());

            _unitOfWorkMock.Setup(u => u.Tarefas.ObterPorIdAsync(tarefaId)).ReturnsAsync(tarefa);
            _unitOfWorkMock.Setup(u => u.Tarefas.RemoverAsync(tarefaId)).Returns(Task.CompletedTask);
            _unitOfWorkMock.Setup(u => u.CommitAsync()).ReturnsAsync(1);

            await _service.DeletarTarefaAsync(tarefaId);

            _unitOfWorkMock.Verify(u => u.Tarefas.RemoverAsync(tarefaId), Times.Once);
            _unitOfWorkMock.Verify(u => u.CommitAsync(), Times.Once);
        }

        /// <summary>
        /// Deleta um comentário e verifica se funciona corretamente quando o comentário existe.
        /// </summary>
        /// <returns>Resultado do teste.</returns>
        [Fact]
        public async Task DeletaComentario_DeveRemover_SeExistente()
        {
            var comentarioId = Guid.NewGuid();

            _unitOfWorkMock.Setup(u => u.Comentarios.ExcluirComentariosPoIdAsync(comentarioId)).Returns(Task.CompletedTask);
            _unitOfWorkMock.Setup(u => u.CommitAsync()).ReturnsAsync(1);

            await _service.DeletaComentarioAsync(comentarioId);

            _unitOfWorkMock.Verify(u => u.Comentarios.ExcluirComentariosPoIdAsync(comentarioId), Times.Once);
            _unitOfWorkMock.Verify(u => u.CommitAsync(), Times.Once);
        }
    }
}
