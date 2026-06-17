# TaskManager API 

API desenvolvida em **.NET 8** seguindo as diretrizes e boas práticas de **Clean Architecture** e princípios do SOLID.

---

## 🛠️ O que foi feito (Histórico de Alterações)

1. **Downgrade do Target (.NET 10 ➔ .NET 8)**: Ajustados todos os arquivos `.csproj` das quatro camadas para rodar nativamente com o SDK do .NET 8, corrigindo dependências NuGet incompatíveis.
2. **Containerização Completa (Docker)**:
   - Criado o **`Dockerfile`** multi-stage otimizado para produção.
   - Atualizado o **`docker-compose.yml`** para orquestrar e conectar a API (`webapi`) e o banco de dados (`postgres`) em uma rede privada interna do Docker.
3. **Segurança nos Comentários**: Implementado validador de autorização no `CommentService` para impedir que um usuário apague comentários criados por outros (permitido apenas para o autor do comentário ou `Admin`).
4. **Mapeamento de DTOs (`TaskResponseDto`)**: Correção no mapper de tarefas para retornar os dados completos do usuário criador (`CreatedBy`) e responsável (`AssignedTo`) nas respostas da API.
5. **Soft Delete Global**: Configurados filtros de consulta globais (`HasQueryFilter`) no `AppDbContext` para ocultar entidades marcadas com `DeletedAt != null` (usuários e tarefas deletados), mantendo a integridade histórica.
6. **Persistência de Refresh Token**:
   - Criada a interface e repositório `IRefreshTokenRepository` para gravação real de refresh tokens.
   - Refatoração do `AuthService` para gerar, persistir e revogar tokens de atualização no login, logout e renovação.
   - Adicionado o endpoint `/api/auth/refresh`.
7. **Integração com Google Calendar**:
   - Criação da interface `ICalendarService` no Domain e implementação concreta em `Infrastructure`.
   - Adicionada lógica de renovação automática do token expirado.
   - Acoplamento automático na criação e atualização de tarefas com prazo.
   - Rota de autenticação Google OAuth no `AuthController` (`/api/auth/google/login` e `/api/auth/google/callback`).
8. **Testes Automatizados**: Criado o projeto de testes unitários `TaskManager.Tests` utilizando `xUnit` e `Moq`.

---

##  Guia de Execução (Tutorial Docker)

Toda a aplicação agora roda dentro do Docker. Você não precisa instalar o PostgreSQL localmente na sua máquina física, o Docker subirá o banco e a API configurados.

### 1. Requisitos
* Docker / Docker Desktop instalado e rodando.
* SDK do .NET 8 (opcional, caso queira rodar localmente fora do container).

### 2. Inicializando a Aplicação (Banco + API)
No terminal, na raiz do projeto (onde está o `docker-compose.yml`), execute o comando para construir e iniciar os containers em segundo plano:
```bash
docker compose up -d --build
```

### 3. Acessando a API e o Swagger
Uma vez inicializados os containers:
* O Swagger (documentação interativa da API) estará disponível no endereço:
  **`http://localhost:5000/swagger`**
* O banco de dados PostgreSQL estará exposto na porta **`5433`** do seu host (para conexões externas usando DBeaver, pgAdmin, etc.).

### 4. Principais Comandos do Docker
* **Ver logs em tempo real**:
  ```bash
  docker compose logs -f
  ```
* **Parar a execução dos containers**:
  ```bash
  docker compose down
  ```
* **Limpar os volumes do banco de dados (reiniciar banco do zero)**:
  ```bash
  docker compose down -v
  ```

---

##  Como Rodar os Testes Unitários

Caso possua o .NET SDK instalado localmente e queira rodar a suíte de testes unitários:

```bash
dotnet test
```

---

##  Fluxo de Teste no Swagger (Passo a Passo)

1. Vá em `/api/users` (POST) e registre um novo usuário.
2. Acesse `/api/auth/login` (POST) com as credenciais que você acabou de criar.
3. Copie o `accessToken` retornado na resposta do login.
4. No topo da página do Swagger, clique no botão **Authorize** (cadeado).
5. No campo de texto, insira `Bearer <SEU_TOKEN_COPIADO>` (exemplo: `Bearer eyJhbGciOi...`) e clique em **Authorize**.
6. A partir deste momento, todos os endpoints protegidos (Tasks, Comments, etc.) estarão acessíveis nas suas rotas.
