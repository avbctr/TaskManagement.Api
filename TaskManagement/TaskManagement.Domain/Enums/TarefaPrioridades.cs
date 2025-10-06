using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace TaskManagement.Domain.Enums
{
    /// <summary>
    /// Define os níveis de prioridade atribuídos a uma tarefa.
    /// </summary>
    public enum TarefaPrioridades
    {
        /// <summary>
        /// Prioridade baixa, geralmente atribuída a tarefas menos urgentes.
        /// </summary>
        [Display(Name = "Baixa")]
        [Description("Prioridade baixa, geralmente atribuída a tarefas menos urgentes.")]
        Low = 0,

        /// <summary>
        /// Prioridade média, indicada para tarefas importantes mas não críticas.
        /// </summary>
        [Display(Name = "Média")]
        [Description("Prioridade média, indicada para tarefas importantes mas não críticas.")]
        Medium = 1,

        /// <summary>
        /// Prioridade alta, usada para tarefas urgentes ou críticas.
        /// </summary>
        [Display(Name = "Alta")]
        [Description("Prioridade alta, usada para tarefas urgentes ou críticas.")]
        High = 2
    }
}
