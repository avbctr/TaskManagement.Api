using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TaskManagement.Infrastructure.Data.Migrations.TaskManagement
{
    /// <inheritdoc />
    public partial class AlteracaoRelacionamentosI : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "data_conclusao",
                schema: "taskmanagement",
                table: "Tarefas",
                type: "timestamptz",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "ix_tarefas_titulo_project_id",
                schema: "taskmanagement",
                table: "Tarefas",
                columns: new[] { "titulo", "project_id" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_projetos_nome_user_id",
                schema: "taskmanagement",
                table: "Projetos",
                columns: new[] { "nome", "user_id" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "ix_tarefas_titulo_project_id",
                schema: "taskmanagement",
                table: "Tarefas");

            migrationBuilder.DropIndex(
                name: "ix_projetos_nome_user_id",
                schema: "taskmanagement",
                table: "Projetos");

            migrationBuilder.DropColumn(
                name: "data_conclusao",
                schema: "taskmanagement",
                table: "Tarefas");
        }
    }
}
