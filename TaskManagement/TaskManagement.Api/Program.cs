using Asp.Versioning;
using Asp.Versioning.ApiExplorer;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using System.IO.Compression;
using System.Text.Json.Serialization;
using TaskManagement.Application.Interfaces.Persistence;
using TaskManagement.Application.Models.Mapping;
using TaskManagement.Application.Services;
using TaskManagement.Infrastructure.Persistence;
using TaskManagement.Infrastructure.Persistence.Repositories;

namespace TaskManagement.Api
{
    /// <summary>
    /// Classe principal responsável pela configuração e inicialização da aplicação.
    /// </summary>
    public static class Program
    {
        /// <summary>
        /// Ponto de entrada da aplicação.
        /// </summary>
        /// <param name="args">Argumentos de linha de comando.</param>
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // onfiguração do DbContext com PostgreSQL e otimizações de consulta
            builder.Services.AddDbContext<ProjetoDbContext>(options =>
            {
                options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"), npgsql =>
                {
                    npgsql.UseQuerySplittingBehavior(QuerySplittingBehavior.SplitQuery); // Evita joins pesados em Includes
                    npgsql.EnableRetryOnFailure(maxRetryCount: 5, maxRetryDelay: TimeSpan.FromSeconds(30), errorCodesToAdd: null); // Requisições resilientes
                    npgsql.CommandTimeout(60); // Tempo limite de 60 segundos para comandos SQL
                });

                options.UseSnakeCaseNamingConvention(); // Converte nomes para snake_case no PostgreSQL
                options.EnableSensitiveDataLogging(false); // Evita logar dados sensíveis
                options.EnableDetailedErrors(true); // Ativa mensagens de erro detalhadas
                options.LogTo(Console.WriteLine, LogLevel.Error); // Loga apenas erros no console
            });

            // Injeção de dependências - Repositórios e Unit of Work
            builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

            // Repositórios
            builder.Services.AddScoped<IProjetoRepository, ProjetoRepository>();
            builder.Services.AddScoped<ITarefaRepository, TarefaRepository>();
            builder.Services.AddScoped<ITarefaHistoricoRepository, TarefaHistoricoRepository>();
            builder.Services.AddScoped<ITarefaComentarioRepository, TarefaComentarioRepository>();

            // Camada de negócio
            builder.Services.AddScoped<IProjetoService, ProjetoService>();
            builder.Services.AddScoped<ITarefaService, TarefaService>();

            // AutoMapper para conversão entre entidades e ViewModels
            builder.Services.AddAutoMapper(cfg => cfg.AddProfile<MappingProfile>());

            // Configuração de controllers e serialização JSON
            builder.Services.AddControllers()
                .AddJsonOptions(options =>
                {
                    options.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull; // Ignora propriedades nulas
                });

            // Compressão Brotli e Gzip para respostas HTTP
            builder.Services.Configure<BrotliCompressionProviderOptions>(opts => opts.Level = CompressionLevel.Fastest);
            builder.Services.Configure<GzipCompressionProviderOptions>(opts => opts.Level = CompressionLevel.Fastest);
            builder.Services.AddResponseCompression(options =>
            {
                options.Providers.Add<BrotliCompressionProvider>();
                options.Providers.Add<GzipCompressionProvider>();
                options.EnableForHttps = true;
            });

            // Versionamento de API
            builder.Services.AddApiVersioning(options =>
            {
                options.AssumeDefaultVersionWhenUnspecified = true;
                options.DefaultApiVersion = new ApiVersion(1, 0);
                options.ReportApiVersions = true;
            }).AddApiExplorer(options => {
                options.GroupNameFormat = "'v'VVV"; // Ex: "v1"
                options.SubstituteApiVersionInUrl = true;
            });

            builder.Services.AddEndpointsApiExplorer();

            // Documentação Swagger
            builder.Services.AddSwaggerGen(options => {
                // Obtém o provedor de descrição de versão da API para iterar sobre todas as versões
                var apiVersionProvider = builder.Services.BuildServiceProvider()
                .GetRequiredService<IApiVersionDescriptionProvider>();

                // Cria um documento Swagger para cada versão da API
                foreach (var description in apiVersionProvider.ApiVersionDescriptions)
                {
                    options.SwaggerDoc(description.GroupName, new OpenApiInfo
                    {
                        Title = $"API Gerenciador de Tarefas - Sprint 1 ({description.ApiVersion})",
                        Version = description.ApiVersion.ToString(),
                        Description = "API RESTful para gerenciamento de projetos e tarefas, desenvolvida como parte de um desafio técnico. Permite criar, visualizar, atualizar e excluir tarefas e projetos, além de registrar comentários e histórico de alterações conforme regras de negócio."
                    });
                }
            });

            var app = builder.Build();

            // Resolve o provedor de versão da API para uso no Swagger
            var apiVersionProvider = app.Services.GetRequiredService<IApiVersionDescriptionProvider>();

            // Página de exceção detalhada em ambiente de desenvolvimento
            if (app.Environment.IsDevelopment())
                app.UseDeveloperExceptionPage();
            else
                app.UseExceptionHandler("/error"); // Página genérica de erro em produção

            // Pipeline de execução
            app.UseRouting();
            app.UseResponseCompression();
            app.UseHttpsRedirection();
            app.UseAuthorization();

            // Swagger UI configurado por versão
            app.UseSwagger();
            app.UseSwaggerUI(options =>
            {
                foreach (var description in apiVersionProvider.ApiVersionDescriptions)
                    options.SwaggerEndpoint($"/swagger/{description.GroupName}/swagger.json", description.GroupName.ToUpperInvariant());
            });

            app.MapControllers();

            // Inicia a aplicação
            app.Run();
        }
    }
}
