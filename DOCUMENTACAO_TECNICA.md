# 📋 Relatório Técnico e Documentação de Arquitetura — Trabalho T2

**Sistema de Gestão de Tarefas Colaborativas — TaskManager API**  
**Disciplina:** Engenharia de Software — Arquitetura e Padrões (UNISINOS)  
**Grupo:** Athos Kölling, Luiz Eduardo, Murilo Teribele  

---

## 1. Visão Geral do Projeto

Nós desenvolvemos o **TaskManager API** como uma plataforma para a gestão de tarefas colaborativas dentro de equipes de desenvolvimento. O objetivo principal do nosso sistema é permitir que os membros de uma organização criem, editem, atribuam e monitorem tarefas em um ambiente seguro, estimulando a cooperação por meio de comentários internos e facilitando o controle de prazos com integrações externas.

Este projeto foi construído seguindo diretrizes rigorosas de engenharia de software, priorizando a testabilidade e o desacoplamento de dependências através da implementação da **Clean Architecture** e dos **Princípios SOLID**.

---

## 2. Requisitos do Sistema

Nosso sistema foi desenvolvido para cobrir integralmente os requisitos funcionais descritos na especificação do trabalho, além de implementar 3 requisitos complementares de nossa escolha para enriquecer o projeto.

### 2.1 Requisitos Funcionais (RF)
*   **RF01 - Gerenciamento de Usuários:** Cadastro de novos usuários com verificação de e-mail exclusivo, consulta a dados cadastrais seguros (sem senha exposta), atualização de informações cadastrais e remoção lógica do sistema.
*   **RF02 - Gerenciamento de Tarefas:** CRUD completo de tarefas associando um criador (`CreatedBy`) e um responsável (`AssignedTo`), com suporte a atualização de estado (Pendente, Em Andamento, Em Revisão, Concluído) e nível de prioridade (Baixo, Médio, Alto).
*   **RF03 - Autenticação e Segurança:** Autenticação stateless via JSON Web Tokens (JWT) e mecanismo de revogação/renovação de sessões via Refresh Tokens persistidos no banco de dados.

### 2.2 Requisitos Complementares Escolhidos e Implementados
Conforme o limite de escopo exigido na especificação, nós selecionamos e implementamos os seguintes **3 requisitos complementares**:
1.  **Integração com Calendários (Google Calendar):** Sempre que uma tarefa com data de vencimento (`DueDate`) é criada ou atualizada, a API efetua uma chamada integrada na API do Google Calendar do responsável para agendar o evento na sua agenda física de forma automática.
2.  **Sistema de Permissões com Papéis (RBAC - Role-Based Access Control):** Diferenciação de papéis no sistema (`Admin` e `User`). Usuários comuns só podem alterar suas próprias tarefas e dados, enquanto administradores possuem controle global sobre a remoção de itens e promoção de usuários.
3.  **Comentários em Tarefas (Sub-recursos aninhados):** Rota `/api/tasks/{id}/comments` com controle de acesso para auditoria. Usuários comuns só podem apagar seus próprios comentários, e administradores podem apagar qualquer comentário.

### 2.3 Requisitos Não Funcionais (RNF)
*   **RNF01 - Arquitetura de Software:** Utilização do padrão **Clean Architecture** para desacoplamento de camadas.
*   **RNF02 - Persistência:** Banco de dados relacional **PostgreSQL 15** em containers e suporte a **EF Core In-Memory** como banco de desenvolvimento rápido (zero-config).
*   **RNF03 - Testabilidade:** Cobertura de testes unitários superior a 60% nas regras de negócio críticas.
*   **RNF04 - Robustez e Tratamento de Erros:** Interceptador global de exceções via Middleware HTTP para segurança e clareza de logs de erro.

---

## 3. Decisões Arquiteturais e Justificativa

Nós optamos pela utilização da **Clean Architecture** devido aos seguintes fatores:
1.  **Independência de Frameworks:** O framework Web API (ASP.NET Core) é apenas um detalhe técnico. Se no futuro decidirmos migrar para Console Application ou outro framework, as regras de negócios ficam intocadas.
2.  **Independência de Banco de Dados:** O domínio define interfaces (ex: `IUserRepository`). A persistência concreta (EF Core, PostgreSQL ou In-Memory) reside na camada de infraestrutura.
3.  **Testabilidade:** Toda a lógica contida em `Application` e `Domain` pode ser testada de forma isolada com mocks, sem precisar inicializar servidores HTTP ou conexões físicas com o banco.

