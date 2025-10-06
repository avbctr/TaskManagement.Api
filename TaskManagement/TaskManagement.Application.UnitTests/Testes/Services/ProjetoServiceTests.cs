using AutoMapper;
using Moq;
using TaskManagement.Application.Interfaces.Persistence;
using TaskManagement.Application.Models.Payloads;
using TaskManagement.Application.Services;
using TaskManagement.Domain.Entities;
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

    }
}
