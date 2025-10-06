using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using TaskManagement.Domain.Entities;

namespace TaskManagement.Infrastructure.Persistence
{
    /// <summary>
    /// Representa o contexto de banco de dados para o módulo de gerenciamento de projetos.
    /// </summary>
    /// <remarks>
    /// Este DbContext define os conjuntos de entidades e configurações de mapeamento para Projeto, Tarefa, Comentário e Histórico.
    /// Também desabilita o comportamento de exclusão em cascata por padrão.
    /// </remarks>
    public class ProjetoDbContext : DbContext
    {
        /// <summary>
        /// Inicializa uma nova instância do <see cref="ProjetoDbContext"/> com as opções especificadas.
        /// </summary>
        /// <param name="options">Opções de configuração do contexto.</param>
        public ProjetoDbContext(DbContextOptions<ProjetoDbContext> options)
            : base(options)
        {
        }

        /// <summary>
        /// Conjunto de dados que representa os projetos armazenados no banco.
        /// </summary>
        public DbSet<Projeto> Projetos { get; set; }

        /// <summary>
        /// Conjunto de dados que representa as tarefas armazenadas no banco.
        /// </summary>
        public DbSet<Tarefa> Tarefas { get; set; }

        /// <summary>
        /// Conjunto de dados que representa os registros históricos de tarefas.
        /// </summary>
        public DbSet<TarefaHistorico> Historicos { get; set; }

        /// <summary>
        /// Conjunto de dados que representa os comentários feitos em tarefas.
        /// </summary>
        public DbSet<TarefaComentario> Comentarios { get; set; }

        /// <summary>
        /// Configura o modelo de entidades e suas relações no banco de dados.
        /// </summary>
        /// <param name="builder">Construtor de modelo utilizado para definir mapeamentos.</param>
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            #region Desabilita Cascate Delete ao Rodar Migration

            var cascadeFKs = builder.Model.GetEntityTypes()
                .SelectMany(t => t.GetForeignKeys())
                .Where(fk => !fk.IsOwnership && fk.DeleteBehavior == DeleteBehavior.Cascade)
                .ToList();

            cascadeFKs.ForEach(fk => fk.DeleteBehavior = DeleteBehavior.Restrict);

            #endregion

