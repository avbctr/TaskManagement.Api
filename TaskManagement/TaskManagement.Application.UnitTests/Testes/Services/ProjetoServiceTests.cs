using AutoMapper;
using Moq;
using TaskManagement.Application.Interfaces.Persistence;
using TaskManagement.Application.Models.Payloads;
using TaskManagement.Application.Models.Responses;
using TaskManagement.Application.Services;
using TaskManagement.Domain.Entities;
using TaskManagement.Domain.Entities.Views;
using TaskManagement.Domain.Enums;

namespace TaskManagement.Application.UnitTests.Testes.Services
{
    public class ProjetoServiceTests
    {
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly ProjetoService _service;

        public ProjetoServiceTests()
        {
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _mapperMock = new Mock<IMapper>();
            _service = new ProjetoService(_unitOfWorkMock.Object, _mapperMock.Object);
        }

        [Fact]
        public async Task RemoverProjeto_DeveFalhar_SeTarefaPendente()
        {
            var tarefas = new List<Tarefa> { new("Tarefa", "Desc", DateTime.Now.AddDays(1), TarefaPrioridades.High, Guid.NewGuid()) };

            _unitOfWorkMock.Setup(u => u.Tarefas.ObterTodasPorProjetoIdAsync(It.IsAny<Guid>())).ReturnsAsync(tarefas);

            var ex = await Assert.ThrowsAsync<InvalidOperationException>(() => _service.RemoverProjetoAsync(Guid.NewGuid()));
            Assert.Equal("Projeto possui tarefas pendentes. Conclua ou remova as tarefas antes de excluir o projeto.", ex.Message);
        }

        [Fact]
        public async Task CriarProjeto_DeveFalhar_SeNomeDuplicado()
        {
            var payload = new NovoProjetoPayload("Projeto X", Guid.NewGuid());
            _unitOfWorkMock.Setup(u => u.Projetos.ValidaConflitoProjetoNomeAsync(payload.Nome, payload.UserId)).ReturnsAsync(true);

            var ex = await Assert.ThrowsAsync<InvalidOperationException>(() => _service.CriarProjetoAsync(payload));
            Assert.Equal("Já existe um projeto com esse nome para o usuário.", ex.Message);
        }


        [Fact]
        public async Task CriarProjeto_DeveFuncionar_SeNomeNaoDuplicado()
        {
            var payload = new NovoProjetoPayload("Projeto Novo", Guid.NewGuid());

            _unitOfWorkMock.Setup(u => u.Projetos.ValidaConflitoProjetoNomeAsync(payload.Nome, payload.UserId)).ReturnsAsync(false);
            _unitOfWorkMock.Setup(u => u.Projetos.AdicionarAsync(It.IsAny<Projeto>())).Returns(Task.CompletedTask);
            _unitOfWorkMock.Setup(u => u.CommitAsync()).ReturnsAsync(1);

            await _service.CriarProjetoAsync(payload);

            _unitOfWorkMock.Verify(u => u.Projetos.AdicionarAsync(It.IsAny<Projeto>()), Times.Once);
            _unitOfWorkMock.Verify(u => u.CommitAsync(), Times.Once);
        }

        [Fact]
        public async Task AtualizarProjeto_DeveFuncionar_SeDadosValidos()
        {
            var projetoId = Guid.NewGuid();
            var userId = Guid.NewGuid();
            var payload = new AtualizaProjetoPayload(projetoId, "Novo Nome", userId);

            var projetoExistente = new Projeto("Antigo Nome", userId);

            _unitOfWorkMock.Setup(u => u.Projetos.ObterPorIdAsync(projetoId)).ReturnsAsync(projetoExistente);
            _unitOfWorkMock.Setup(u => u.Projetos.ValidaConflitoProjetoNomeAsync(payload.Nome, userId)).ReturnsAsync(false);
            _unitOfWorkMock.Setup(u => u.Projetos.AtualizarAsync(It.IsAny<Projeto>())).Returns(Task.CompletedTask);
            _unitOfWorkMock.Setup(u => u.CommitAsync()).ReturnsAsync(1);

            await _service.AtualizarProjetoAsync(payload);

            _unitOfWorkMock.Verify(u => u.Projetos.AtualizarAsync(It.IsAny<Projeto>()), Times.Once);
            _unitOfWorkMock.Verify(u => u.CommitAsync(), Times.Once);
        }

        [Fact]
        public async Task ObterProjetoCompleto_DeveRetornarProjeto_SeExistente()
        {
            var projetoId = Guid.NewGuid();
            var projeto = new Projeto("Projeto Teste", Guid.NewGuid());

            _unitOfWorkMock.Setup(u => u.Projetos.ObterPorIdAsync(projetoId)).ReturnsAsync(projeto);
            _mapperMock.Setup(m => m.Map<ProjetoViewModel>(projeto)).Returns(new ProjetoViewModel());

            var resultado = await _service.ObterProjetoCompletoAsync(projetoId);

            Assert.NotNull(resultado);
        }

        [Fact]
        public async Task ObterProjetosPorUsuario_DeveRetornarLista()
        {
            var userId = Guid.NewGuid();
            var projetos = new List<Projeto> { new("Projeto 1", userId), new("Projeto 2", userId) };

            _unitOfWorkMock.Setup(u => u.Projetos.ObterTodosPorUserIdAsync(userId)).ReturnsAsync(projetos);
            _mapperMock.Setup(m => m.Map<IEnumerable<ProjetoResumoViewModel>>(projetos)).Returns(new List<ProjetoResumoViewModel>());

            var resultado = await _service.ObterProjetosPorUsuarioAsync(userId);

            Assert.NotNull(resultado);
            Assert.IsAssignableFrom<IEnumerable<ProjetoResumoViewModel>>(resultado);
        }

        [Fact]
        public void ApiResult_Ok_DeveRetornarSucesso()
        {
            var result = ApiResult<string>.Ok("Tudo certo");

            Assert.True(result.Success);
            Assert.Equal("Tudo certo", result.Data);
            Assert.Null(result.Errors);
        }

        [Fact]
        public void ApiResult_Fail_DeveRetornarErro()
        {
            var result = ApiResult<string>.Fail("Erro", new List<string> { "Detalhe" });

            Assert.False(result.Success);
            Assert.Equal("Erro", result.Message);
            Assert.Contains("Detalhe", result.Errors ?? Enumerable.Empty<string>());
        }
    }
}
