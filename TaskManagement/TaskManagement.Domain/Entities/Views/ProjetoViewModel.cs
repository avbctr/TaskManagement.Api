using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace TaskManagement.Domain.Entities.Views
{
    /// <summary>
    /// Representa os dados completos de um projeto, incluindo suas tarefas associadas.
    /// </summary>
    public class ProjetoViewModel
    {
        /// <summary>
        /// Identificador exclusivo do projeto.
        /// </summary>
        [Display(Name = "Identificador do Projeto")]
        [Description("Identificador exclusivo do projeto.")]
        public Guid ProjetoId { get; set; }

        /// <summary>
        /// Nome atribuído ao projeto.
        /// </summary>
        [Display(Name = "Nome do Projeto")]
        [Description("Nome atribuído ao projeto.")]
        public string Nome { get; set; }

        /// <summary>
        /// Data e hora em que o projeto foi criado.
        /// </summary>
        [Display(Name = "Data de Criação")]
        [Description("Data e hora em que o projeto foi criado.")]
        public DateTime CriadoEm { get; set; }

        /// <summary>
        /// Data e hora da última atualização do projeto, se houver.
        /// </summary>
        [Display(Name = "Data de Atualização")]
        [Description("Data e hora da última atualização do projeto, se houver.")]
        public DateTime? AtualizadoEm { get; set; }

        /// <summary>
        /// Identificador do usuário proprietário do projeto.
        /// </summary>
        [Display(Name = "Identificador do Usuário")]
        [Description("Identificador do usuário proprietário do projeto.")]
        public Guid UserId { get; set; }

        /// <summary>
        /// Lista de tarefas vinculadas ao projeto.
        /// </summary>
        [Display(Name = "Tarefas do Projeto")]
        [Description("Lista de tarefas vinculadas ao projeto.")]
        public List<TarefaViewModel> Tarefas { get; set; } = new();
    }
}
