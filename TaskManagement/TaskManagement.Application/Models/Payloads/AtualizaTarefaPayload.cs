using TaskManagement.Domain.Enums;

namespace TaskManagement.Application.Models.Payloads
{
    /// <summary>
    /// Representa os dados necessários para atualizar uma tarefa existente.
    /// </summary>
    /// <param name="TarefaId">Identificador da tarefa a ser atualizada.</param>
    /// <param name="Titulo">Novo título da tarefa (opcional).</param>
    /// <param name="Descricao">Nova descrição da tarefa (opcional).</param>
    /// <param name="Status">Novo status da tarefa.</param>
    /// <param name="Prioridade">Prioridade atual da tarefa (não pode ser alterada).</param>
    /// <param name="DescricaoAlteracao">Descrição da alteração realizada, usada para histórico.</param>
    /// <param name="UserId">Identificador do usuário que realizou a alteração.</param>
    public record AtualizaTarefaPayload(Guid TarefaId, string? Titulo, string? Descricao, TarefaStatus Status, TarefaPrioridades Prioridade, string DescricaoAlteracao, Guid UserId);
}
