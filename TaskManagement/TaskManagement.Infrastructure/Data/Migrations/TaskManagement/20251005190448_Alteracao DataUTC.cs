using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TaskManagement.Infrastructure.Data.Migrations.TaskManagement
{
    /// <inheritdoc />
    public partial class AlteracaoDataUTC : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "descricao",
                schema: "taskmanagement",
                table: "Tarefas",
                type: "character varying(500)",
                maxLength: 500,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(500)",
                oldMaxLength: 500);

            migrationBuilder.AlterColumn<DateTime>(
                name: "data_vencimento",
                schema: "taskmanagement",
                table: "Tarefas",
                type: "timestamptz",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "timestamp without time zone");

            migrationBuilder.AlterColumn<DateTime>(
                name: "data_registro",
                schema: "taskmanagement",
                table: "TarefaHistoricos",
                type: "timestamptz",
                nullable: false,
                defaultValueSql: "CURRENT_TIMESTAMP AT TIME ZONE 'UTC'",
                oldClrType: typeof(DateTime),
                oldType: "timestamp without time zone",
                oldDefaultValueSql: "CURRENT_TIMESTAMP AT TIME ZONE 'UTC'");

            migrationBuilder.AlterColumn<DateTime>(
                name: "data_comentario",
                schema: "taskmanagement",
                table: "TarefaComentarios",
                type: "timestamptz",
                nullable: false,
                defaultValueSql: "CURRENT_TIMESTAMP AT TIME ZONE 'UTC'",
                oldClrType: typeof(DateTime),
                oldType: "timestamp without time zone",
                oldDefaultValueSql: "CURRENT_TIMESTAMP AT TIME ZONE 'UTC'");

            migrationBuilder.AlterColumn<DateTime>(
                name: "criado_em",
                schema: "taskmanagement",
                table: "Projetos",
                type: "timestamptz",
                nullable: false,
                defaultValueSql: "CURRENT_TIMESTAMP AT TIME ZONE 'UTC'",
                oldClrType: typeof(DateTime),
                oldType: "timestamp without time zone",
                oldDefaultValueSql: "CURRENT_TIMESTAMP AT TIME ZONE 'UTC'");

            migrationBuilder.AlterColumn<DateTime>(
                name: "atualizado_em",
                schema: "taskmanagement",
                table: "Projetos",
                type: "timestamptz",
                nullable: true,
                defaultValueSql: "CURRENT_TIMESTAMP AT TIME ZONE 'UTC'",
                oldClrType: typeof(DateTime),
                oldType: "timestamp without time zone",
                oldNullable: true,
                oldDefaultValueSql: "CURRENT_TIMESTAMP AT TIME ZONE 'UTC'");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "descricao",
                schema: "taskmanagement",
                table: "Tarefas",
                type: "character varying(500)",
                maxLength: 500,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "character varying(500)",
                oldMaxLength: 500,
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "data_vencimento",
                schema: "taskmanagement",
                table: "Tarefas",
                type: "timestamp without time zone",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "timestamptz");

            migrationBuilder.AlterColumn<DateTime>(
                name: "data_registro",
                schema: "taskmanagement",
                table: "TarefaHistoricos",
                type: "timestamp without time zone",
                nullable: false,
                defaultValueSql: "CURRENT_TIMESTAMP AT TIME ZONE 'UTC'",
                oldClrType: typeof(DateTime),
                oldType: "timestamptz",
                oldDefaultValueSql: "CURRENT_TIMESTAMP AT TIME ZONE 'UTC'");

            migrationBuilder.AlterColumn<DateTime>(
                name: "data_comentario",
                schema: "taskmanagement",
                table: "TarefaComentarios",
                type: "timestamp without time zone",
                nullable: false,
                defaultValueSql: "CURRENT_TIMESTAMP AT TIME ZONE 'UTC'",
                oldClrType: typeof(DateTime),
                oldType: "timestamptz",
                oldDefaultValueSql: "CURRENT_TIMESTAMP AT TIME ZONE 'UTC'");

            migrationBuilder.AlterColumn<DateTime>(
                name: "criado_em",
                schema: "taskmanagement",
                table: "Projetos",
                type: "timestamp without time zone",
                nullable: false,
                defaultValueSql: "CURRENT_TIMESTAMP AT TIME ZONE 'UTC'",
                oldClrType: typeof(DateTime),
                oldType: "timestamptz",
                oldDefaultValueSql: "CURRENT_TIMESTAMP AT TIME ZONE 'UTC'");

            migrationBuilder.AlterColumn<DateTime>(
                name: "atualizado_em",
                schema: "taskmanagement",
                table: "Projetos",
                type: "timestamp without time zone",
                nullable: true,
                defaultValueSql: "CURRENT_TIMESTAMP AT TIME ZONE 'UTC'",
                oldClrType: typeof(DateTime),
                oldType: "timestamptz",
                oldNullable: true,
                oldDefaultValueSql: "CURRENT_TIMESTAMP AT TIME ZONE 'UTC'");
        }
    }
}
