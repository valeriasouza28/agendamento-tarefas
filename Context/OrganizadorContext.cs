using Microsoft.EntityFrameworkCore;
using Tarefas.Models;
namespace Tarefas.Context
{
  public class OrganizadorContext : DbContext
  {
    public OrganizadorContext(DbContextOptions<OrganizadorContext> options) : base(options)
    {

    }

    public DbSet<Tarefa> Tarefas { get; set; }
  }
}