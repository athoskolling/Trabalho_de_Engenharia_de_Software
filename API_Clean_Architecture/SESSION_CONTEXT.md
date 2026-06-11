# Contexto de Sessão — TaskManager API

Este arquivo serve para orientar o reinício da conversa amanhã. Ele detalha onde paramos, a divisão de responsabilidades da equipe e a lista de tarefas pendentes.

---

## 1. Visão Geral e Responsabilidades
* **Objetivo:** Implementar a camada **Application** (DTOs, Services e Validators) sem depender diretamente da finalização imediata do banco de dados (Infrastructure) ou das controllers (API).
* **Divisão de Trabalho:**
  * **Athos:** Application (DTOs, Services, Validators), Domain (Interfaces, Enums), JWT.
  * **Luiz Eduardo:** Infrastructure (EF Core, Repositórios, Migrações, Google Calendar).
  * **Murilo:** API (Controllers, Program.cs, Middleware, Swagger).

---

## 2. Onde Paramos Hoje
* Analisamos o arquivo `T2_CONTEXT.md` na pasta Downloads.
* Mapeamos a pasta `TaskManager.Application/Dtos` no workspace.
* Identificamos que o [LoginRequestDto.cs](file:///C:/Users/athos/Documents/GitHub/Trabalho_de_Engenharia_de_Software/API_Clean_Architecture/TaskManager.Application/Dtos/Auth/LoginRequestDto.cs) está incompleto (falta a propriedade `Password`).
* As pastas de DTOs (`User`, `Task`, `Comment`) estão vazias e preparadas para receber suas classes.
* Criamos o arquivo de estudos [DTOS_EXPLANATION.md](file:///C:/Users/athos/Documents/GitHub/Trabalho_de_Engenharia_de_Software/API_Clean_Architecture/DTOS_EXPLANATION.md) detalhando por que DTOs usam apenas `get; set;`, o processo de desserialização e o fluxo de Roteamento/Model Binding no .NET.

---

## 3. Próximos Passos (Para Retomar Amanhã)

Siga este checklist ordenado para implementar os DTOs guiado pela IA:

### Fase 1: Ajuste e Criação de DTOs de Autenticação (`Auth`)
- [ ] Atualizar o [LoginRequestDto.cs](file:///C:/Users/athos/Documents/GitHub/Trabalho_de_Engenharia_de_Software/API_Clean_Architecture/TaskManager.Application/Dtos/Auth/LoginRequestDto.cs) adicionando a propriedade `Password`.
- [ ] Criar o [LoginResponseDto.cs](file:///C:/Users/athos/Documents/GitHub/Trabalho_de_Engenharia_de_Software/API_Clean_Architecture/TaskManager.Application/Dtos/Auth/LoginResponseDto.cs) (`AccessToken`, `RefreshToken`, `ExpiresAt`).

### Fase 2: DTOs de Usuário (`User`)
- [ ] Criar [CreateUserDto.cs](file:///C:/Users/athos/Documents/GitHub/Trabalho_de_Engenharia_de_Software/API_Clean_Architecture/TaskManager.Application/Dtos/User/CreateUserDto.cs) (`Name`, `Email`, `Password`, `Role`).
- [ ] Criar [UpdateUserDto.cs](file:///C:/Users/athos/Documents/GitHub/Trabalho_de_Engenharia_de_Software/API_Clean_Architecture/TaskManager.Application/Dtos/User/UpdateUserDto.cs) (`Name?`, `Email?`).
- [ ] Criar [UserResponseDto.cs](file:///C:/Users/athos/Documents/GitHub/Trabalho_de_Engenharia_de_Software/API_Clean_Architecture/TaskManager.Application/Dtos/User/UserResponseDto.cs) (`Id`, `Name`, `Email`, `Role`, `CreatedAt`).

### Fase 3: DTOs de Tarefas (`Task`)
- [ ] Criar [CreateTaskDto.cs](file:///C:/Users/athos/Documents/GitHub/Trabalho_de_Engenharia_de_Software/API_Clean_Architecture/TaskManager.Application/Dtos/Task/CreateTaskDto.cs).
- [ ] Criar [UpdateTaskDto.cs](file:///C:/Users/athos/Documents/GitHub/Trabalho_de_Engenharia_de_Software/API_Clean_Architecture/TaskManager.Application/Dtos/Task/UpdateTaskDto.cs).
- [ ] Criar [TaskResponseDto.cs](file:///C:/Users/athos/Documents/GitHub/Trabalho_de_Engenharia_de_Software/API_Clean_Architecture/TaskManager.Application/Dtos/Task/TaskResponseDto.cs).
- [ ] Criar [TaskFilterDto.cs](file:///C:/Users/athos/Documents/GitHub/Trabalho_de_Engenharia_de_Software/API_Clean_Architecture/TaskManager.Application/Dtos/Task/TaskFilterDto.cs).

### Fase 4: DTOs de Comentários (`Comment`)
- [ ] Criar [CreateCommentDto.cs](file:///C:/Users/athos/Documents/GitHub/Trabalho_de_Engenharia_de_Software/API_Clean_Architecture/TaskManager.Application/Dtos/Comment/CreateCommentDto.cs).
- [ ] Criar [CommentResponseDto.cs](file:///C:/Users/athos/Documents/GitHub/Trabalho_de_Engenharia_de_Software/API_Clean_Architecture/TaskManager.Application/Dtos/Comment/CommentResponseDto.cs).

---

## Como retomar a conversa com o assistente de IA amanhã:
*Copie e envie a seguinte mensagem para reestabelecer o contexto imediatamente:*

> **Mensagem para colar no chat:**
> "Olá! Vamos continuar o desenvolvimento da camada de Application do TaskManager. Leia o arquivo [SESSION_CONTEXT.md](file:///C:/Users/athos/Documents/GitHub/Trabalho_de_Engenharia_de_Software/API_Clean_Architecture/SESSION_CONTEXT.md) e o [DTOS_EXPLANATION.md](file:///C:/Users/athos/Documents/GitHub/Trabalho_de_Engenharia_de_Software/API_Clean_Architecture/DTOS_EXPLANATION.md) na raiz da pasta `API_Clean_Architecture` para recuperar o nosso andamento e me guie a partir da Fase 1 dos DTOs."
