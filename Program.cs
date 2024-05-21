using Tarefas.Context;
using Microsoft.EntityFrameworkCore;
using Tarefas.Middleware;
using System.Text.Json.Serialization;
using Tarefas.Models;
using Microsoft.EntityFrameworkCore.Storage.Json;

var builder = WebApplication.CreateBuilder(args);

// builder.Services.AddDbContext<OrganizadorContext>(options =>
//     options.UseSqlite(builder.Configuration.GetConnectionString("ConexaoPadrao")));

builder.Services.AddDbContext<OrganizadorContext>(opt =>
opt.UseInMemoryDatabase("TodoList"));
builder.Services.AddControllers().AddJsonOptions(options =>
{
    options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
    options.JsonSerializerOptions.Converters.Add(new NullableDateTimeConverter());
});

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configuração do middleware de tratamento de exceções
app.UseMiddleware<ExceptionMiddleware>();


// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
