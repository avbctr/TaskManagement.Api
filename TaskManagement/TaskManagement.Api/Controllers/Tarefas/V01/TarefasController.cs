using Asp.Versioning;
using Microsoft.AspNetCore.Mvc;
using TaskManagement.Application.Interfaces.Persistence;
using TaskManagement.Application.Models.Payloads;
using TaskManagement.Application.Models.Responses;
using TaskManagement.Domain.Entities.Views;

namespace TaskManagement.Api.Controllers.Tarefas.V01
{
    /// <summary>
    /// Controlador responsável pelo gerenciamento de tarefas dentro de projetos.
    /// </summary>
    /// <remarks>
    /// Esta API permite criar, visualizar, atualizar e comentar tarefas vinculadas a projetos.
    /// Cada operação respeita regras de negócio como limite de tarefas por projeto, histórico de alterações e prioridade fixa.
    /// </remarks>
    [ApiController]
    [ApiVersion("1.0")]
    [Route("v{version:apiVersion}/[controller]")]
    public class TarefasController : ControllerBase
    {
        private readonly ILogger<TarefasController> _logger;
        private readonly ITarefaService _tarefaService;

        /// <summary>
        /// Inicializa uma nova instância do controlador de tarefas.
        /// </summary>
        /// <param name="tarefaService">Serviço de negócio responsável pelas operações de tarefa.</param>
        /// <param name="logger">Instância do logger para registro de eventos.</param>
        public TarefasController(ITarefaService tarefaService, ILogger<TarefasController> logger)
            => (_tarefaService, _logger) = (tarefaService, logger);

        /// <summary>
        /// Obtém os dados completos de uma tarefa específica.
        /// </summary>
        /// <param name="id">Identificador único da tarefa.</param>
        /// <returns>Objeto contendo título, descrição, status, prioridade, comentários e histórico da tarefa.</returns>
        /// <response code="200">Retorna os dados da tarefa, se encontrada.</response>
        /// <response code="404">Tarefa não encontrada.</response>
        [HttpGet("{id:guid}")]
        [ProducesResponseType(typeof(ApiResult<TarefaViewModel?>), StatusCodes.Status200OK)]
        public async Task<IActionResult> ObterTarefaCompleta(Guid id)
        {
            try
            {
                var tarefa = await _tarefaService.ObterTarefaCompletaAsync(id);
                return Ok(ApiResult<TarefaViewModel?>.Ok(tarefa));
            }
            catch (Exception ex)
            {
                // Loga o erro internamente (ex: Serilog, ILogger, etc.)
                _logger.LogError(ex, "Erro ao obter tarefa com ID {TarefaId}", id);

                return BadRequest(ApiResult<TarefaViewModel?>.Fail("Não foi possível obter os dados da tarefa. Tente novamente mais tarde.",
                    new List<string> { "Falha interna ao processar a requisição." } ));
            }
        }

