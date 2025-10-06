using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using TaskManagement.Domain.Enums;

namespace TaskManagement.Domain.Entities.Views
{
    /// <summary>
    /// Representa um registro histórico de alterações realizadas em uma tarefa.
    /// </summary>
    public class TarefaHistoricoViewModel
    {
        /// <summary>
        /// Identificador exclusivo do registro de histórico.
        /// </summary>
        [Display(Name = "Identificador do Histórico")]
        [Description("Identificador exclusivo do registro de histórico.")]
        public Guid HistoricoId { get; set; }

        /// <summary>
        /// Descrição da alteração realizada na tarefa.
        /// </summary>
        [Display(Name = "Descrição")]
        [Description("Descrição da alteração realizada na tarefa.")]
        public string Descricao { get; set; }

        /// <summary>
        /// Status da tarefa no momento do registro.
        /// </summary>
        [Display(Name = "Status")]
        [Description("Status da tarefa no momento do registro.")]
        public TarefaStatus Status { get; set; }

        /// <summary>
        /// Prioridade da tarefa no momento do registro.
        /// </summary>
        [Display(Name = "Prioridade")]
        [Description("Prioridade da tarefa no momento do registro.")]
        public TarefaPrioridades Prioridade { get; set; }

        /// <summary>
        /// Data e hora em que o registro foi criado.
        /// </summary>
        [Display(Name = "Data do Registro")]
        [Description("Data e hora em que o registro foi criado.")]
        public DateTime DataRegistro { get; set; }

        /// <summary>
        /// Identificador do usuário responsável pela alteração.
        /// </summary>
        [Display(Name = "Identificador do Usuário")]
        [Description("Identificador do usuário responsável pela alteração.")]
        public Guid UsuarioId { get; set; }
    }
}