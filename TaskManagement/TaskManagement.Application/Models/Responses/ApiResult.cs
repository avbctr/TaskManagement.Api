namespace TaskManagement.Application.Models.Responses
{
    /// <summary>
    /// Representa uma resposta padronizada da API.
    /// </summary>
    /// <typeparam name="T">Tipo do dado retornado.</typeparam>
    public class ApiResult<T>
    {
        /// <summary>
        /// Indica se a operação foi bem-sucedida.
        /// </summary>
        public bool Success { get; set; }

        /// <summary>
        /// Mensagem informativa ou de erro.
        /// </summary>
        public string? Message { get; set; }

        /// <summary>
        /// Dados retornados pela operação (se houver).
        /// </summary>
        public T? Data { get; set; }

        /// <summary>
        /// Lista de erros detalhados (se houver).
        /// </summary>
        public List<string>? Errors { get; set; }

        /// <summary>
        /// Cria uma resposta de sucesso.
        /// </summary>
        public static ApiResult<T> Ok(T data, string? message = null)
            => new() { Success = true, Data = data, Message = message };

        /// <summary>
        /// Cria uma resposta de falha com mensagem e erros.
        /// </summary>
        public static ApiResult<T> Fail(string message, List<string>? errors = null)
            => new() { Success = false, Message = message, Errors = errors };
    }
}
