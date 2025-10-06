using AutoMapper;
using Moq;
using TaskManagement.Application.Interfaces.Persistence;
using TaskManagement.Application.Models.Payloads;
using TaskManagement.Application.Services;
using TaskManagement.Domain.Entities;
using TaskManagement.Domain.Enums;

namespace TaskManagement.Application.UnitTests.Testes.Services
{
    public class TarefaServiceTests
    {
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly TarefaService _service;

        public TarefaServiceTests()
        {
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _mapperMock = new Mock<IMapper>();
            _service = new TarefaService(_unitOfWorkMock.Object, _mapperMock.Object);
        }

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
    }
}
