# 📌 Sistema de Gerenciamento de Tarefas – API

[![Status da Build](https://img.shields.io/badge/build-passing-brightgreen)](https://github.com/avbctr/TaskManagement.Api/actions)
[![Cobertura de Código](https://img.shields.io/badge/coverage-80%2B-blue)](https://github.com/avbctr/TaskManagement.Api)
[![Licença](https://img.shields.io/badge/license-MIT-lightgrey)](https://github.com/avbctr/TaskManagement.Api/blob/main/LICENSE)

Esta é uma API RESTful desenvolvida como parte de um desafio técnico, com foco em produtividade, organização e colaboração. A aplicação permite que usuários gerenciem projetos e tarefas, respeitando regras de negócio reais e pensadas para equipes ágeis.

---

## 📖 Índice

- [✨ Tecnologias e Arquitetura](#-tecnologias-e-arquitetura)
- [📂 Visão Geral da Estrutura do Projeto](#-visão-geral-da-estrutura-do-projeto)
- [🚀 Como Executar o Projeto](#-como-executar-o-projeto)
- [⚙️ Detalhes de Configuração](#️-detalhes-de-configuração)
- [🧪 Testes e Cobertura de Código](#-testes-e-cobertura-de-código)
- [📝 Endpoints da API](#-endpoints-da-api)
- [🧐 Fase 2 – Perguntas para o Product Owner (PO)](#-fase-2--perguntas-para-o-product-owner-po)
- [🛠️ Fase 3 – Melhorias e Visão de Futuro](#️-fase-3--melhorias-e-visão-de-futuro)
- [👨‍💻 Sobre o Autor](#-sobre-o-autor)

---

## ✨ Tecnologias e Arquitetura

O projeto foi construído com base em decisões arquiteturais sólidas e tecnologias modernas, visando escalabilidade, testabilidade e clareza de código.

- **Framework:** .NET 8
- **Arquitetura:** Clean Architecture
- **Banco de Dados:** PostgreSQL (via Docker)
- **ORM:** Entity Framework Core 8
- **Testes:** xUnit + Moq (com cobertura superior a 80%)
- **Containerização:** Docker & Docker Compose
- **Documentação da API:** Swagger (com comentários XML detalhados)

---

## 📂 Visão Geral da Estrutura do Projeto

A solução está organizada seguindo os princípios da **Clean Architecture**, promovendo a separação de responsabilidades e a manutenibilidade.

- **`TaskManagement.Domain`**: Contém as entidades de negócio principais, enums e view models. Este é o coração da aplicação e não possui dependências externas.
- **`TaskManagement.Application`**: Contém a lógica da aplicação, incluindo serviços, interfaces (repositórios, serviços) e objetos de transferência de dados (payloads, responses). Orquestra a camada de domínio para realizar operações de negócio.
- **`TaskManagement.Infrastructure`**: Implementa as preocupações externas, como o acesso a dados (Entity Framework DbContext, repositórios) e integrações com outros serviços. Depende da camada de Aplicação.
- **`TaskManagement.Api`**: A camada de apresentação, que expõe as funcionalidades da aplicação através de uma API RESTful. Lida com requisições HTTP, validação e serialização, dependendo da camada de Aplicação para executar as tarefas.
- **`TaskManagement.Application.UnitTests`**: Contém testes unitários para a camada de Aplicação, garantindo que a lógica de negócio está correta e confiável.

---

## 🚀 Como Executar o Projeto

### Pré-requisitos

- Docker
- Docker Compose

### Executando a Aplicação

1.  **Clone o repositório:**
    ```bash
    git clone https://github.com/avbctr/TaskManagement.Api.git
    ```

2.  **Navegue até o diretório do projeto:**
    ```bash
    cd TaskManagement.Api
    ```

3.  **Construa e execute os containers:**
    ```bash
    docker-compose up --build
    ```

A API estará disponível em:
- **HTTP:** `http://localhost:8080`
- **HTTPS:** `https://localhost:8081`
- **Swagger UI:** `http://localhost:8080/swagger`

*Observação: As migrations do Entity Framework são aplicadas automaticamente na primeira execução.*

---

## ⚙️ Detalhes de Configuração

A configuração da aplicação é gerenciada através do arquivo `appsettings.json` e arquivos específicos de ambiente, como `appsettings.Development.json`.

- **`ConnectionStrings`**: A string `DefaultConnection` é usada pelo Entity Framework para se conectar ao banco de dados PostgreSQL. Ao usar o `docker-compose`, esta já vem pré-configurada para se conectar ao serviço `postgres`.
- **`Logging`**: Configura os níveis de log para diferentes partes da aplicação.
- **`Kestrel`**: Define os endpoints (portas HTTP/HTTPS) para o servidor web.

Para desenvolvimento local fora do Docker, pode ser necessário ajustar a string `DefaultConnection` para apontar para sua instância local do PostgreSQL.

---

## 🧪 Testes e Cobertura de Código

- **Testes Unitários:** Desenvolvidos com xUnit and Moq.
- **Cobertura de Código:** Superior a 80%.
- **Relatórios:** Gerados via Coverlet + ReportGenerator.
- **Foco:** Regras de negócio, fluxos positivos e negativos.

---

## 📝 Endpoints da API

As rotas documentadas foram verificadas com base no código-fonte.

### Projetos (`v1/Projetos`)

| Método | Endpoint                        | Descrição                               |
| :----- | :------------------------------ | :---------------------------------------- |
| `GET`    | `/{id}`                         | Obtém um projeto pelo seu ID.             |
| `GET`    | `/usuario/{userId}`             | Obtém todos os projetos de um usuário.    |
| `POST`   | `/`                             | Cria um novo projeto.                     |
| `PUT`    | `/`                             | Atualiza um projeto existente.            |
| `DELETE` | `/{id}`                         | Deleta um projeto pelo seu ID.            |

### Tarefas (`v1/Tarefas`)

| Método | Endpoint                        | Descrição                               |
| :----- | :------------------------------ | :---------------------------------------- |
| `GET`    | `/{id}`                         | Obtém uma tarefa pelo seu ID.             |
| `POST`   | `/`                             | Cria uma nova tarefa.                     |
| `PUT`    | `/`                             | Atualiza uma tarefa existente.            |
| `DELETE` | `/{id}`                         | Deleta uma tarefa pelo seu ID.            |
| `POST`   | `/comentario`                   | Adiciona um comentário a uma tarefa.      |
| `DELETE` | `/comentario/{comentarioId}`    | Deleta um comentário pelo seu ID.         |
| `GET`    | `/relatorio-desempenho`         | Obtém um relatório de desempenho (perfil gerente). |

---

## 🧐 Fase 2 – Perguntas para o Product Owner (PO)

Para garantir que o produto evolua com clareza e propósito, aqui estão as perguntas que eu faria ao PO:

### 🔐 Autenticação e Permissões
- Como os perfis de "usuário" e "gerente" serão definidos e validados? O perfil virá via JWT?
- Precisamos de integração com um serviço de identidade externo?

### 👥 Colaboração
- Projetos serão sempre individuais ou poderão ser compartilhados entre usuários?
- Haverá permissões granulares (leitura, escrita, administração)?

### 🔔 Notificações
- Devemos alertar usuários sobre tarefas vencendo ou novos comentários?
- Qual o canal preferido: e-mail, push, integração com Slack?

### 📊 Relatórios
- O relatório de tarefas concluídas atende a qual dor real?
- Há outros KPIs que os gerentes gostariam de acompanhar (ex: tempo médio de conclusão, tarefas atrasadas)?

### ✅ Fluxo de Conclusão
- Mudar o status para "Concluída" é suficiente ou precisa de validação por um gerente?

### ⚠️ UX de Erros
- Como o frontend deve reagir a erros de negócio? Mensagens genéricas ou códigos específicos para UX personalizada?

---

## 🛠️ Fase 3 – Melhorias e Visão de Futuro

Pensando na evolução do projeto, proponho as seguintes melhorias técnicas e arquiteturais:

### 🧱 Arquitetura
- **CQRS com MediatR:** Separar os modelos de leitura (Queries) dos de escrita (Commands) melhora a organização do código, facilita testes e permite escalar partes da aplicação de forma independente.
- **FluentValidation:** Centraliza a validação dos DTOs, deixando os controllers mais limpos e permitindo testes unitários específicos para regras de entrada.
- **AutoMapper:** Reduz o código repetitivo de mapeamento entre entidades e view models, diminuindo erros e aumentando a produtividade.

### 🔍 Observabilidade e Resiliência
- **Serilog:** Logging estruturado facilita a análise de logs em ambientes distribuídos e melhora a rastreabilidade de erros.
- **Health Checks:** Permite monitorar a saúde da API e suas dependências, essencial para ambientes orquestrados como Kubernetes.
- **Polly:** Implementa políticas de resiliência como retries e circuit breakers, protegendo a aplicação contra falhas temporárias de serviços externos.
- **Rate Limiting:** Controla o número de requisições por usuário ou IP, protegendo a API contra abusos e garantindo estabilidade em cenários de alta carga.

### ☁️ Cloud & DevOps
- **Pipeline de CI/CD:** Automatiza testes, build e deploy, garantindo entregas contínuas e seguras com cada commit.
- **Deploy em AKS ou AWS ECS:** Permite escalar horizontalmente com alta disponibilidade e gerenciamento simplificado.
- **Gerenciamento de Segredos:** Move credenciais sensíveis para serviços como Azure Key Vault ou AWS Secrets Manager, aumentando a segurança da aplicação.

### 🧪 Testes
- **Testes de Integração:** Validam o fluxo completo da aplicação, garantindo que os componentes funcionem bem juntos.
- **Testes de Contrato:** Asseguram que a comunicação entre frontend e backend esteja sempre alinhada.
- **Testes de Carga:** Avaliam o desempenho da API sob diferentes níveis de uso, antecipando gargalos e otimizando recursos.

---

## 👨‍💻 Sobre o Autor

**Anderson Costa ∴**

*Analista de Sistemas/Especialista .NET/C#*

[![LinkedIn](https://img.shields.io/badge/LinkedIn-0077B5?style=flat&logo=linkedin&logoColor=white)](https://br.linkedin.com/in/andersonvbcosta)
[![Portfolio](https://img.shields.io/badge/Portfolio-4B0082?style=flat&logo=react&logoColor=white)](https://avbc.dev)
[![GitHub](https://img.shields.io/badge/GitHub-181717?style=flat&logo=github&logoColor=white)](https://github.com/avbctr)

![.NET](https://img.shields.io/badge/.NET-512BD4?style=flat&logo=dotnet&logoColor=white)
![C#](https://img.shields.io/badge/C%23-239120?style=flat&logo=csharp&logoColor=white)
![Azure](https://img.shields.io/badge/Azure-0078D4?style=flat&logo=microsoftazure&logoColor=white)
