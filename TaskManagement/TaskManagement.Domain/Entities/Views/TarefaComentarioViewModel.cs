using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace TaskManagement.Domain.Entities.Views
{
    /// <summary>
    /// Representa os dados de um comentário associado a uma tarefa.
    /// </summary>
    public class TarefaComentarioViewModel
    {
        /// <summary>
        /// Identificador exclusivo do comentário.
        /// </summary>
        [Display(Name = "Identificador do Comentário")]
        [Description("Identificador exclusivo do comentário.")]
        public Guid ComentarioId { get; set; }

        /// <summary>
        /// Conteúdo textual do comentário.
        /// </summary>
        [Display(Name = "Conteúdo")]
        [Description("Conteúdo textual do comentário.")]
        public string Conteudo { get; set; }

        /// <summary>
        /// Data e hora em que o comentário foi registrado.
        /// </summary>
        [Display(Name = "Data do Comentário")]
        [Description("Data e hora em que o comentário foi registrado.")]
        public DateTime DataComentario { get; set; }

        /// <summary>
        /// Identificador do usuário que realizou o comentário.
        /// </summary>
        [Display(Name = "Identificador do Usuário")]
        [Description("Identificador do usuário que realizou o comentário.")]
        public Guid UsuarioId { get; set; }
    }
}