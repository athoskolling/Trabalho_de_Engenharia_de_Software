# Estágio de Build
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copia os arquivos de projeto (.csproj) para restaurar as dependências
COPY ["TaskManager.API/TaskManager.API.csproj", "TaskManager.API/"]
COPY ["TaskManager.Application/TaskManager.Application.csproj", "TaskManager.Application/"]
COPY ["TaskManager.Infrastructure/TaskManager.Infrastructure.csproj", "TaskManager.Infrastructure/"]
COPY ["TaskManager.Domain/TaskManager.Domain.csproj", "TaskManager.Domain/"]

# Restaura as dependências
RUN dotnet restore "TaskManager.API/TaskManager.API.csproj"

# Copia todo o código fonte e compila a aplicação
COPY . .
WORKDIR "/src/TaskManager.API"
RUN dotnet build "TaskManager.API.csproj" -c Release -o /app/build

# Estágio de Publicação
FROM build AS publish
RUN dotnet publish "TaskManager.API.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Estágio Final
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
COPY --from=publish /app/publish .

# Define a URL de escuta do ASP.NET Core e expõe a porta
ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080

ENTRYPOINT ["dotnet", "TaskManager.API.dll"]
