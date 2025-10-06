using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace TaskManagement.Domain.Enums
{
    /// <summary>
    /// Define os possíveis estados de uma tarefa dentro de um projeto.
    /// </summary>
    public enum TarefaStatus
    {
        /// <summary>
        /// Tarefa ainda não iniciada.
        /// </summary>
        [Display(Name = "Pendente")]
        [Description("Tarefa ainda não iniciada.")]
        Pendente = 0,

        /// <summary>
        /// Tarefa em execução.
        /// </summary>
        [Display(Name = "Em Andamento")]
        [Description("Tarefa em execução.")]
        EmAndamento = 1,

        /// <summary>
        /// Tarefa finalizada.
        /// </summary>
        [Display(Name = "Concluída")]
        [Description("Tarefa finalizada.")]
        Concluida = 2
    }
}
