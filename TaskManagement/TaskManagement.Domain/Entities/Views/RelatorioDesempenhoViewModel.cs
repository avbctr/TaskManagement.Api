using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskManagement.Domain.Entities.Views
{
    /// <summary>
    /// DTO utilizado para representar o relatório de desempenho de tarefas concluídas por usuário.
    /// </summary>

    public class RelatorioDesempenhoViewModel
    {
        /// <summary>
        /// Identificador do usuário responsável pelas tarefas.
        /// </summary>
        public Guid UserId { get; set; }

        /// <summary>
        /// Nome do usuário (opcional, se estiver disponível no contexto).
        /// </summary>
        public string NomeUsuario { get; set; }

        /// <summary>
        /// Quantidade total de tarefas concluídas nos últimos 30 dias.
        /// </summary>
        public int TotalConcluidas { get; set; }

        /// <summary>
        /// Média diária de tarefas concluídas nos últimos 30 dias.
        /// </summary>
        public double MediaDiariaConcluidas => Math.Round(TotalConcluidas / 30.0, 2);
    }
}
