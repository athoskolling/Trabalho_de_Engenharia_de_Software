# Entendendo DTOs (Data Transfer Objects) na Clean Architecture

Este documento explica o propósito, o funcionamento e as diretrizes de implementação de DTOs no projeto **TaskManager**.

---

## 1. O que é um DTO e para que serve?

O **DTO (Data Transfer Object)** é um objeto simples (apenas propriedades, sem lógica de negócios) usado para transportar dados entre diferentes camadas do sistema, especificamente entre a camada de **API** (Controladores) e a camada de **Application** (Serviços e Casos de Uso).

### Benefícios no uso de DTOs:
1. **Desacoplamento:** A API externa (JSON) fica separada do banco de dados e das entidades de Domínio. Se uma entidade interna mudar, a API pública não quebra.
2. **Segurança:** Evita expor dados confidenciais (ex: `PasswordHash`) retornando apenas o que é seguro (ex: `UserResponseDto`).
3. **Validação Simplificada:** Facilita a validação das propriedades de entrada (com FluentValidation) antes de enviá-las para os serviços.

---

## 2. Por que DTOs usam apenas `{ get; set; }` e sem construtores com parâmetros?

DTOs são projetados como **estruturas anêmicas de dados**. Eles não devem conter lógica de negócio ou construtores com parâmetros obrigatórios devido à **compatibilidade com a Desserialização**:

```
[JSON do Cliente]  ───(Envio na Rede)───►  [C# Object (DTO)]
```

* Quando uma requisição HTTP chega na API, a biblioteca do .NET (`System.Text.Json`) precisa converter o texto JSON em um objeto C#.
* Para fazer isso de forma automática e performática, o .NET exige um **construtor padrão vazio** (sem parâmetros) para instanciar a classe e propriedades com **`get; set;`** (ou `init;`) para preencher os valores mapeados pelo nome das chaves.

---

## 3. Como o sistema sabe "para onde ir"? (Roteamento e Model Binding)

O fluxo de comunicação é coordenado de forma transparente pelo ASP.NET Core através de dois mecanismos:

1. **Roteamento (Routing):** O framework analisa a URL e o Método HTTP (ex: `POST /api/auth/login`) e decide qual controlador (`AuthController`) e qual ação (`Login`) devem processar a chamada.
2. **Model Binding:** Ao identificar o parâmetro decorado com `[FromBody] LoginRequestDto request` na ação, o ASP.NET intercepta o JSON do corpo da requisição, desserializa-o criando uma instância do DTO com os valores preenchidos e o passa como argumento para o método.

Se o JSON contiver campos inválidos ou não mapeados, o desserializador os ignora por padrão. A validação subsequente ( FluentValidation ) garante que as propriedades obrigatórias do DTO estejam corretas antes de a requisição prosseguir.

---

## 4. Estrutura e Conteúdo dos DTOs do Projeto

### 📂 Dtos/Auth
* **`LoginRequestDto.cs`**: `Email`, `Password`.
  * *Entrada para autenticação (email e senha em texto limpo).*
* **`LoginResponseDto.cs`**: `AccessToken`, `RefreshToken`, `ExpiresAt`.
  * *Dados retornados para o frontend gerenciar a sessão do usuário.*

### 📂 Dtos/User
* **`CreateUserDto.cs`**: `Name`, `Email`, `Password`, `Role`.
  * *Campos necessários para registrar uma nova conta.*
* **`UpdateUserDto.cs`**: `Name?`, `Email?` (nullable).
  * *Permite atualização parcial (mudar apenas um dos campos).*
* **`UserResponseDto.cs`**: `Id`, `Name`, `Email`, `Role`, `CreatedAt`.
  * *Exibição pública de dados do usuário (nunca expõe hash de senha).*

### 📂 Dtos/Task
* **`CreateTaskDto.cs`**: `Title`, `Description`, `Priority`, `DueDate?`, `AssignedToId?`.
  * *Dados para criar uma tarefa. O criador (`CreatedById`) é extraído do token JWT da sessão.*
* **`UpdateTaskDto.cs`**: `Title?`, `Description?`, `Status?`, `Priority?`, `DueDate?`, `AssignedToId?`.
  * *Permite alterar campos específicos da tarefa individualmente.*
* **`TaskResponseDto.cs`**: `Id`, `Title`, `Description`, `Status`, `Priority`, `DueDate?`, `CreatedAt`, `UpdatedAt`, `AssignedTo` (`UserResponseDto?`), `CreatedBy` (`UserResponseDto`).
  * *Retorna os detalhes completos da tarefa e objetos detalhados dos usuários associados.*
* **`TaskFilterDto.cs`**: `Status?`, `Priority?`, `DueBefore?`, `AssignedToId?`.
  * *Utilizado para filtros dinâmicos na busca/listagem de tarefas.*

### 📂 Dtos/Comment
* **`CreateCommentDto.cs`**: `Content`.
  * *Apenas o texto. O ID da tarefa vem da rota HTTP e o autor é extraído do JWT.*
* **`CommentResponseDto.cs`**: `Id`, `Content`, `CreatedAt`, `User` (`UserResponseDto`).
  * *Estrutura para exibir comentários na tela de detalhes da tarefa.*