            // Configuração da entidade Projeto
            builder.Entity<Projeto>(entity =>
            {
                entity.ToTable("Projetos", "taskmanagement");

                entity.HasKey(p => p.ProjetoId);

                entity.Property(p => p.ProjetoId)
                    .IsRequired()
                    .ValueGeneratedOnAdd();

                entity.HasIndex(i => new { i.Nome, i.UserId}).IsUnique();

                entity.Property(p => p.Nome)
                    .IsRequired()
                    .HasMaxLength(150);

                entity.Property(p => p.UserId)
                    .IsRequired();

                entity.HasMany(p => p.TarefasInternas)
                    .WithOne(t => t.Projeto)
                    .HasForeignKey(t => t.ProjectId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.Ignore(p => p.Tarefas);

                entity.Property(p => p.CriadoEm)
                    .IsRequired()
                    .HasColumnType("timestamptz")
                    .HasDefaultValueSql("CURRENT_TIMESTAMP AT TIME ZONE 'UTC'")
                    .ValueGeneratedOnAdd();

                entity.Property(p => p.AtualizadoEm)
                    .HasColumnType("timestamptz")
                    .HasDefaultValueSql("CURRENT_TIMESTAMP AT TIME ZONE 'UTC'")
                    .ValueGeneratedOnUpdate();
            });

            // Configuração da entidade Tarefa
            builder.Entity<Tarefa>(entity =>
            {
                entity.ToTable("Tarefas", "taskmanagement");

                entity.HasKey(p => p.TarefaId);

                entity.Property(p => p.TarefaId)
                    .IsRequired()
                    .ValueGeneratedOnAdd();

                entity.HasIndex(i => new { i.Titulo, i.ProjectId }).IsUnique();

                entity.Property(p => p.Titulo)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.Property(p => p.Descricao)
                    .HasMaxLength(500);

                entity.Property(p => p.DataVencimento)
                    .IsRequired()
                    .HasColumnType("timestamptz");

                entity.Property(p => p.DataConclusao)
                    .HasColumnType("timestamptz");

                entity.Property(p => p.Prioridade)
                    .IsRequired()
                    .HasConversion<int>();

                entity.Property(p => p.Status)
                    .IsRequired()
                    .HasConversion<int>();

                entity.Property(p => p.ProjectId)
                    .IsRequired();

                entity.HasMany(c => c.ComentariosInternos)
                    .WithOne(c => c.Tarefa)
                    .HasForeignKey(c => c.TarefaId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.Ignore(p => p.AtualizaHistorico);
                entity.Ignore(p => p.Comentarios);
            });

            // Configuração da entidade TarefaHistorico
            builder.Entity<TarefaHistorico>(entity =>
            {
                entity.ToTable("TarefaHistoricos", "taskmanagement");

                entity.HasKey(h => h.HistoricoId);

                entity.Property(h => h.HistoricoId)
                    .IsRequired()
                    .ValueGeneratedOnAdd();

                entity.Property(h => h.TarefaId)
                    .IsRequired();

                entity.Property(h => h.Descricao)
                    .IsRequired()
                    .HasMaxLength(500);

                entity.Property(h => h.Status)
                    .IsRequired()
                    .HasConversion<int>();

                entity.Property(h => h.Prioridade)
                    .IsRequired()
                    .HasConversion<int>();

                entity.Property(h => h.DataRegistro)
                    .IsRequired()
                    .HasColumnType("timestamptz")
                    .HasDefaultValueSql("CURRENT_TIMESTAMP AT TIME ZONE 'UTC'")
                    .ValueGeneratedOnAdd();

                entity.Property(h => h.UsuarioId)
                    .IsRequired();

                entity.HasOne(h => h.Tarefa)
                    .WithMany(t => t.HistoricosInternos)
                    .HasForeignKey(h => h.TarefaId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // Configuração da entidade TarefaComentario
            builder.Entity<TarefaComentario>(entity =>
            {
                entity.ToTable("TarefaComentarios", "taskmanagement");

                entity.HasKey(c => c.ComentarioId);

                entity.Property(c => c.ComentarioId)
                    .IsRequired()
                    .ValueGeneratedOnAdd();

                entity.Property(c => c.TarefaId)
                    .IsRequired();

                entity.Property(c => c.Conteudo)
                    .IsRequired()
                    .HasMaxLength(1000);

                entity.Property(c => c.DataComentario)
                    .IsRequired()
                    .HasColumnType("timestamptz")
                    .HasDefaultValueSql("CURRENT_TIMESTAMP AT TIME ZONE 'UTC'")
                    .ValueGeneratedOnAdd();

                entity.Property(c => c.UsuarioId)
                    .IsRequired();

                entity.HasOne(c => c.Tarefa)
                    .WithMany(t => t.ComentariosInternos)
                    .HasForeignKey(c => c.TarefaId)
                    .OnDelete(DeleteBehavior.Cascade);
            });
        }
    }

    /// <summary>
    /// Fábrica de contexto utilizada em tempo de design para instanciar o <see cref="ProjetoDbContext"/>.
    /// Essa classe é necessária para operações de migração com o Entity Framework Core,
    /// como <c>dotnet ef migrations add</c> e <c>dotnet ef database update</c>,
    /// especialmente em projetos desacoplados ou bibliotecas de infraestrutura.
    /// </summary>
    public class ProjetoDdContextFactory : IDesignTimeDbContextFactory<ProjetoDbContext>
    {
        /// <summary>
        /// Cria uma nova instância de <see cref="ProjetoDbContext"/> com as configurações necessárias
        /// para execução de comandos de migração e atualização de banco de dados.
        /// </summary>
        /// <param name="args">Argumentos de linha de comando passados pelo CLI do EF Core (não utilizados).</param>
        /// <returns>Instância configurada de <see cref="ProjetoDbContext"/>.</returns>
        public ProjetoDbContext CreateDbContext(string[] args)
        {
            // Inicializa o construtor de opções para o DbContext
            var optionsBuilder = new DbContextOptionsBuilder<ProjetoDbContext>();

            // Connection string de exemplo para PostgreSQL.
            // ⚠️ ATENÇÃO: Nunca versionar credenciais reais. Use variáveis de ambiente ou arquivos seguros em produção.
            var connectionString = "Host=pgsql.avbc.dev;Port=5432;Database=avbc1;Username=avbc1;Password=Test01Api25;Pooling=true;SSL Mode=Prefer;Trust Server Certificate=true;SearchPath=taskmanagement"; //"Host=localhost;Port=5432;Database=seu_banco;Username=seu_usuario;Password=sua_senha;SearchPath=taskmanagement";

            // Configura o provedor Npgsql (PostgreSQL) com:
            // - Schema padrão definido via SearchPath
            // - Tabela de histórico de migrações armazenada no schema "taskmanagement"
            // - Convenção de nomes em snake_case para compatibilidade com PostgreSQL
            optionsBuilder
                .UseNpgsql(connectionString, npgsqlOptions =>
                    npgsqlOptions.MigrationsHistoryTable("__EFMigrationsHistory", "taskmanagement"))
                .UseSnakeCaseNamingConvention();

            // Retorna uma instância do contexto com as opções configuradas
            return new ProjetoDbContext(optionsBuilder.Options);
        }
    }
}
