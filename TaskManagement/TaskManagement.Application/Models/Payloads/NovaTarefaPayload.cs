using TaskManagement.Domain.Enums;

namespace TaskManagement.Application.Models.Payloads
{
    /// <summary>
    /// Representa os dados necessários para criar uma nova tarefa em um projeto.
    /// </summary>
    /// <param name="Titulo">Título da tarefa.</param>
    /// <param name="Descricao">Descrição detalhada da tarefa.</param>
    /// <param name="DataVencimento">Data limite para conclusão da tarefa.</param>
    /// <param name="Prioridade">Prioridade atribuída à tarefa.</param>
    /// <param name="ProjetoId">Identificador do projeto ao qual a tarefa será vinculada.</param>
    /// <param name="UserId">Identificador do usuário responsável pela criação.</param>
    public record NovaTarefaPayload(string Titulo, string Descricao, DateTime DataVencimento, TarefaPrioridades Prioridade, Guid ProjetoId, Guid UserId);
}
