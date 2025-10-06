using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using TaskManagement.Domain.Enums;

namespace TaskManagement.Domain.Entities
{
    /// <summary>
    /// Representa um projeto que possui tarefas associadas, um identificador único e informações de propriedade.
    /// </summary>
    /// <remarks>
    /// Um projeto contém uma coleção de tarefas, identificadas por um ID exclusivo, e está vinculado a um usuário específico.
    /// A classe impõe um limite máximo de tarefas que podem ser adicionadas.
    /// </remarks>
    [Table("Projetos")]
    public class Projeto
    {
        /// <summary>
        /// Define o número máximo de tarefas permitidas por projeto.
        /// </summary>
        private const int LimiteMaxTarefas = 20;

        /// <summary>
        /// Identificador exclusivo do projeto.
        /// </summary>
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Display(Name = "Identificador do Projeto")]
        [Description("Identificador exclusivo do projeto.")]
        public Guid ProjetoId { get; private set; }

        /// <summary>
        /// Nome atribuído ao projeto.
        /// </summary>
        [Required]
        [Display(Name = "Nome do Projeto")]
        [Description("Nome atribuído ao projeto.")]
        [MaxLength(150)]
        public string Nome { get; private set; }

        /// <summary>
        /// Data e hora em que o projeto foi criado.
        /// </summary>
        [Required]
        [Display(Name = "Data de Criação")]
        [Description("Data e hora em que o projeto foi criado.")]
        [DataType(DataType.DateTime)]
        public DateTime CriadoEm { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// Data e hora da última atualização do projeto.
        /// </summary>
        [Display(Name = "Data de Atualização")]
        [Description("Data e hora da última atualização do projeto.")]
        [DataType(DataType.DateTime)]
        public DateTime? AtualizadoEm { get; set; }

        /// <summary>
        /// Identificador do usuário proprietário do projeto.
        /// </summary>
        [Required]
        [Display(Name = "Identificador do Usuário")]
        [Description("Identificador do usuário proprietário do projeto.")]
        public Guid UserId { get; private set; }

        /// <summary>
        /// Lista interna de tarefas vinculadas ao projeto. Utilizada para persistência e controle interno.
        /// </summary>
        [Description("Lista interna de tarefas vinculadas ao projeto.")]
        public ICollection<Tarefa> TarefasInternas { get; private set; } = new List<Tarefa>();

        /// <summary>
        /// Coleção de tarefas associadas ao projeto, exposta como somente leitura.
        /// </summary>
        [Display(Name = "Tarefas do Projeto")]
        [Description("Coleção de tarefas associadas ao projeto, exposta como somente leitura.")]
        public IReadOnlyCollection<Tarefa> Tarefas => TarefasInternas.ToList().AsReadOnly();

        /// <summary>
        /// Construtor utilizado pelo Entity Framework.
        /// </summary>
        private Projeto() { }

        /// <summary>
        /// Inicializa uma nova instância da classe <see cref="Projeto"/> com nome e usuário definidos.
        /// </summary>
        /// <param name="nome">Nome do projeto.</param>
        /// <param name="userId">Identificador do usuário proprietário.</param>
        public Projeto(string nome, Guid userId)
        {
            ProjetoId = Guid.NewGuid();
            Nome = nome;
            UserId = userId;
        }

        /// <summary>
        /// Verifica se todas as tarefas estão elegíveis para exclusão.
        /// </summary>
        /// <remarks>
        /// Uma tarefa é considerada não elegível para exclusão se seu status for <see cref="TarefaStatus.Pendente"/>.
        /// Este método retorna <see langword="true"/> se nenhuma tarefa estiver com o status <see cref="TarefaStatus.Pendente"/>;
        /// caso contrário, retorna <see langword="false"/>.
        /// </remarks>
        /// <returns>
        /// <see langword="true"/> se todas as tarefas forem elegíveis para exclusão; caso contrário, <see langword="false"/>.
        /// </returns>
        public bool TarefasElegiveisParaExclusao() => !TarefasInternas.Any(t => t.Status == TarefaStatus.Pendente);

        /// <summary>
        /// Adiciona uma nova tarefa ao projeto, respeitando o limite máximo permitido.
        /// </summary>
        /// <param name="tarefa">Instância da tarefa a ser adicionada.</param>
        /// <exception cref="InvalidOperationException">
        /// Lançada quando o número máximo de tarefas já foi atingido.
        /// </exception>
        public void AdicionaTarefa(Tarefa tarefa)
        {
            if (TarefasInternas.Count >= LimiteMaxTarefas)
                throw new InvalidOperationException("O projeto atingiu o número máximo de tarefas.");

            TarefasInternas.Add(tarefa);
        }

        /// <summary>
        /// Atualiza o nome do projeto, caso o novo nome seja válido.
        /// </summary>
        /// <param name="novoNome">Novo nome a ser atribuído ao projeto.</param>
        public void AtualizarNome(string novoNome)
        {
            if (!String.IsNullOrWhiteSpace(novoNome))
                Nome = novoNome;
        }
    }
}
