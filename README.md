# TaskManager API — Sistema de Gestão de Tarefas Colaborativas

Trabalho Acadêmico T2 — Disciplina de Engenharia de Software: Arquitetura e Padrões (UNISINOS)  
**Professor:** Guilherme Silva de Lacerda  
**Equipe:** Athos Kölling, Luiz Eduardo, Murilo Teribele  

---

## 1. Visão Geral do Sistema

O **TaskManager API** é uma solução robusta e escalável para a gestão colaborativa de tarefas em equipes. O sistema foi projetado para permitir que os membros de uma organização criem, editem, atribuam, comentem e concluam tarefas de forma dinâmica, sob um modelo seguro de controle de acesso baseado em papéis (RBAC).

### Objetivos do Sistema:
*   **Gestão de Tarefas:** Criação, atribuição, atualização de status/prioridade e exclusão de tarefas.
*   **Colaboração em Tempo Real:** Inserção de comentários em tarefas específicas para comunicação direta entre os membros.
*   **Sincronização de Calendário:** Integração com a API do Google Calendar para agendar automaticamente no calendário do usuário responsável as tarefas que possuem prazos de entrega (`DueDate`).
*   **Segurança e Auditoria:** Controle de acesso baseado em papéis (`Admin`, `User`, `Guest`), autenticação via JSON Web Tokens (JWT) com persistência real de `Refresh Tokens` para renovação de sessões e política de exclusão lógica (**Soft Delete**) para manter integridade referencial histórica.

---

## 2. Decisões Arquiteturais e Padrões Aplicados

A API foi desenvolvida em **.NET 8** utilizando o padrão **Clean Architecture (Arquitetura Limpa)** para garantir o desacoplamento de código, testabilidade, manutenibilidade e flexibilidade tecnológica.

### Estrutura de Camadas (Clean Architecture)

```mermaid
graph TD
    Client[HTTP Client / Swagger] <-->|JSON Requests / JWT| API[API Layer: Controllers & Middlewares]
    API <-->|DTOs| Application[Application Layer: Services & Validators]
    Application <-->|Interfaces & Entities| Domain[Domain Layer: Entities, Enums & Interfaces]
    Infrastructure[Infrastructure Layer: EF Core, Repositories & External Services] <-->|Implementations| Domain
    Infrastructure <-->|EF Core / Npgsql| Database[(PostgreSQL Database)]
    Infrastructure <-->|Google APIs| GoogleCalendar[Google Calendar API]
```

1.  **Domain (Núcleo):** Contém as entidades de negócio (`User`, `TaskItem`, `Comment`, `RefreshToken`), Enums e as interfaces de repositório (`IUserRepository`, etc.). Não possui dependências externas.
2.  **Application (Casos de Uso):** Contém os serviços de aplicação (`UserService`, `TaskService`, `CommentService`, `AuthService`), DTOs e validadores de entrada (`FluentValidation`). Depende apenas da camada de *Domain*.
3.  **Infrastructure (Detalhes Técnicos/Mecanismos):** Implementa o acesso a banco de dados utilizando **Entity Framework Core** com PostgreSQL, repositórios concretos e serviços de infraestrutura externa (como `GoogleCalendarService`). Depende apenas da camada de *Domain*.
4.  **API (Ponto de Entrada):** Controladores ASP.NET Core, middlewares de exceções e configurações de injeção de dependência (`Program.cs`). Depende de *Application* e *Infrastructure*.

### Princípios SOLID Aplicados
*   **Single Responsibility Principle (SRP):** Cada classe possui uma responsabilidade única. Por exemplo, a validação de DTOs é isolada nos validadores do FluentValidation, mantendo os controllers e serviços limpos.
*   **Open/Closed Principle (OCP):** A integração com serviços externos é feita através de interfaces (ex: `ICalendarService`). Caso a equipe decida trocar o Google Calendar pelo Outlook Calendar, basta criar uma nova implementação de `ICalendarService` sem alterar a lógica de negócios do `TaskService`.
*   **Liskov Substitution Principle (LSP):** As implementações concretas dos repositórios e serviços de infraestrutura podem ser substituídas por mocks nos testes unitários sem alterar o comportamento esperado do sistema.
*   **Interface Segregation Principle (ISP):** Foram criadas interfaces de repositório segregadas (`IUserRepository`, `ITaskRepository`, `ICommentRepository`) em vez de uma interface genérica monolítica.
*   **Dependency Inversion Principle (DIP):** As classes de serviços dependem de abstrações (interfaces) e não de implementações concretas. As dependências são injetadas dinamicamente no runtime pelo container nativo do ASP.NET Core.

