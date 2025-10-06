using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using TaskManagement.Domain.Enums;

namespace TaskManagement.Domain.Entities.Views
{
    /// <summary>
    /// Representa os dados completos de uma tarefa, incluindo seus comentários e histórico de alterações.
    /// </summary>
    public class TarefaViewModel
    {
        /// <summary>
        /// Identificador exclusivo da tarefa.
        /// </summary>
        [Display(Name = "Identificador da Tarefa")]
        [Description("Identificador exclusivo da tarefa.")]
        public Guid TarefaId { get; set; }

        /// <summary>
        /// Título da tarefa.
        /// </summary>
        [Display(Name = "Título")]
        [Description("Título da tarefa.")]
        public string Titulo { get; set; }

        /// <summary>
        /// Descrição detalhada da tarefa.
        /// </summary>
        [Display(Name = "Descrição")]
        [Description("Descrição detalhada da tarefa.")]
        public string Descricao { get; set; }

        /// <summary>
        /// Data limite para conclusão da tarefa.
        /// </summary>
        [Display(Name = "Data de Vencimento")]
        [Description("Data limite para conclusão da tarefa.")]
        public DateTime DataVencimento { get; set; }

        /// <summary>
        /// Prioridade atribuída à tarefa.
        /// </summary>
        [Display(Name = "Prioridade")]
        [Description("Prioridade atribuída à tarefa.")]
        public TarefaPrioridades Prioridade { get; set; }

        /// <summary>
        /// Status atual da tarefa.
        /// </summary>
        [Display(Name = "Status")]
        [Description("Status atual da tarefa.")]
        public TarefaStatus Status { get; set; }

        /// <summary>
        /// Lista de comentários associados à tarefa.
        /// </summary>
        [Display(Name = "Comentários")]
        [Description("Lista de comentários associados à tarefa.")]
        public List<TarefaComentarioViewModel> Comentarios { get; set; } = new();

        /// <summary>
        /// Histórico de alterações realizadas na tarefa.
        /// </summary>
        [Display(Name = "Histórico")]
        [Description("Histórico de alterações realizadas na tarefa.")]
        public List<TarefaHistoricoViewModel> Historico { get; set; } = new();
    }
}