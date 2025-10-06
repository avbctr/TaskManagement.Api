using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace TaskManagement.Domain.Entities.Views
{
    /// <summary>
    /// Representa uma visão resumida de um projeto, contendo apenas os dados essenciais.
    /// </summary>
    public class ProjetoResumoViewModel
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
    }
}
