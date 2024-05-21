markdown

Copiar código

`# Tarefas API

Esta é uma aplicação ASP.NET Core Web API para gerenciar tarefas. A aplicação permite criar, ler, atualizar e excluir tarefas, bem como realizar buscas baseadas em critérios específicos como título, data e status.

## Requisitos

- [.NET 6 SDK](https://dotnet.microsoft.com/download/dotnet/6.0)
- [SQLite](https://www.sqlite.org/download.html)

## Configuração do Ambiente

### 1. Clonar o Repositório

```bash
git clone https://github.com/seu-usuario/tarefas-api.git
cd tarefas-api` 

### 2. Configurar o Banco de Dados

A aplicação está configurada para usar SQLite. Certifique-se de que o arquivo de banco de dados esteja configurado corretamente no `appsettings.Development.json`:

json

Copiar código

`{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "AllowedHosts": "*",
  "ConnectionStrings": {
    "DefaultConnection": "Data Source=tarefas.db"
  }
}` 

### 3. Atualizar o Banco de Dados

Execute as migrações para criar o banco de dados:

bash

Copiar código

`dotnet ef database update` 

### 4. Executar a Aplicação

Para iniciar a aplicação, execute:

bash

Copiar código

`dotnet run` 

A API estará disponível em `https://localhost:5001`.

## Endpoints da API

### Criar Tarefa

http

Copiar código

`POST /Tarefa` 

Body:

json

Copiar código

`{
  "titulo": "Nova Tarefa",
  "descricao": "Descrição da nova tarefa",
  "data": "2023-05-21T00:00:00",
  "status": 0
}` 

### Obter Tarefa por ID

http

Copiar código

`GET /Tarefa/{id}` 

### Obter Todas as Tarefas

http

Copiar código

`GET /Tarefa/ObterTodos` 

### Obter Tarefas por Título

http

Copiar código

`POST /Tarefa/ObterPorTitulo` 

Body:

json

Copiar código

`"Exemplo de Título"` 

### Obter Tarefas por Data

http

Copiar código

`GET /Tarefa/ObterPorData` 

Body:

json

Copiar código

`"2023-05-21"` 

### Obter Tarefas por Status

http

Copiar código

`GET /Tarefa/ObterPorStatus` 

Body:

json

Copiar código

`0` 

### Atualizar Tarefa

http

Copiar código

`PUT /Tarefa/{id}` 

Body:

json

Copiar código

`{
  "id": 1,
  "titulo": "Tarefa Atualizada",
  "descricao": "Descrição atualizada",
  "data": "2023-05-22T00:00:00",
  "status": 1
}` 

### Deletar Tarefa

http

Copiar código

`DELETE /Tarefa/{id}` 

## Middleware de Exceção

A aplicação inclui um middleware personalizado para lidar com exceções não tratadas e retornar respostas JSON apropriadas. O middleware está configurado no `Program.cs`.

csharp

Copiar código

`var builder = WebApplication.CreateBuilder(args);

// Adicionar serviços ao contêiner
builder.Services.AddControllers().AddJsonOptions(options =>
{
    options.JsonSerializerOptions.Converters.Add(new NullableDateTimeConverter());
});

// Configuração de outros serviços...

builder.Services.AddTransient<ExceptionMiddleware>();

var app = builder.Build();

// Configuração do pipeline HTTP...
app.UseMiddleware<ExceptionMiddleware>();

app.UseRouting();

app.UseEndpoints(endpoints =>
{
    endpoints.MapControllers();
});

app.Run();` 

## Contribuição

Contribuições são bem-vindas! Sinta-se à vontade para abrir issues e pull requests.

## Licença

Este projeto está licenciado sob a Licença MIT. Veja o arquivo LICENSE para mais detalhes.

javascript

Copiar código

 ``Este `README.md` cobre a configuração do ambiente, execução da aplicação, descrição dos endpoints da API, e detalhes sobre o middleware de exceção. Sinta-se à vontade para ajustar ou expandir conforme necessário para o seu projeto específico.``