using Asp.Versioning;
using Microsoft.AspNetCore.Mvc;
using TaskManagement.Application.Interfaces.Persistence;
using TaskManagement.Application.Models.Payloads;
using TaskManagement.Application.Models.Responses;
using TaskManagement.Domain.Entities.Views;

namespace TaskManagement.Api.Controllers.Projetos.V01
{
    /// <summary>
    /// Controlador responsável pelo gerenciamento de projetos.
    /// </summary>
    /// <remarks>
    /// Esta API permite que usuários criem, atualizem, visualizem e removam projetos.
    /// Cada projeto pode conter até 20 tarefas, e só pode ser removido se todas estiverem concluídas.
    /// </remarks>
    [ApiController]
    [ApiVersion("1.0")]
    [Route("v{version:apiVersion}/[controller]")]
    public class ProjetosController : ControllerBase
    {
        private readonly ILogger<ProjetosController> _logger;
        private readonly IProjetoService _projetoService;

        /// <summary>
        /// Inicializa uma nova instância do controlador de projetos.
        /// </summary>
        /// <param name="projetoService">Serviço de negócio responsável pelas operações de projeto.</param>
        public ProjetosController(IProjetoService projetoService, ILogger<ProjetosController> logger)
            => (_projetoService, _logger) = (projetoService, logger);

        /// <summary>
        /// Obtém os dados completos de um projeto, incluindo suas tarefas e comentários.
        /// </summary>
        /// <param name="id">Identificador único do projeto.</param>
        /// <returns>Objeto contendo os dados completos do projeto.</returns>
        /// <response code="200">Projeto encontrado com sucesso.</response>
        /// <response code="404">Projeto não encontrado.</response>
        [HttpGet("{id:guid}")]
        [ProducesResponseType(typeof(ApiResult<ProjetoViewModel?>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResult<ProjetoViewModel?>), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> ObterProjetoCompleto(Guid id)
        {
            try
            {
                var projeto = await _projetoService.ObterProjetoCompletoAsync(id);
                return Ok(ApiResult<ProjetoViewModel?>.Ok(projeto));
            }
            catch (KeyNotFoundException ky)
            {
                _logger.LogWarning(ky, "Projeto com ID {ProjetoId} não encontrado.", id);
                return NotFound(ApiResult<ProjetoViewModel?>.Fail("Projeto não encontrado.", new List<string> { "Verifique o ID do projeto e tente novamente." }));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao obter projeto com ID {ProjetoId}.", id);
                return BadRequest(ApiResult<ProjetoViewModel?>.Fail("Não foi possível obter os dados do projeto. Tente novamente mais tarde.",
                    new List<string> { "Falha interna ao processar a requisição." }));
            }
        }

        /// <summary>
        /// Lista todos os projetos vinculados a um usuário específico.
        /// </summary>
        /// <param name="UserId">Identificador do usuário.</param>
        /// <returns>Lista de projetos resumidos.</returns>
        /// <response code="200">Projetos encontrados com sucesso.</response>
        [HttpGet("usuario/{UserId:guid}")]
        [ProducesResponseType(typeof(ApiResult<IEnumerable<ProjetoResumoViewModel>>), StatusCodes.Status200OK)]
        public async Task<IActionResult> ObterProjetosPorUsuario(Guid UserId)
        {
            try
            {
                if (UserId == Guid.Empty)
                    return BadRequest(ApiResult<IEnumerable<ProjetoResumoViewModel>>.Fail("UserId inválido.", new List<string> { "O UserId não pode ser vazio." }));

                var projetos = await _projetoService.ObterProjetosPorUsuarioAsync(UserId);
                return Ok(ApiResult<IEnumerable<ProjetoResumoViewModel>>.Ok(projetos));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao validar UserId {UserId}.", UserId);
                return BadRequest(ApiResult<IEnumerable<ProjetoResumoViewModel>>.Fail("Não foi possível processar a requisição. Tente novamente mais tarde.",
                    new List<string> { "Falha interna ao processar a requisição." }));
            }
        }

        /// <summary>
        /// Cria um novo projeto para o usuário.
        /// </summary>
        /// <param name="payload">Dados necessários para criação do projeto.</param>
        /// <returns>Mensagem de sucesso ou erro genérico.</returns>
        /// <response code="200">Projeto criado com sucesso.</response>
        /// <response code="400">Erro ao criar projeto.</response>
        [HttpPost]
        [ProducesResponseType(typeof(ApiResult<string>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResult<string>), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> CriarProjeto([FromBody] NovoProjetoPayload payload)
        {
            try
            {
                await _projetoService.CriarProjetoAsync(payload);
                return Ok(ApiResult<string>.Ok("Projeto criado com sucesso."));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao criar projeto com dados {@Projeto}", payload);
                return BadRequest(ApiResult<string>.Fail(
                    "Não foi possível criar o projeto.",
                    new List<string> { "Falha interna ao processar a requisição." }
                ));
            }
        }

        /// <summary>
        /// Atualiza o nome de um projeto existente.
        /// </summary>
        /// <param name="payload">Dados atualizados do projeto.</param>
        /// <returns>Mensagem de sucesso ou erro genérico.</returns>
        /// <response code="200">Projeto atualizado com sucesso.</response>
        /// <response code="400">Erro ao atualizar projeto.</response>
        [HttpPut]
        [ProducesResponseType(typeof(ApiResult<string>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResult<string>), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> AtualizarProjeto([FromBody] AtualizaProjetoPayload payload)
        {
            try
            {
                await _projetoService.AtualizarProjetoAsync(payload);
                return Ok(ApiResult<string>.Ok("Projeto atualizado com sucesso."));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao atualizar projeto com dados {@Projeto}", payload);
                return BadRequest(ApiResult<string>.Fail(
                    "Não foi possível atualizar o projeto.",
                    new List<string> { "Falha interna ao processar a requisição." }
                ));
            }
        }

        /// <summary>
        /// Remove um projeto, desde que não possua tarefas pendentes.
        /// </summary>
        /// <param name="id">Identificador do projeto.</param>
        /// <returns>Mensagem de sucesso ou erro genérico.</returns>
        /// <response code="200">Projeto removido com sucesso.</response>
        /// <response code="400">Erro ao remover projeto (ex: tarefas pendentes).</response>
        [HttpDelete("{id:guid}")]
        [ProducesResponseType(typeof(ApiResult<string>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResult<string>), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> RemoverProjeto(Guid id)
        {
            try
            {
                await _projetoService.RemoverProjetoAsync(id);
                return Ok(ApiResult<string>.Ok("Projeto removido com sucesso."));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao remover projeto com ID {ProjetoId}.", id);
                return BadRequest(ApiResult<string>.Fail(
                    "Não foi possível remover o projeto.",
                    new List<string> { "Verifique se todas as tarefas estão concluídas antes de tentar novamente." }
                ));
            }
        }
    }
}
