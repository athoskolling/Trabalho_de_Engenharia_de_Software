# Contexto de Sessão — TaskManager API

Este arquivo gerencia o andamento do projeto e serve para orientar o reinício das conversas a cada sessão.

---

## 1. Funcionamento da Sessão
* **Regra de Ouro:** A IA atua estritamente como professora (orientando e explicando), sem escrever nenhum código. Consulte o arquivo [TEACHER_AGENT.md](file:///home/afros_brasiliensis/Documents/Trabalho_de_Engenharia_de_Software/TEACHER_AGENT.md) para regras completas.
* **Finalização de Sessão:** Ao receber o comando `finalizar sessão`, a IA executará automaticamente o commit e push para a branch diária correspondente à data atual (ex: `daily/yyyy-MM-dd`).

---

## 2. Divisão de Trabalho
* **Objetivo:** Implementar a camada **Application** (DTOs, Services e Validators) sem depender diretamente da infraestrutura ou controllers.
* **Divisão de Trabalho original:**
  * **Athos:** Application (DTOs, Services, Validators), Domain (Interfaces, Enums), JWT.
  * **Luiz Eduardo:** Infrastructure (EF Core, Repositórios, Migrações, Google Calendar).
  * **Murilo:** API (Controllers, Program.cs, Middleware, Swagger).

---

## 3. Onde Paramos Hoje
* **Estrutura de DTOs Preparada:** Todos os arquivos físicos de DTOs nas pastas `User`, `Task` e `Comment` foram criados, mas seus conteúdos foram resetados (apenas a declaração de `namespace` foi mantida) para que **você** possa codar cada um deles sob a orientação da IA.
* **Nova Organização:** Os arquivos de contexto ([SESSION_CONTEXT.md](file:///home/afros_brasiliensis/Documents/Trabalho_de_Engenharia_de_Software/SESSION_CONTEXT.md)), regras de agente ([TEACHER_AGENT.md](file:///home/afros_brasiliensis/Documents/Trabalho_de_Engenharia_de_Software/TEACHER_AGENT.md)) e plano de ação ([plano_de_acao.md](file:///home/afros_brasiliensis/Documents/Trabalho_de_Engenharia_de_Software/plano_de_acao.md)) foram movidos e criados na raiz da pasta `Trabalho_de_Engenharia_de_Software`.
* **Explicação do CreateUserDto Concluída:** A IA detalhou os conceitos de DTO e as propriedades necessárias para o [CreateUserDto.cs](file:///home/afros_brasiliensis/Documents/Trabalho_de_Engenharia_de_Software/API_Clean_Architecture/TaskManager.Application/Dtos/User/CreateUserDto.cs). A próxima sessão deve iniciar com a escrita deste arquivo por parte do usuário.

---

## 4. Próximos Passos (Checklist para o Usuário implementar)

### Fase 2: Implementar DTOs de Usuário (`User`)
- [ ] Implementar [CreateUserDto.cs](file:///home/afros_brasiliensis/Documents/Trabalho_de_Engenharia_de_Software/API_Clean_Architecture/TaskManager.Application/Dtos/User/CreateUserDto.cs) (`Name`, `Email`, `Password`, `Role` [UserRole]).
- [ ] Implementar [UpdateUserDto.cs](file:///home/afros_brasiliensis/Documents/Trabalho_de_Engenharia_de_Software/API_Clean_Architecture/TaskManager.Application/Dtos/User/UpdateUserDto.cs) (`Name?`, `Email?`).
- [ ] Implementar [UserResponseDto.cs](file:///home/afros_brasiliensis/Documents/Trabalho_de_Engenharia_de_Software/API_Clean_Architecture/TaskManager.Application/Dtos/User/UserResponseDto.cs) (`Id`, `Name`, `Email`, `Role`, `CreatedAt`).

### Fase 3: Implementar DTOs de Tarefas (`Task`)
- [ ] Implementar [CreateTaskDto.cs](file:///home/afros_brasiliensis/Documents/Trabalho_de_Engenharia_de_Software/API_Clean_Architecture/TaskManager.Application/Dtos/Task/CreateTaskDto.cs) (`Title`, `Description`, `Priority` [TaskPriority], `DueDate?`, `AssignedToId?`).
- [ ] Implementar [UpdateTaskDto.cs](file:///home/afros_brasiliensis/Documents/Trabalho_de_Engenharia_de_Software/API_Clean_Architecture/TaskManager.Application/Dtos/Task/UpdateTaskDto.cs) (`Title?`, `Description?`, `Status?` [TaskStatus], `Priority?`, `DueDate?`, `AssignedToId?`).
- [ ] Implementar [TaskResponseDto.cs](file:///home/afros_brasiliensis/Documents/Trabalho_de_Engenharia_de_Software/API_Clean_Architecture/TaskManager.Application/Dtos/Task/TaskResponseDto.cs) (`Id`, `Title`, `Description`, `Status`, `Priority`, `DueDate?`, `CreatedAt`, `UpdatedAt`, `AssignedTo` [UserResponseDto?], `CreatedBy` [UserResponseDto]).
- [ ] Implementar [TaskFilterDto.cs](file:///home/afros_brasiliensis/Documents/Trabalho_de_Engenharia_de_Software/API_Clean_Architecture/TaskManager.Application/Dtos/Task/TaskFilterDto.cs) (`Status?`, `Priority?`, `DueBefore?`, `AssignedToId?`).

### Fase 4: Implementar DTOs de Comentários (`Comment`)
- [ ] Implementar [CreateCommentDto.cs](file:///home/afros_brasiliensis/Documents/Trabalho_de_Engenharia_de_Software/API_Clean_Architecture/TaskManager.Application/Dtos/Comment/CreateCommentDto.cs) (`Content`).
- [ ] Implementar [CommentResponseDto.cs](file:///home/afros_brasiliensis/Documents/Trabalho_de_Engenharia_de_Software/API_Clean_Architecture/TaskManager.Application/Dtos/Comment/CommentResponseDto.cs) (`Id`, `Content`, `CreatedAt`, `User` [UserResponseDto]).

### Fase 5: Validação com FluentValidation
- [ ] Criar validador para `LoginRequestDto` (`LoginRequestDtoValidator.cs`).
- [ ] Criar validador para `CreateUserDto` (`CreateUserDtoValidator.cs`).
- [ ] Criar validador para `UpdateUserDto` (`UpdateUserDtoValidator.cs`).
- [ ] Criar validador para `CreateTaskDto` (`CreateTaskDtoValidator.cs`).
- [ ] Criar validador para `UpdateTaskDto` (`UpdateTaskDtoValidator.cs`).
- [ ] Criar validador para `CreateCommentDto` (`CreateCommentDtoValidator.cs`).

### Fase 6: Interfaces e Repositórios (Domain)
- [ ] Definir interfaces de repositórios para `IUserRepository`, `ITaskRepository` e `ICommentRepository`.

### Fase 7: Serviços da Aplicação (Application Services)
- [ ] Implementar interfaces de serviços (`IUserService`, `ITaskService`, `ICommentService`).
- [ ] Guiar a implementação das classes concretas dos serviços e lógica de negócios.

---

## Como retomar a conversa na próxima sessão:
*Copie e envie a seguinte mensagem para reestabelecer o contexto imediatamente:*

> **Mensagem para colar no chat:**
> "Olá! Vamos continuar o desenvolvimento do TaskManager sob o **Modo Professor**. Leia o arquivo de contexto [SESSION_CONTEXT.md](file:///home/afros_brasiliensis/Documents/Trabalho_de_Engenharia_de_Software/SESSION_CONTEXT.md) no diretório raiz e me oriente no próximo passo sem gerar código."