---

## 3. Modelagem de Dados (Banco de Dados)

O banco de dados utilizado é o **PostgreSQL (v15)**. O relacionamento entre as tabelas foi configurado via Fluent API no Entity Framework Core para garantir chaves estrangeiras restritas e evitar deleções em cascata que corrompam o histórico do sistema.

### Diagrama Entidade-Relacionamento (ERD)

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

### Detalhes de Modelagem:
*   **Soft Delete:** Tanto a tabela `Users` quanto `TaskItems` utilizam a estratégia de Soft Delete. No `AppDbContext.cs`, configuramos filtros globais de consulta (`HasQueryFilter(e => e.DeletedAt == null)`) para ocultar automaticamente registros deletados das consultas SQL geradas pelo EF Core.
*   **Chaves Estrangeiras:** O relacionamento de `TaskItem` com `User` possui duas pontas: `CreatedBy` (Criador) e `AssignedTo` (Responsável). Ambas chaves estrangeiras foram mapeadas explicitamente impedindo a deleção em cascata (`DeleteBehavior.Restrict`).
*   **Índice Único:** O campo `Email` na tabela de `Users` possui restrição de unicidade para evitar cadastros duplicados.

---

## 4. Guia de Endpoints e Fluxo de Requisições

A API está documentada com o padrão OpenAPI/Swagger. A tabela a seguir descreve os principais endpoints da aplicação:

| Recurso | Método HTTP | Rota | Autenticação | Descrição |
| :--- | :---: | :--- | :---: | :--- |
| **Autenticação** | `POST` | `/api/auth/login` | Não | Autentica o usuário e retorna o `AccessToken` (JWT) e o `RefreshToken`. |
| | `POST` | `/api/auth/refresh` | Não | Recebe um Refresh Token válido e gera um novo Access Token. |
| | `POST` | `/api/auth/logout` | Sim | Revoga o Refresh Token ativo do usuário logado. |
| | `GET` | `/api/auth/google/login` | Sim | Gera e retorna a URL de autorização OAuth 2.0 do Google. |
| | `GET` | `/api/auth/google/callback`| Não | Callback do Google que recebe o código de autorização e persiste os tokens na conta do usuário. |
| **Usuários** | `POST` | `/api/users` | Não | Registra um novo usuário no sistema. |
| | `GET` | `/api/users/{id}` | Sim | Busca dados cadastrais seguros de um usuário específico. |
| | `PUT` | `/api/users/{id}` | Sim | Atualiza dados cadastrais do usuário (Nome, E-mail, Senha). |
| | `DELETE` | `/api/users/{id}` | Sim | Remove logicamente o usuário do sistema (Soft Delete). |
| **Tarefas** | `POST` | `/api/tasks` | Sim | Cria uma nova tarefa. Se contiver `DueDate`, agenda no Google Calendar. |
| | `GET` | `/api/tasks` | Sim | Lista tarefas aplicando filtros avançados (Status, Prioridade, Responsável). |
| | `GET` | `/api/tasks/{id}` | Sim | Obtém detalhes completos de uma tarefa específica. |
| | `PUT` | `/api/tasks/{id}` | Sim | Atualiza informações da tarefa (título, descrição, status, responsável). |
| | `DELETE` | `/api/tasks/{id}` | Sim | Deleta logicamente uma tarefa (Soft Delete). |
| **Comentários** | `POST` | `/api/tasks/{id}/comments`| Sim | Adiciona um comentário a uma tarefa específica. |
| | `GET` | `/api/tasks/{id}/comments`| Sim | Obtém todos os comentários de uma tarefa. |
| | `DELETE` | `/api/tasks/{id}/comments/{commentId}`| Sim | Remove um comentário (autorizado apenas para o autor ou Admin). |

### Exemplo de Fluxo: Login e Chamada Protegida
1.  O cliente envia uma requisição `POST /api/auth/login` com as credenciais do usuário.
2.  O servidor responde com o payload de login:
    ```json
    {
      "accessToken": "eyJhbGciOiJIUzI1NiIsIn...",
      "refreshToken": "7c82a20b-...",
      "expiresAt": "2026-06-17T20:15:00Z"
    }
    ```
