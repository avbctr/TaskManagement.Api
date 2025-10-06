namespace TaskManagement.Application.Models.Payloads
{
    /// <summary>
    /// Representa o payload para criação de um novo projeto.
    /// </summary>
    /// <param name="Nome">O nome do projeto. Este valor não pode ser nulo ou vazio.</param>
    /// <param name="UserId">O identificador único do usuário associado ao projeto.</param>
    public record NovoProjetoPayload(string Nome, Guid UserId);
}
