using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using TaskManagement.Domain.Enums;

namespace TaskManagement.Domain.Entities
{
    /// <summary>
    /// Representa uma tarefa dentro de um projeto, contendo informações como título, descrição, prioridade, status e histórico de atualizações.
    /// </summary>
    [Table("Tarefas")]
    public class Tarefa
    {
        /// <summary>
        /// Identificador exclusivo da tarefa.
        /// </summary>
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Display(Name = "Identificador da Tarefa")]
        [Description("Identificador exclusivo da tarefa.")]
        public Guid TarefaId { get; private set; }

        /// <summary>
        /// Título da tarefa.
        /// </summary>
        [Required]
        [MaxLength(100)]
        [Display(Name = "Título")]
        [Description("Título da tarefa.")]
        public string Titulo { get; private set; }

        /// <summary>
        /// Descrição detalhada da tarefa.
        /// </summary>
        [MaxLength(500)]
        [Display(Name = "Descrição")]
        [Description("Descrição detalhada da tarefa.")]
        public string Descricao { get; private set; }

        /// <summary>
        /// Data limite para conclusão da tarefa.
        /// </summary>
        [Display(Name = "Data de Vencimento")]
        [Description("Data limite para conclusão da tarefa.")]
        [DataType(DataType.DateTime)]
        public DateTime DataVencimento { get; private set; }

        /// <summary>
        /// Data em que a tarefa foi concluída. Pode ser nula se a tarefa ainda não foi concluída.
        /// </summary>
        [Display(Name = "Data de Conclusão")]
        [Description("Data em que a tarefa foi concluída.")]
        [DataType(DataType.DateTime)]
        public DateTime? DataConclusao { get; private set; }

        /// <summary>
        /// Prioridade atribuída à tarefa. Não pode ser alterada após a criação.
        /// </summary>
        [Display(Name = "Prioridade")]
        [Description("Prioridade atribuída à tarefa. Não pode ser alterada após a criação.")]
        public TarefaPrioridades Prioridade { get; private set; }

        /// <summary>
        /// Status atual da tarefa.
        /// </summary>
        [Display(Name = "Status")]
        [Description("Status atual da tarefa.")]
        public TarefaStatus Status { get; private set; }

        /// <summary>
        /// Identificador do projeto ao qual a tarefa pertence.
        /// </summary>
        [Display(Name = "Identificador do Projeto")]
        [Description("Identificador do projeto ao qual a tarefa pertence.")]
        public Guid ProjectId { get; private set; }

        /// <summary>
        /// Instância do projeto relacionado à tarefa.
        /// </summary>
        [Display(Name = "Projeto")]
        [Description("Instância do projeto relacionado à tarefa.")]
        [ForeignKey("ProjectId")]
        public virtual Projeto Projeto { get; private set; }

        /// <summary>
        /// Histórico de alterações realizadas na tarefa, exposto como coleção somente leitura.
        /// </summary>
        [Display(Name = "Histórico de Atualizações")]
        [Description("Histórico de alterações realizadas na tarefa.")]
        public IReadOnlyCollection<TarefaHistorico> AtualizaHistorico => HistoricosInternos.ToList().AsReadOnly();

        /// <summary>
        /// Coleção interna de registros de histórico da tarefa.
        /// </summary>
        [Description("Coleção interna de registros de histórico da tarefa.")]
        public virtual ICollection<TarefaHistorico> HistoricosInternos { get; private set; } = new List<TarefaHistorico>();

        /// <summary>
        /// Comentários associados à tarefa, expostos como coleção somente leitura.
        /// </summary>
        [Display(Name = "Comentários")]
        [Description("Comentários associados à tarefa.")]
        public IReadOnlyCollection<TarefaComentario> Comentarios => ComentariosInternos.ToList().AsReadOnly();

        /// <summary>
        /// Coleção interna de comentários vinculados à tarefa.
        /// </summary>
        [Description("Coleção interna de comentários vinculados à tarefa.")]
        public virtual ICollection<TarefaComentario> ComentariosInternos { get; private set; } = new List<TarefaComentario>();

        /// <summary>
        /// Construtor utilizado pelo Entity Framework.
        /// </summary>
        private Tarefa() { }

        /// <summary>
        /// Inicializa uma nova instância da tarefa com os dados fornecidos.
        /// </summary>
        /// <param name="titulo">Título da tarefa.</param>
        /// <param name="descricao">Descrição da tarefa.</param>
        /// <param name="dataVencimento">Data de vencimento da tarefa.</param>
        /// <param name="prioridade">Prioridade atribuída à tarefa.</param>
        /// <param name="projectId">Identificador do projeto ao qual a tarefa pertence.</param>
        public Tarefa(string titulo, string descricao, DateTime dataVencimento, TarefaPrioridades prioridade, Guid projectId)
        {
            TarefaId = Guid.NewGuid();
            Titulo = titulo;
            Descricao = descricao;
            DataVencimento = dataVencimento;
            Prioridade = prioridade;
            ProjectId = projectId;
            Status = TarefaStatus.Pendente;
        }

        /// <summary>
        /// Atualiza os dados da tarefa, exceto a prioridade.
        /// </summary>
        /// <param name="titulo">Novo título.</param>
        /// <param name="descricao">Nova descrição.</param>
        /// <param name="status">Novo status da tarefa.</param>
        public void Atualizar(string titulo, string descricao, TarefaStatus status)
        {
            if (!String.IsNullOrEmpty(titulo))
                Titulo = titulo;

            if (!String.IsNullOrEmpty(descricao))
                Descricao = descricao;

            if (status == TarefaStatus.Concluida)
                DataConclusao = DateTime.UtcNow;

            Status = status;
        }

        /// <summary>
        /// Atualiza apenas o status da tarefa.
        /// </summary>
        /// <param name="novoStatus">Novo status a ser atribuído.</param>
        public void AtualizarStatus(TarefaStatus novoStatus)
        {
            if (novoStatus == TarefaStatus.Concluida)
                DataConclusao = DateTime.UtcNow;

            Status = novoStatus;
        }

        /// <summary>
        /// Adiciona um novo registro ao histórico da tarefa.
        /// </summary>
        /// <param name="historyEntry">Instância do histórico a ser adicionada.</param>
        public void AdicionaHistorico(TarefaHistorico historyEntry) => HistoricosInternos.Add(historyEntry);

        /// <summary>
        /// Adiciona um novo comentário à tarefa.
        /// </summary>
        /// <param name="comment">Instância do comentário a ser adicionada.</param>
        public void AdicionaComentario(TarefaComentario comment) => ComentariosInternos.Add(comment);
    }
}
