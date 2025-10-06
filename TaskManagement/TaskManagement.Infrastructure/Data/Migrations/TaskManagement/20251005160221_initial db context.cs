using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TaskManagement.Infrastructure.Data.Migrations.TaskManagement
{
    /// <inheritdoc />
#pragma warning disable CS8981 // The type name only contains lower-cased ascii characters. Such names may become reserved for the language.
    public partial class initialdbcontext : Migration
#pragma warning restore CS8981 // The type name only contains lower-cased ascii characters. Such names may become reserved for the language.
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "taskmanagement");

            migrationBuilder.CreateTable(
                name: "Projetos",
                schema: "taskmanagement",
                columns: table => new
                {
                    projeto_id = table.Column<Guid>(type: "uuid", nullable: false),
                    nome = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: false),
                    criado_em = table.Column<DateTime>(type: "timestamp without time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP AT TIME ZONE 'UTC'"),
                    atualizado_em = table.Column<DateTime>(type: "timestamp without time zone", nullable: true, defaultValueSql: "CURRENT_TIMESTAMP AT TIME ZONE 'UTC'"),
                    user_id = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_projetos", x => x.projeto_id);
                });

            migrationBuilder.CreateTable(
                name: "Tarefas",
                schema: "taskmanagement",
                columns: table => new
                {
                    tarefa_id = table.Column<Guid>(type: "uuid", nullable: false),
                    titulo = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    descricao = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    data_vencimento = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    prioridade = table.Column<int>(type: "integer", nullable: false),
                    status = table.Column<int>(type: "integer", nullable: false),
                    project_id = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_tarefas", x => x.tarefa_id);
                    table.ForeignKey(
                        name: "fk_tarefas_projetos_project_id",
                        column: x => x.project_id,
                        principalSchema: "taskmanagement",
                        principalTable: "Projetos",
                        principalColumn: "projeto_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TarefaComentarios",
                schema: "taskmanagement",
                columns: table => new
                {
                    comentario_id = table.Column<Guid>(type: "uuid", nullable: false),
                    tarefa_id = table.Column<Guid>(type: "uuid", nullable: false),
                    conteudo = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: false),
                    data_comentario = table.Column<DateTime>(type: "timestamp without time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP AT TIME ZONE 'UTC'"),
                    usuario_id = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_tarefa_comentarios", x => x.comentario_id);
                    table.ForeignKey(
                        name: "fk_tarefa_comentarios_tarefas_tarefa_id",
                        column: x => x.tarefa_id,
                        principalSchema: "taskmanagement",
                        principalTable: "Tarefas",
                        principalColumn: "tarefa_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TarefaHistoricos",
                schema: "taskmanagement",
                columns: table => new
                {
                    historico_id = table.Column<Guid>(type: "uuid", nullable: false),
                    tarefa_id = table.Column<Guid>(type: "uuid", nullable: false),
                    descricao = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    status = table.Column<int>(type: "integer", nullable: false),
                    prioridade = table.Column<int>(type: "integer", nullable: false),
                    data_registro = table.Column<DateTime>(type: "timestamp without time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP AT TIME ZONE 'UTC'"),
                    usuario_id = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_tarefa_historicos", x => x.historico_id);
                    table.ForeignKey(
                        name: "fk_tarefa_historicos_tarefas_tarefa_id",
                        column: x => x.tarefa_id,
                        principalSchema: "taskmanagement",
                        principalTable: "Tarefas",
                        principalColumn: "tarefa_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "ix_tarefa_comentarios_tarefa_id",
                schema: "taskmanagement",
                table: "TarefaComentarios",
                column: "tarefa_id");

            migrationBuilder.CreateIndex(
                name: "ix_tarefa_historicos_tarefa_id",
                schema: "taskmanagement",
                table: "TarefaHistoricos",
                column: "tarefa_id");

            migrationBuilder.CreateIndex(
                name: "ix_tarefas_project_id",
                schema: "taskmanagement",
                table: "Tarefas",
                column: "project_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TarefaComentarios",
                schema: "taskmanagement");

            migrationBuilder.DropTable(
                name: "TarefaHistoricos",
                schema: "taskmanagement");

            migrationBuilder.DropTable(
                name: "Tarefas",
                schema: "taskmanagement");

            migrationBuilder.DropTable(
                name: "Projetos",
                schema: "taskmanagement");
        }
    }
}
