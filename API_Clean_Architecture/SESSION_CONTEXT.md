# Contexto de Sessão — TaskManager API

Este arquivo serve para orientar o reinício da conversa na próxima sessão. Ele detalha onde paramos e a lista de tarefas pendentes.

---

## 1. Visão Geral e Responsabilidades
* **Objetivo:** Implementar a camada **Application** (DTOs, Services e Validators) sem depender diretamente da finalização imediata do banco de dados (Infrastructure) ou das controllers (API).
* **Divisão de Trabalho:**
  * **Athos:** Application (DTOs, Services, Validators), Domain (Interfaces, Enums), JWT.
  * **Luiz Eduardo:** Infrastructure (EF Core, Repositórios, Migrações, Google Calendar).
  * **Murilo:** API (Controllers, Program.cs, Middleware, Swagger).

---

## 2. Onde Paramos Hoje
* Finalizamos a **Fase 1**:
  * O [LoginRequestDto.cs](file:///home/afros_brasiliensis/Documents/Trabalho_de_Engenharia_de_Software/API_Clean_Architecture/TaskManager.Application/Dtos/Auth/LoginRequestDto.cs) foi ajustado com a propriedade `Password` e correções de sintaxe.
  * O [LoginResponseDto.cs](file:///home/afros_brasiliensis/Documents/Trabalho_de_Engenharia_de_Software/API_Clean_Architecture/TaskManager.Application/Dtos/Auth/LoginResponseDto.cs) foi criado contendo `AccessToken`, `Expiration` e `RefreshToken`.
* Iniciamos a **Fase 2**:
  * Criamos os arquivos físicos de DTO de usuário na pasta `TaskManager.Application/Dtos/User/`, mas eles estão vazios (prontos para receber o conteúdo na próxima sessão):
    * [CreateUserDto.cs](file:///home/afros_brasiliensis/Documents/Trabalho_de_Engenharia_de_Software/API_Clean_Architecture/TaskManager.Application/Dtos/User/CreateUserDto.cs)
    * [UpdateUserDto.cs](file:///home/afros_brasiliensis/Documents/Trabalho_de_Engenharia_de_Software/API_Clean_Architecture/TaskManager.Application/Dtos/User/UpdateUserDto.cs)
    * [UserResponseDto.cs](file:///home/afros_brasiliensis/Documents/Trabalho_de_Engenharia_de_Software/API_Clean_Architecture/TaskManager.Application/Dtos/User/UserResponseDto.cs)

---

## 3. Próximos Passos (Para Retomar)

Siga este checklist ordenado para implementar os DTOs guiado pela IA:

### Fase 2: DTOs de Usuário (`User`)
- [ ] Implementar [CreateUserDto.cs](file:///home/afros_brasiliensis/Documents/Trabalho_de_Engenharia_de_Software/API_Clean_Architecture/TaskManager.Application/Dtos/User/CreateUserDto.cs) (`Name`, `Email`, `Password`, `Role` [UserRole]).
- [ ] Implementar [UpdateUserDto.cs](file:///home/afros_brasiliensis/Documents/Trabalho_de_Engenharia_de_Software/API_Clean_Architecture/TaskManager.Application/Dtos/User/UpdateUserDto.cs) (`Name?`, `Email?`).
- [ ] Implementar [UserResponseDto.cs](file:///home/afros_brasiliensis/Documents/Trabalho_de_Engenharia_de_Software/API_Clean_Architecture/TaskManager.Application/Dtos/User/UserResponseDto.cs) (`Id`, `Name`, `Email`, `Role`, `CreatedAt`).

### Fase 3: DTOs de Tarefas (`Task`)
- [ ] Criar [CreateTaskDto.cs](file:///home/afros_brasiliensis/Documents/Trabalho_de_Engenharia_de_Software/API_Clean_Architecture/TaskManager.Application/Dtos/Task/CreateTaskDto.cs).
- [ ] Criar [UpdateTaskDto.cs](file:///home/afros_brasiliensis/Documents/Trabalho_de_Engenharia_de_Software/API_Clean_Architecture/TaskManager.Application/Dtos/Task/UpdateTaskDto.cs).
- [ ] Criar [TaskResponseDto.cs](file:///home/afros_brasiliensis/Documents/Trabalho_de_Engenharia_de_Software/API_Clean_Architecture/TaskManager.Application/Dtos/Task/TaskResponseDto.cs).
- [ ] Criar [TaskFilterDto.cs](file:///home/afros_brasiliensis/Documents/Trabalho_de_Engenharia_de_Software/API_Clean_Architecture/TaskManager.Application/Dtos/Task/TaskFilterDto.cs).

### Fase 4: DTOs de Comentários (`Comment`)
- [ ] Criar [CreateCommentDto.cs](file:///home/afros_brasiliensis/Documents/Trabalho_de_Engenharia_de_Software/API_Clean_Architecture/TaskManager.Application/Dtos/Comment/CreateCommentDto.cs).
- [ ] Criar [CommentResponseDto.cs](file:///home/afros_brasiliensis/Documents/Trabalho_de_Engenharia_de_Software/API_Clean_Architecture/TaskManager.Application/Dtos/Comment/CommentResponseDto.cs).

---

## Como retomar a conversa com o assistente de IA na próxima sessão:
*Copie e envie a seguinte mensagem para reestabelecer o contexto imediatamente:*

> **Mensagem para colar no chat:**
> "Olá! Vamos continuar o desenvolvimento da camada de Application do TaskManager. Leia o arquivo [SESSION_CONTEXT.md](file:///home/afros_brasiliensis/Documents/Trabalho_de_Engenharia_de_Software/API_Clean_Architecture/SESSION_CONTEXT.md) para recuperar o nosso andamento e me guie a partir da Fase 2 (DTOs de Usuário)."
