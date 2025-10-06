using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TaskManagement.Domain.Entities
{
    /// <summary>
    /// Representa um comentário feito em uma tarefa de projeto.
    /// </summary>
    [Table("TarefaComentarios")]
    public class TarefaComentario
    {
        /// <summary>
        /// Identificador exclusivo do comentário.
        /// </summary>
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Display(Name = "Identificador do Comentário")]
        [Description("Identificador exclusivo do comentário.")]
        public Guid ComentarioId { get; private set; }

        /// <summary>
        /// Identificador da tarefa associada ao comentário.
        /// </summary>
        [Display(Name = "Identificador da Tarefa")]
        [Description("Identificador da tarefa associada ao comentário.")]
        public Guid TarefaId { get; private set; }

        /// <summary>
        /// Instância da tarefa relacionada.
        /// </summary>
        [Display(Name = "Tarefa")]
        [Description("Instância da tarefa relacionada.")]
        [ForeignKey("TarefaId")]
        public virtual Tarefa Tarefa { get; private set; }

        /// <summary>
        /// Conteúdo textual do comentário.
        /// </summary>
        [Required]
        [MaxLength(1000)]
        [Display(Name = "Conteúdo")]
        [Description("Conteúdo textual do comentário.")]
        public string Conteudo { get; private set; }

        /// <summary>
        /// Data e hora em que o comentário foi feito.
        /// </summary>
        [Display(Name = "Data do Comentário")]
        [Description("Data e hora em que o comentário foi feito.")]
        public DateTime DataComentario { get; private set; }

        /// <summary>
        /// Identificador do autor do comentário.
        /// </summary>
        [Display(Name = "Autor")]
        [Description("Identificador do autor do comentário.")]
        public Guid UsuarioId { get; private set; }

        /// <summary>
        /// Construtor utilizado pelo Entity Framework.
        /// </summary>
        private TarefaComentario() { }

        /// <summary>
        /// Inicializa um novo comentário.
        /// </summary>
        /// <param name="tarefaId">ID da tarefa associada.</param>
        /// <param name="conteudo">Texto do comentário.</param>
        /// <param name="UserId">ID do autor do comentário.</param>
        public TarefaComentario(Guid tarefaId, string conteudo, Guid UserId)
        {
            ComentarioId = Guid.NewGuid();
            TarefaId = tarefaId;
            Conteudo = conteudo;
            UsuarioId = UserId;
            DataComentario = DateTime.UtcNow;
        }
    }
}
