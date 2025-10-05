# Sistema de Gerenciamento de Tarefas - API

Esta é a API RESTful para o sistema de gerenciamento de tarefas, desenvolvida como parte de um desafio técnico. A API permite o gerenciamento de projetos e tarefas, seguindo um conjunto de regras de negócio específicas.

## ✨ Tecnologias e Arquitetura

Este projeto foi desenvolvido utilizando um stack moderno e práticas de alta qualidade para garantir testabilidade, manutenibilidade e escalabilidade.

- **Framework:** .NET 8
- **Arquitetura:** Clean Architecture
- **Banco de Dados:** PostgreSQL (orquestrado com Docker)
- **ORM:** Entity Framework Core 8
- **Testes:** xUnit e Moq
- **Containerização:** Docker & Docker Compose

## 🚀 Como Executar o Projeto

É necessário ter o **Docker** e o **Docker Compose** instalados para executar este projeto.

1.  Clone o repositório:
    ```bash
    git clone <URL_DO_SEU_REPOSITORIO>
    cd TaskManagement.Api
    ```

2.  Execute o Docker Compose a partir da raiz do projeto (onde o arquivo `docker-compose.yml` está localizado):
    ```bash
    docker-compose up --build
    ```

3.  Aguarde os containers serem construídos e iniciados. A API estará disponível nos seguintes endereços:
    - `http://localhost:8080`
    - `https://localhost:8081`

A primeira vez que a API for iniciada, as migrations do Entity Framework serão aplicadas automaticamente para criar o banco de dados e as tabelas.

## 📝 Endpoints da API

*(Aqui você listaria os endpoints, por exemplo:)*

| Verbo  | Rota                         | Descrição                                 |
| :----- | :--------------------------- | :---------------------------------------- |
| `GET`  | `/api/users/{userId}/projects` | Lista todos os projetos de um usuário.    |
| `POST` | `/api/projects`              | Cria um novo projeto.                     |
| `GET`  | `/api/projects/{projectId}/tasks` | Lista todas as tarefas de um projeto. |
| ...    | ...                          | ...                                       |


---

## 🧐 Fase 2: Perguntas para o Product Owner (PO)

Visando o refinamento do produto e futuras implementações, eu levantaria as seguintes questões com o PO:

1.  **Gestão de Usuários e Permissões:** Como o "usuário" e a role de "gerente" serão definidos e gerenciados pelo serviço externo? Precisamos de um ID de usuário único (UUID, e-mail)? A role virá em um token JWT?
2.  **Colaboração:** A visão é que um projeto pertença a um único usuário ou múltiplos usuários poderão colaborar em um mesmo projeto? Se sim, quais seriam os níveis de permissão (leitura, escrita)?
3.  **Notificações:** Devemos notificar os usuários sobre eventos importantes, como a data de vencimento de uma tarefa se aproximando ou quando um comentário é adicionado? Se sim, por qual canal (e-mail, push notification, etc.)?
4.  **Priorização dos Relatórios:** Qual é a principal dor que o relatório de "média de tarefas concluídas" busca resolver? Existem outros KPIs (Key Performance Indicators) de desempenho que seriam mais valiosos para os gerentes neste momento?
5.  **Critérios de Aceitação para "Concluída":** Apenas mudar o status para "Concluída" é suficiente, ou haverá um fluxo de aprovação onde um gerente precisa validar a conclusão da tarefa?
6.  **Tratamento de Erros no Frontend:** Qual é a experiência de usuário esperada quando uma regra de negócio é violada (ex: limite de tarefas atingido)? Devemos retornar mensagens de erro genéricas ou códigos específicos que o frontend possa interpretar para exibir mensagens amigáveis?

---

## 🛠️ Fase 3: Possíveis Melhorias e Visão de Futuro

Pensando na evolução do projeto, proponho as seguintes melhorias técnicas e arquiteturais:

1.  **Arquitetura e Padrões:**
    * **Implementar CQRS com MediatR:** Separar os modelos de leitura (Queries) dos de escrita (Commands) pode otimizar e organizar melhor o código, especialmente em cenários complexos.
    * **Validação de Requisições com FluentValidation:** Mover a validação dos DTOs para uma camada dedicada usando FluentValidation, mantendo os controllers mais limpos e a lógica de validação centralizada e testável.
    * **Mapeamento com AutoMapper:** Automatizar o mapeamento entre Entidades e DTOs para reduzir código boilerplate e suscetível a erros.

2.  **Observabilidade e Resiliência:**
    * **Logging Estruturado com Serilog:** Implementar logging estruturado para facilitar a busca e análise de logs em ambientes como a nuvem.
    * **Health Checks:** Adicionar endpoints de Health Check para monitorar a saúde da API e suas dependências (como o banco de dados), essencial para ambientes orquestrados como Kubernetes.
    * **Polly para Resiliência:** Utilizar a biblioteca Polly para implementar políticas de resiliência, como *retries* e *circuit breakers*, em chamadas a serviços externos.

3.  **Visão de Cloud/DevOps:**
    * **Pipeline de CI/CD:** Criar um pipeline automatizado (usando GitHub Actions, Azure DevOps, etc.) que, a cada commit na branch principal, execute os testes, construa a imagem Docker e a publique em um registro de contêineres (como Docker Hub ou Azure Container Registry).
    * **Estratégia de Deploy:** A arquitetura containerizada permite um deploy fácil em serviços como Azure App Service, Azure Kubernetes Service (AKS) ou AWS ECS, garantindo escalabilidade e alta disponibilidade.
    * **Gerenciamento de Configuração:** Mover segredos, como connection strings, do `appsettings.json` para um serviço de gerenciamento de segredos (como Azure Key Vault ou AWS Secrets Manager) para aumentar a segurança.

4.  **Testes:**
    * **Testes de Integração:** Adicionar uma camada de testes de integração que utilize um banco de dados de teste (em memória ou um container Docker) para validar o fluxo completo da aplicação, desde o controller até o banco de dados.
