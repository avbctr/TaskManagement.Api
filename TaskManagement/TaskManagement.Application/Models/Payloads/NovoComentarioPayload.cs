namespace TaskManagement.Application.Models.Payloads
{
    /// <summary>
    /// Representa os dados necessários para adicionar um novo comentário a uma tarefa.
    /// </summary>
    /// <param name="TarefaId">Identificador da tarefa à qual o comentário será vinculado.</param>
    /// <param name="Conteudo">Texto do comentário.</param>
    /// <param name="UserId">Identificador do usuário autor do comentário.</param>
    public record NovoComentarioPayload(Guid TarefaId, string Conteudo, Guid UserId);
}
