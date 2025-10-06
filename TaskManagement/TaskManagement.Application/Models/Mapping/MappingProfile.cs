using AutoMapper;
using TaskManagement.Domain.Entities;
using TaskManagement.Domain.Entities.Views;

namespace TaskManagement.Application.Models.Mapping
{
    /// <summary>
    /// Perfil de mapeamento do AutoMapper que define as conversões entre entidades de domínio e seus respectivos ViewModels.
    /// </summary>
    /// <remarks>
    /// Este perfil é utilizado para transformar objetos de entidades como <see cref="Projeto"/>, <see cref="Tarefa"/> e seus componentes
    /// em modelos de visualização utilizados na camada de apresentação.
    /// </remarks>
    public class MappingProfile : Profile
    {
        /// <summary>
        /// Inicializa uma nova instância da classe <see cref="MappingProfile"/>, configurando os mapeamentos entre entidades e ViewModels.
        /// </summary>
        public MappingProfile()
        {
            CreateMap<Projeto, ProjetoViewModel>();
            CreateMap<Projeto, ProjetoResumoViewModel>();
            CreateMap<Tarefa, TarefaViewModel>();
            CreateMap<TarefaComentario, TarefaComentarioViewModel>();
            CreateMap<TarefaHistorico, TarefaHistoricoViewModel>();
        }
    }
}