3.  O cliente copia o `accessToken` e o anexa nas próximas requisições HTTP dentro do header `Authorization`:
    ```http
    Authorization: Bearer eyJhbGciOiJIUzI1NiIsIn...
    ```

---

## 5. Guia de Execução e Deploy (Passo a Passo)

### Requisitos Prévios
*   **Docker** e **Docker Compose** instalados (Recomendado)  
    *ou*
*   **.NET SDK (versão 8, 9 ou 10)** e um banco **PostgreSQL** ativo localmente.

---

### Opção A: Execução via Docker (Recomendado e Isolado)

1.  **Configurar Variáveis de Ambiente:**  
    Crie um arquivo chamado `.env` na pasta raiz do projeto de arquitetura (`API_Clean_Architecture/`) com o seguinte conteúdo:
    ```env
    GOOGLE_CLIENT_ID=seu_client_id_do_google.apps.googleusercontent.com
    GOOGLE_CLIENT_SECRET=sua_chave_secreta_do_google
    ```
2.  **Iniciar a Aplicação:**  
    Execute o comando na pasta `API_Clean_Architecture/` para compilar e subir os containers do PostgreSQL e da API em segundo plano:
    ```bash
    docker compose up -d --build
    ```
3.  **Acessar a Aplicação:**
    *   O Swagger estará disponível no link: **`http://localhost:5000/swagger`**
    *   O banco PostgreSQL estará disponível externamente na porta `5433` (ex: para DBeaver).

---

### Opção B: Execução Local com Banco em Memória (Sem dependência de Docker ou PostgreSQL)

Esta opção é a mais rápida e não exige que você tenha o Docker Desktop ou o PostgreSQL instalados na sua máquina física. A API rodará utilizando um **Banco de Dados em Memória (Microsoft.EntityFrameworkCore.InMemory)** e inicializará instantaneamente.

1.  **Configurar o modo em memória:**  
    O arquivo `TaskManager.API/appsettings.Development.json` já está configurado por padrão com a propriedade `"UseInMemoryDatabase": "true"`. Isso diz à API para usar o provedor de banco em memória durante o desenvolvimento local.
2.  **Iniciar a API:**  
    No terminal, na pasta `API_Clean_Architecture/`, execute o comando abaixo (aplica roll-forward automático para o seu SDK instalado):
    ```powershell
    $env:DOTNET_ROLL_FORWARD="Major"
    dotnet run --project TaskManager.API
    ```
3.  **Acessar o Swagger:**  
    Acesse pelo navegador o Swagger no endereço: **`http://localhost:5261/swagger`**

---

## 6. Testes Automatizados

A API conta com um projeto de testes unitários (`TaskManager.Tests`) desenvolvido com **xUnit** e **Moq** para isolar as dependências e testar de forma estrita as regras de negócio das camadas de serviços.

### Estratégia de Testes:
*   **Mocking:** Utilização da biblioteca `Moq` para mockar os repositórios (`IUserRepository`, `ITaskRepository`) e serviços externos (`ICalendarService`).
*   **Cenários Testados:**
    *   **Autenticação:** Validação de login com credenciais válidas, criptografia de senhas (BCrypt) e persistência de refresh tokens na base de dados.
    *   **Gestão de Tarefas:** Criação de tarefas com data de término, verificando se o gatilho automático de envio de evento para o `GoogleCalendarService` foi chamado com os parâmetros corretos.
    *   **Segurança de Comentários:** Testes unitários para validar se apenas o autor do comentário ou administradores (`Admin`) conseguem excluir comentários.

### Como Executar os Testes Unitários:
Na pasta `API_Clean_Architecture/`, execute o comando:
```bash
$env:DOTNET_ROLL_FORWARD="Major"
dotnet test
```

---

## 7. Integração Google Calendar (OAuth 2.0)

A sincronização de tarefas com calendário utiliza o fluxo oficial de concessão de consentimento OAuth 2.0 do Google.

### Configuração no Painel do Google Cloud Console
1.  **Ativar API:** Ativar a "Google Calendar API" no console do projeto.
2.  **Consentimento:** Configurar a tela de consentimento de login do OAuth (OAuth Consent Screen) como Externa e incluir os e-mails de teste autorizados.
3.  **Redirect URI:** Registrar a URL de retorno local de callback: `http://localhost:5000/api/auth/google/callback`.
4.  **Credenciais:** Gerar o Client ID e Client Secret e informá-los nas configurações da API (conforme seção 5).