### 3.1 Detalhamento das Camadas
*   **`TaskManager.Domain` (C# Puro):** Entidades com estado próprio, Enums globais e interfaces de Repositório. É o núcleo inviolável do nosso projeto.
*   **`TaskManager.Application`:** Orquestração de casos de uso. Contém os nossos serviços de negócio, DTOs e validadores de negócio (`FluentValidation`).
*   **`TaskManager.Infrastructure`:** DbContext (`AppDbContext`), configurações de relacionamento (Fluent API), migrações e integrações externas (como chamadas à API do Google via SDK oficial).
*   **`TaskManager.API`:** Controladores REST, middlewares globais de erros, e a inicialização técnica do pipeline HTTP.

### 3.2 Mapeamento de Princípios SOLID Aplicados
*   **SRP (Single Responsibility Principle):** Nosso `ExceptionHandlingMiddleware` é encarregado apenas de traduzir exceções em respostas JSON limpas. Os controllers apenas repassam DTOs e retornam códigos de status. A validação de formato reside nos validadores isolados.
*   **OCP (Open/Closed Principle):** O uso da interface `ICalendarService` na nossa aplicação permite anexar múltiplos provedores de calendário (Google, Outlook, Apple) no futuro. Estendemos o software criando novas implementações da interface, sem modificar a classe `TaskService`.
*   **LSP (Liskov Substitution Principle):** A substituição do banco PostgreSQL pelo provedor em memória (`InMemory`) funciona perfeitamente sem quebrar as interfaces ou a lógica interna de consultas da API.
*   **ISP (Interface Segregation Principle):** Em vez de interfaces genéricas monolíticas, cada repositório é restrito às operações específicas de sua respectiva entidade (ex: `IRefreshTokenRepository` possui apenas operações de persistência e expiração do token).
*   **DIP (Dependency Inversion Principle):** A nossa camada de aplicação não referencia a camada de infraestrutura. Ela referencia apenas as interfaces do domínio. A infraestrutura implementa essas interfaces, e a injeção é resolvida no `Program.cs`.

---

## 4. Modelagem de Dados

Nosso banco de dados é estruturado de forma a persistir informações cadastrais, de tarefas com seus prazos e comentários vinculados, além da auditoria de login.

### 4.1 Diagrama Entidade-Relacionamento (ERD)

```mermaid
erDiagram
    USER ||--o{ TASK_ITEM : "cria (CreatedTasks)"
    USER ||--o{ TASK_ITEM : "e atribuído (AssignedTasks)"
    USER ||--o{ COMMENT : "escreve"
    USER ||--o{ REFRESH_TOKEN : "possui"
    TASK_ITEM ||--o{ COMMENT : "contém"

    USER {
        guid Id PK
        string Name
        string Email UK
        string PasswordHash
        int Role
        datetime CreatedAt
        datetime DeletedAt
        string GoogleAccessToken
        string GoogleRefreshToken
    }

    TASK_ITEM {
        guid Id PK
        string Title
        string Description
        int State
        int Priority
        datetime CreatedAt
        datetime DueDate
        datetime DeletedAt
        guid CreatedById FK
        guid AssignedToId FK
    }

    COMMENT {
        guid Id PK
        string Content
        datetime CreatedAt
        guid TaskItemId FK
        guid UserId FK
    }

    REFRESH_TOKEN {
        guid Id PK
        string Token
        datetime ExpiresAt
        datetime CreatedAt
        datetime RevokedAt
        guid UserId FK
    }
```

### 4.2 Dicionário de Tabelas

#### Tabela `Users` (Usuários)
Guarda os dados cadastrais e as credenciais de sincronização OAuth da API do Google Calendar.
*   `Id` (UUID, Chave Primária): Identificador único do usuário.
*   `Name` (VARCHAR(150), Not Null): Nome completo.
*   `Email` (VARCHAR(100), Not Null, Unique Index): E-mail de cadastro.
*   `PasswordHash` (VARCHAR(250), Not Null): Senha criptografada com algoritmo BCrypt.
*   `Role` (INT, Not Null): Nível de permissão (0: Guest, 1: User, 2: Admin).
*   `CreatedAt` (TIMESTAMP, Not Null): Data de criação da conta.
*   `DeletedAt` (TIMESTAMP, Null): Timestamp para exclusão lógica (**Soft Delete**).
*   `GoogleAccessToken` (TEXT, Null): Token de acesso ativo para a conta Google.
*   `GoogleRefreshToken` (TEXT, Null): Token de renovação do Google para contornar expirações de token sem intervenção do usuário.

#### Tabela `TaskItems` (Tarefas)
Guarda os dados das tarefas com chaves estrangeiras vinculadas à tabela `Users`.
*   `Id` (UUID, Chave Primária): Identificador único.
*   `Title` (VARCHAR(100), Not Null): Título descritivo.
*   `Description` (TEXT, Not Null): Descrição da tarefa.
*   `State` (INT, Not Null): Estado da tarefa (0: Pending, 1: InProgress, 2: InReview, 3: Completed).
*   `Priority` (INT, Not Null): Prioridade (0: Low, 1: Medium, 2: High).
*   `CreatedAt` (TIMESTAMP, Not Null): Data de criação.
*   `DueDate` (TIMESTAMP, Null): Data limite de entrega (se informada, agenda no Google Calendar).
*   `DeletedAt` (TIMESTAMP, Null): Campo de exclusão lógica (**Soft Delete**).
*   `CreatedById` (UUID, FK -> Users, Restrict): ID do usuário criador da tarefa.
*   `AssignedToId` (UUID, FK -> Users, Restrict): ID do usuário encarregado de executar a tarefa.

#### Tabela `Comments` (Comentários)
Guarda comentários associados às tarefas de forma aninhada.
*   `Id` (UUID, Chave Primária): Identificador único.
*   `Content` (TEXT, Not Null): Conteúdo do comentário.
*   `CreatedAt` (TIMESTAMP, Not Null): Data de inserção do comentário.
*   `TaskItemId` (UUID, FK -> TaskItems, Cascade): ID da tarefa que hospeda o comentário.
*   `UserId` (UUID, FK -> Users, Cascade): ID do autor do comentário.

#### Tabela `RefreshTokens` (Sessões)
Guarda tokens de renovação persistidos no login para segurança do fluxo JWT.
*   `Id` (UUID, Chave Primária): Identificador único.
*   `Token` (VARCHAR(150), Not Null): String hash gerada para renovação da sessão.
*   `ExpiresAt` (TIMESTAMP, Not Null): Data de expiração do Refresh Token.
*   `CreatedAt` (TIMESTAMP, Not Null): Data de emissão.
*   `RevokedAt` (TIMESTAMP, Null): Data de cancelamento manual do token (logout).
*   `UserId` (UUID, FK -> Users, Cascade): ID do usuário proprietário da sessão.

### 4.3 Mecanismos de Integridade e Soft Delete
*   **Filtros Globais de Consulta:** No nosso arquivo `AppDbContext.cs`, as consultas para `User` e `TaskItem` são filtradas por padrão com `DeletedAt == null`. Consultas do tipo `_context.Users.ToList()` retornam apenas usuários ativos, sem que tenhamos que incluir cláusulas `Where` manualmente.
*   **Deleção sem Cascata:** Configurado via Fluent API para evitar perda acidental de dados de auditoria:
    ```csharp
    builder.Entity<TaskItem>()
        .HasOne(t => t.CreatedBy)
        .WithMany(u => u.CreatedTasks)
        .HasForeignKey(t => t.CreatedById)
        .OnDelete(DeleteBehavior.Restrict);
    ```
    Isso impede que a exclusão de um usuário remova as tarefas que ele criou, garantindo consistência histórica.

---

## 5. Fluxo de Requisições e Exemplos da API

Abaixo encontram-se exemplos das principais chamadas HTTP para o uso e testes da API.

### 5.1 Registro de Usuário (`POST /api/users`)
*   **Requisição:**
    ```http
    POST /api/users HTTP/1.1
    Content-Type: application/json

    {
      "name": "Athos Kolling",
      "email": "athos@example.com",
      "password": "SenhaForteDeExemplo123!",
      "role": 1
    }
    ```
*   **Resposta (201 Created):**
    ```json
    {
      "id": "e42718aa-2c5e-4bb5-8664-df83981881b2",
      "name": "Athos Kolling",
      "email": "athos@example.com",
      "role": 1,
      "createdAt": "2026-06-17T19:35:00.000Z"
    }
    ```

### 5.2 Login (`POST /api/auth/login`)
*   **Requisição:**
    ```http
    POST /api/auth/login HTTP/1.1
    Content-Type: application/json

    {
      "email": "athos@example.com",
      "password": "SenhaForteDeExemplo123!"
    }
    ```
*   **Resposta (200 OK):**
    ```json
    {
      "accessToken": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.ey...",
      "refreshToken": "6fbf2d1e-bf2d-45db-99e5-9c8846c4f03a",
      "expiresAt": "2026-06-17T20:35:00.000Z"
    }
    ```

### 5.3 Criação de Tarefa com Prazo (`POST /api/tasks`)
*   **Requisição:** (Exige cabeçalho `Authorization: Bearer <accessToken>`)
    ```http
    POST /api/tasks HTTP/1.1
    Authorization: Bearer eyJhbGciOiJIUzI1NiIsIn...
    Content-Type: application/json

    {
      "title": "Configurar Google OAuth",
      "description": "Criar credenciais no console de desenvolvedor do Google.",
      "priority": 1,
      "dueDate": "2026-06-25T15:00:00Z",
      "assignedToId": "e42718aa-2c5e-4bb5-8664-df83981881b2"
    }
    ```
*   **Resposta (201 Created):**
    ```json
    {
      "id": "3bb60f08-7a5d-4f11-8255-a0c441c2c2f6",
      "title": "Configurar Google OAuth",
      "description": "Criar credenciais no console de desenvolvedor do Google.",
      "state": 0,
      "priority": 1,
      "dueDate": "2026-06-25T15:00:00Z",
      "createdById": "e42718aa-2c5e-4bb5-8664-df83981881b2",
      "assignedToId": "e42718aa-2c5e-4bb5-8664-df83981881b2",
      "createdAt": "2026-06-17T19:38:12.000Z"
    }
    ```

---

## 6. Configuração, Deploy e Ambientes de Execução

Nós projetamos a API para ter máxima flexibilidade de implantação, aceitando execução em containers isolados ou execução em modo de desenvolvimento rápido com banco em memória.

### 6.1 Execução no Docker (Ambiente Isolado)
Toda a nossa pilha de produção é gerida pelo Docker. 
1.  **Variáveis de Ambiente (.env):** O arquivo `.env` deve ser colocado no diretório `API_Clean_Architecture/` contendo as credenciais de segurança do Google Calendar:
    ```env
    GOOGLE_CLIENT_ID=seu_client_id.apps.googleusercontent.com
    GOOGLE_CLIENT_SECRET=sua_chave_secreta
    ```
2.  **Inicialização:** O comando `docker compose up -d --build` constrói a imagem da Web API e inicializa o PostgreSQL 15 na rede interna do Docker.
    *   A API expõe a porta **`5000`** para o host (`http://localhost:5000/swagger`).
    *   O banco é exposto na porta **`5433`** do host para conexões locais de banco de dados.

### 6.2 Execução Local Rápida (Banco In-Memory)
Para testes locais rápidos sem Docker ou dependências locais:
1.  **Parâmetro de Ativação:** No nosso arquivo `appsettings.Development.json`, a flag `"UseInMemoryDatabase"` está configurada como `"true"`.
2.  **Mecanismo de Ação:** O `Program.cs` desvia o registro do banco de dados para o provedor `InMemoryDatabase` do Entity Framework. A inicialização não executa as Migrations físicas e executa o `EnsureCreated()`, criando todo o esquema de tabelas na RAM instantaneamente de forma automática.
3.  **Portas de Execução:** O servidor do Kestrel rodará por padrão no endereço:
    👉 **`http://localhost:5261/swagger`**

---

## 7. Testes Automatizados

Garantir a qualidade do software é uma das nossas principais preocupações. Nós desenvolvemos uma suíte de testes utilizando frameworks consagrados de mercado.

### 7.1 Stack de Testes
*   **xUnit:** Nosso executor de testes assíncronos e asserções matemáticas/lógicas.
*   **Moq:** Biblioteca para simular o comportamento de chamadas de persistência e serviços externos, garantindo o total isolamento do código dos testes.

### 7.2 Casos de Testes Cobertos (7 cenários de negócios)
*   **`AuthServiceTests` (Camada de Autenticação):**
    *   `LoginAsync_WithValidCredentials_ShouldReturnTokensAndPersistRefreshToken`: Valida se o fornecimento de credenciais válidas gera o par JWT + Refresh Token correto e invoca o repositório de persistência do token de renovação.
*   **`TaskServiceTests` (Camada de Negócios de Tarefas):**
    *   `CreateAsync_ShouldCreateTaskAndTriggerCalendarEvent`: Verifica se ao criar uma tarefa, o serviço dispara o fluxo de criação de evento integrado no `GoogleCalendarService` enviando as informações de título, descrição e prazo corretos.
*   **`CommentServiceTests` (Segurança e Validação de Papéis nos Comentários):**
    *   `DeleteAsync_OwnerRequesting_ShouldDeleteSuccessfully`: Testa se o autor original do comentário consegue removê-lo.
    *   `DeleteAsync_AdminRequesting_ShouldDeleteSuccessfully`: Testa se um usuário com papel de `Admin` consegue apagar comentários criados por outros membros.
    *   `DeleteAsync_OtherUserRequesting_ShouldThrowUnauthorizedAccessException`: Testa a segurança do sistema garantindo que uma tentativa de remoção por um usuário comum sem posse do comentário dispare uma exceção de segurança, bloqueando a operação e protegendo os dados.
*   **`UserServiceTests` (Lógica de Usuários):**
    *   `CreateAsync_WithUniqueEmail_ShouldCreateUserAndHashPassword`: Valida o cadastro básico de um usuário, verificando se a senha foi convertida em hash seguro e nunca guardada em texto puro.
    *   `CreateAsync_WithDuplicateEmail_ShouldThrowInvalidOperationException`: Valida se a tentativa de cadastro de um e-mail já existente é barrada e levanta um erro de operação de negócio.
