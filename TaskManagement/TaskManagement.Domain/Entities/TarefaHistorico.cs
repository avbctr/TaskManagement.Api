using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using TaskManagement.Domain.Enums;

namespace TaskManagement.Domain.Entities
{
    /// <summary>
    /// Representa um registro de alteração realizado em uma tarefa de projeto.
    /// </summary>

    [Table("TarefaHistoricos")]
    public class TarefaHistorico
    {
        /// <summary>
        /// Identificador exclusivo do registro de histórico.
        /// </summary>
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Display(Name = "Identificador do Histórico")]
        [Description("Identificador exclusivo do registro de histórico.")]
        public Guid HistoricoId { get; private set; }

        /// <summary>
        /// Identificador da tarefa associada ao histórico.
        /// </summary>
        [Display(Name = "Identificador da Tarefa")]
        [Description("Identificador da tarefa associada ao histórico.")]
        public Guid TarefaId { get; private set; }

        /// <summary>
        /// Instância da tarefa relacionada.
        /// </summary>
        [Display(Name = "Tarefa")]
        [Description("Instância da tarefa relacionada.")]
        [ForeignKey("TarefaId")]
        public virtual Tarefa Tarefa { get; private set; }

        /// <summary>
        /// Descrição da alteração realizada.
        /// </summary>
        [Required]
        [MaxLength(500)]
        [Display(Name = "Descrição da Alteração")]
        [Description("Descrição da alteração realizada.")]
        public string Descricao { get; private set; }

        /// <summary>
        /// Status da tarefa após a alteração.
        /// </summary>
        [Display(Name = "Status Atual")]
        [Description("Status da tarefa após a alteração.")]
        public TarefaStatus Status { get; private set; }

        /// <summary>
        /// Prioridade da tarefa após a alteração.
        /// </summary>
        [Display(Name = "Prioridade Atual")]
        [Description("Prioridade da tarefa após a alteração.")]
        public TarefaPrioridades Prioridade { get; private set; }

        /// <summary>
        /// Data e hora em que a alteração foi registrada.
        /// </summary>
        [Display(Name = "Data de Registro")]
        [Description("Data e hora em que a alteração foi registrada.")]
        public DateTime DataRegistro { get; private set; }

        /// <summary>
        /// Identificador do usuário que realizou a alteração.
        /// </summary>
        [Display(Name = "Usuário Responsável")]
        [Description("Identificador do usuário que realizou a alteração.")]
        public Guid UsuarioId { get; private set; }

        /// <summary>
        /// Construtor utilizado pelo Entity Framework.
        /// </summary>
        private TarefaHistorico() { }

        /// <summary>
        /// Inicializa um novo registro de histórico com os dados fornecidos.
        /// </summary>
        /// <param name="tarefaId">ID da tarefa associada.</param>
        /// <param name="descricao">Descrição da alteração.</param>
        /// <param name="status">Status atual da tarefa.</param>
        /// <param name="prioridade">Prioridade atual da tarefa.</param>
        /// <param name="UserId">ID do usuário responsável pela alteração.</param>
        public TarefaHistorico(Guid tarefaId, string descricao, TarefaStatus status, TarefaPrioridades prioridade, Guid UserId)
        {
            HistoricoId = Guid.NewGuid();
            TarefaId = tarefaId;
            Descricao = descricao;
            Status = status;
            Prioridade = prioridade;
            UsuarioId = UserId;
            DataRegistro = DateTime.UtcNow;
        }
    }
}