        /// <summary>
        /// Cria uma nova tarefa dentro de um projeto existente.
        /// </summary>
        /// <param name="payload">Dados necessários para criação da tarefa.</param>
        /// <returns>Mensagem de sucesso ou erro.</returns>
        /// <response code="200">Tarefa criada com sucesso.</response>
        /// <response code="400">Erro ao criar tarefa (ex: limite de tarefas excedido).</response>
        [HttpPost]
        [ProducesResponseType(typeof(ApiResult<string>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResult<string>), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> CriarTarefa([FromBody] NovaTarefaPayload payload)
        {
            try
            {
                await _tarefaService.AdicionarTarefaAsync(payload);
                return Ok(ApiResult<string>.Ok("Tarefa criada com sucesso."));
            }
            catch (Exception ex)
            {
                // Loga o erro internamente (ex: Serilog, ILogger, etc.)
                _logger.LogError(ex, "Erro ao criar tarefa com dados {@Tarefa}", payload);

                return BadRequest(ApiResult<string>.Fail("Erro ao criar tarefa.",
                    new List<string> { "Falha interna ao processar a requisição." }));
            }
        }

        /// <summary>
        /// Atualiza os dados de uma tarefa existente, como título, descrição ou status.
        /// </summary>
        /// <param name="payload">Dados atualizados da tarefa.</param>
        /// <returns>Mensagem de sucesso ou erro.</returns>
        /// <response code="200">Tarefa atualizada com sucesso.</response>
        /// <response code="400">Erro ao atualizar tarefa.</response>
        [HttpPut]
        [ProducesResponseType(typeof(ApiResult<string>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResult<string>), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> AtualizarTarefa([FromBody] AtualizaTarefaPayload payload)
        {
            try
            {
                await _tarefaService.AtualizarTarefaAsync(payload);
                return Ok(ApiResult<string>.Ok("Tarefa atualizada com sucesso."));
            }
            catch (Exception ex)
            {
                // Loga o erro internamente (ex: Serilog, ILogger, etc.)
                _logger.LogError(ex, "Erro ao atualizar tarefa com dados {@Tarefa}", payload);

                return BadRequest(ApiResult<string>.Fail("Erro ao atualizar tarefa.", new List<string> { "Falha interna ao processar a requisição." }));
            }
        }

        /// <summary>
        /// Adiciona um comentário a uma tarefa existente.
        /// </summary>
        /// <param name="payload">Dados do comentário a ser adicionado.</param>
        /// <returns>Mensagem de sucesso ou erro.</returns>
        /// <response code="200">Comentário adicionado com sucesso.</response>
        /// <response code="400">Erro ao adicionar comentário.</response>
        [HttpPost("comentario")]
        [ProducesResponseType(typeof(ApiResult<string>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResult<string>), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> AdicionarComentario([FromBody] NovoComentarioPayload payload)
        {
            try
            {
                await _tarefaService.AdicionarComentarioAsync(payload);
                return Ok(ApiResult<string>.Ok("Comentário adicionado com sucesso."));
            }
            catch (Exception ex)
            {
                // Loga o erro internamente (ex: Serilog, ILogger, etc.)
                _logger.LogError(ex, "Erro ao adicionar comentário com dados {@Comentario}", payload);
                return BadRequest(ApiResult<string>.Fail("Erro ao adicionar comentário.", new List<string> { "Falha interna ao processar a requisição." }));
            }
        }

        /// <summary>
        /// Remove uma tarefa específica de um projeto.
        /// </summary>
        /// <param name="id"></param>
        /// <returns>Mensagem de sucesso ou erro.</returns>
        /// <response code="200">Comentário adicionado com sucesso.</response>
        /// <response code="400">Erro ao adicionar comentário.</response>
        [HttpDelete("{id:guid}")]
        [ProducesResponseType(typeof(ApiResult<string>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResult<string>), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> RemoverTarefa(Guid id)
        {
            try
            {
                await _tarefaService.DeletarTarefaAsync(id);
                return Ok(ApiResult<string>.Ok("Tarefa removida com sucesso."));
            }
            catch (Exception ex)
            {
                // Loga o erro internamente (ex: Serilog, ILogger, etc.)
                _logger.LogError(ex, "Erro ao remover tarefa com ID {TarefaId}", id);
                return BadRequest(ApiResult<string>.Fail("Erro ao remover tarefa.", new List<string> { "Falha interna ao processar a requisição." }));
            }
        }

        /// <summary>
        /// Exclui um comentário específico de uma tarefa.
        /// </summary>
        /// <param name="comentarioId"></param>
        /// <returns>Mensagem de sucesso ou erro.</returns>
        /// <response code="200">Comentário adicionado com sucesso.</response>
        /// <response code="400">Erro ao adicionar comentário.</response>
        [HttpDelete("comentario/{comentarioId:guid}")]
        [ProducesResponseType(typeof(ApiResult<string>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResult<string>), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> RemoverComentario(Guid comentarioId)
        {
            try
            {
                await _tarefaService.DeletaComentarioAsync(comentarioId);
                return Ok(ApiResult<string>.Ok("Comentário removido com sucesso."));
            }
            catch (Exception ex)
            {
                // Loga o erro internamente (ex: Serilog, ILogger, etc.)
                _logger.LogError(ex, "Erro ao remover comentário com ID {ComentarioId}", comentarioId);
                return BadRequest(ApiResult<string>.Fail("Erro ao remover comentário.", new List<string> { "Falha interna ao processar a requisição." }));
            }
        }
    }
}
