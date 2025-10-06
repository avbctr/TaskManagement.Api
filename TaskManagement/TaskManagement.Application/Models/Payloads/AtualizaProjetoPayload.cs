namespace TaskManagement.Application.Models.Payloads
{
    /// <summary>
    /// Representa os dados necessários para atualizar o nome de um projeto existente.
    /// </summary>
    /// <param name="ProjetoId">Identificador do projeto a ser atualizado.</param>
    /// <param name="Nome">Novo nome do projeto.</param>
    /// <param name="UserId">Identificador do usuário proprietário do projeto.</param>
    public record AtualizaProjetoPayload(Guid ProjetoId, string Nome, Guid UserId);
}