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
    /// <summary>
    /// Classe de testes unitários para <see cref="ProjetoService"/>.
    /// Valida regras de negócio relacionadas à criação, atualização, remoção e consulta de projetos.
    /// </summary>
    public class ProjetoServiceTests
    {
        /// <summary>
        /// Mock da unidade de trabalho que simula os repositórios e persistência.
        /// </summary>
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;

        /// <summary>
        /// Mock do AutoMapper utilizado para conversão entre entidades e view models.
        /// </summary>
        private readonly Mock<IMapper> _mapperMock;

        /// <summary>
        /// Instância do serviço de projeto sendo testado.
        /// </summary>
        private readonly ProjetoService _service;

        /// <summary>
        /// Inicializa os mocks e a instância do serviço para os testes.
        /// </summary>
        public ProjetoServiceTests()
        {
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _mapperMock = new Mock<IMapper>();
            _service = new ProjetoService(_unitOfWorkMock.Object, _mapperMock.Object);
        }

        /// <summary>
        /// Verifica se a tentativa de remover um projeto com tarefas pendentes lança exceção.
        /// </summary>
        [Fact]
        public async Task RemoverProjeto_DeveFalhar_SeTarefaPendente()
        {
            var tarefas = new List<Tarefa> { new("Tarefa", "Desc", DateTime.Now.AddDays(1), TarefaPrioridades.High, Guid.NewGuid()) };

            _unitOfWorkMock.Setup(u => u.Tarefas.ObterTodasPorProjetoIdAsync(It.IsAny<Guid>())).ReturnsAsync(tarefas);

            var ex = await Assert.ThrowsAsync<InvalidOperationException>(() => _service.RemoverProjetoAsync(Guid.NewGuid()));
            Assert.Equal("Projeto possui tarefas pendentes. Conclua ou remova as tarefas antes de excluir o projeto.", ex.Message);
        }

        /// <summary>
        /// Verifica se a criação de projeto falha quando já existe um nome duplicado para o mesmo usuário.
        /// </summary>
        [Fact]
        public async Task CriarProjeto_DeveFalhar_SeNomeDuplicado()
        {
            var payload = new NovoProjetoPayload("Projeto X", Guid.NewGuid());
            _unitOfWorkMock.Setup(u => u.Projetos.ValidaConflitoProjetoNomeAsync(payload.Nome, payload.UserId)).ReturnsAsync(true);

            var ex = await Assert.ThrowsAsync<InvalidOperationException>(() => _service.CriarProjetoAsync(payload));
            Assert.Equal("Já existe um projeto com esse nome para o usuário.", ex.Message);
        }

        /// <summary>
        /// Verifica se a criação de projeto funciona corretamente quando não há conflito de nome.
        /// </summary>
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

        /// <summary>
        /// Verifica se a atualização de projeto funciona corretamente com dados válidos.
        /// </summary>
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

        /// <summary>
        /// Verifica se a consulta de projeto completo retorna dados quando o projeto existe.
        /// </summary>
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

        /// <summary>
        /// Verifica se a consulta de projetos por usuário retorna uma lista válida.
        /// </summary>
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

        /// <summary>
        /// Verifica se o método ApiResult.Ok retorna sucesso corretamente.
        /// </summary>
        [Fact]
        public void ApiResult_Ok_DeveRetornarSucesso()
        {
            var result = ApiResult<string>.Ok("Tudo certo");

            Assert.True(result.Success);
            Assert.Equal("Tudo certo", result.Data);
            Assert.Null(result.Errors);
        }

        /// <summary>
        /// Verifica se o método ApiResult.Fail retorna erro corretamente.
        /// </summary>
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
