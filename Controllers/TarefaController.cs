using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Tarefas.Context;
using Tarefas.Models;
using System.Threading.Tasks;
using System.Linq;
using System.Collections.Generic;
using System;

namespace TodoApi.Controllers
{
  [ApiController]
  [Route("[controller]")]
  public class TarefaController : ControllerBase
  {
    private readonly ILogger<TarefaController> _logger;
    private readonly OrganizadorContext _context;

    public TarefaController(OrganizadorContext context, ILogger<TarefaController> logger)
    {
      _context = context;
      _logger = logger;
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> ObterPorId(int id)
    {
      // Buscar a tarefa pelo Id no banco utilizando o EF
      var tarefa = await _context.Tarefas.FindAsync(id);

      // Validar o tipo de retorno. Se não encontrar a tarefa, retornar NotFound,
      // caso contrário retornar OK com a tarefa encontrada
      if (tarefa == null)
        return NotFound(new { Erro = "Tarefa não encontrada" });

      return Ok(tarefa);
    }

    [HttpGet("ObterTodos")]
    public async Task<ActionResult<IEnumerable<Tarefa>>> ObterTodos()
    {
      // Obter todas as tarefas do banco
      var tarefas = await _context.Tarefas.ToListAsync();
      return Ok(tarefas);
    }

    [HttpGet("ObterPorTitulo/{titulo}")]
    public async Task<IActionResult> ObterPorTitulo(string titulo)
    {
      // Validação do título
      if (string.IsNullOrEmpty(titulo?.Trim()))
        return BadRequest(new { Erro = "O título não pode ser vazio" });

      // Buscar tarefas pelo título no banco
      var tarefas = await _context.Tarefas
          .Where(t => t.Titulo.Contains(titulo))
          .ToListAsync();

      return Ok(tarefas);
    }

    [HttpGet("ObterPorData/{data}")]
    public async Task<IActionResult> ObterPorData(DateTime data)
    {
      // Validação da data
      if (data == DateTime.MinValue)
        return BadRequest(new { Erro = "A data não pode ser vazia" });

      // Buscar tarefas pela data no banco
      var tarefa = await _context.Tarefas
          .Where(x => x.Data.Date == data.Date)
          .ToListAsync();

      // Verificação se nenhuma tarefa foi encontrada
      if (!tarefa.Any())
        return NotFound(new { Erro = "Nenhuma tarefa encontrada para a data especificada" });

      return Ok(tarefa);
    }

    [HttpGet("ObterPorStatus/{status}")]
    public async Task<IActionResult> ObterPorStatus(EnumStatusTarefa status)
    {
      // Verificação de validade do status
      if (!Enum.IsDefined(typeof(EnumStatusTarefa), status))
        return BadRequest(new { Erro = "O status fornecido é inválido" });

      // Buscar tarefas pelo status no banco
      var tarefa = await _context.Tarefas
          .Where(x => x.Status == status)
          .ToListAsync();

      // Verificação se nenhuma tarefa foi encontrada
      if (!tarefa.Any())
        return NotFound(new { Erro = "Nenhuma tarefa encontrada com o status especificado" });

      return Ok(tarefa);
    }

    [HttpPost]
    public async Task<IActionResult> Criar([FromBody] Tarefa tarefa)
    {
      try
      {
        // Validação de Data
        if (tarefa.Data == DateTime.MinValue)
          return BadRequest(new { Erro = "A data da tarefa não pode ser vazia" });
        bool data = tarefa.Data.ToString() == "";

        if (tarefa.Data == null || tarefa.Data == default || data)
          return BadRequest(new { Erro = "A data da tarefa não pode ser vazia" });

        if (tarefa.Data < DateTime.Now)
          return BadRequest(new { Erro = "A data da tarefa não pode ser menor que a data atual" });

        // Validação de Título
        if (string.IsNullOrEmpty(tarefa.Titulo?.Trim()))
          return BadRequest(new { Erro = "O título da tarefa não pode ser vazio" });

        // Validação de Descrição
        if (string.IsNullOrEmpty(tarefa.Descricao?.Trim()))
          return BadRequest(new { Erro = "A descrição da tarefa não pode ser vazia" });

        // Validação de Status
        if (!Enum.IsDefined(typeof(EnumStatusTarefa), tarefa.Status))
          return BadRequest(new { Erro = "O status da tarefa é inválido" });

        // Adicionar a tarefa recebida no EF e salvar as mudanças
        _context.Tarefas.Add(tarefa);
        await _context.SaveChangesAsync();
        return CreatedAtAction(nameof(ObterPorId), new { id = tarefa.Id }, tarefa);
      }
      catch (DbUpdateException ex)
      {
        // Captura exceções de atualização de banco de dados
        _logger.LogError(ex, "An error occurred while updating the database.");
        return StatusCode(StatusCodes.Status500InternalServerError, new { Erro = "An error occurred while updating the database. Please try again later." });
      }
      catch (Exception ex)
      {
        // Captura outras exceções inesperadas
        _logger.LogError(ex, "An unexpected error occurred while creating the task.");
        return StatusCode(StatusCodes.Status500InternalServerError, new { Erro = "An unexpected error occurred. Please try again later." });
      }

    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Atualizar(int id, [FromBody] Tarefa tarefa)
    {
      // Validação do ID
      if (id != tarefa.Id)
        return BadRequest(new { Erro = "ID da tarefa inválido" });

      // Buscar a tarefa pelo Id no banco
      var tarefaBanco = await _context.Tarefas.FindAsync(id);

      // Verificação se a tarefa existe no banco
      if (tarefaBanco == null)
        return NotFound(new { Erro = "Tarefa não encontrada" });

      try
      {
        // Validação de Data
        if (tarefa.Data == DateTime.MinValue)
          return BadRequest(new { Erro = "A data da tarefa não pode ser vazia" });

        if (tarefa.Data < DateTime.Now)
          return BadRequest(new { Erro = "A data da tarefa não pode ser menor que a data atual" });

        // Validação de Título
        if (string.IsNullOrEmpty(tarefa.Titulo?.Trim()))
          return BadRequest(new { Erro = "O título da tarefa não pode ser vazio" });

        // Validação de Descrição
        if (string.IsNullOrEmpty(tarefa.Descricao?.Trim()))
          return BadRequest(new { Erro = "A descrição da tarefa não pode ser vazia" });

        // Atualizar os campos da tarefa
        tarefaBanco.Titulo = tarefa.Titulo;
        tarefaBanco.Descricao = tarefa.Descricao;
        tarefaBanco.Data = tarefa.Data;
        tarefaBanco.Status = tarefa.Status;

        // Marcar a tarefa como modificada e salvar as mudanças
        _context.Entry(tarefaBanco).State = EntityState.Modified;
        await _context.SaveChangesAsync();

        return NoContent();
      }
      catch (DbUpdateException ex)
      {
        _logger.LogError(ex, "An error occurred while updating the database.");
        return StatusCode(StatusCodes.Status500InternalServerError, new { Erro = "An error occurred while updating the database. Please try again later." });
      }
      catch (Exception ex)
      {
        _logger.LogError(ex, "An unexpected error occurred while updating the task.");
        return StatusCode(StatusCodes.Status500InternalServerError, new { Erro = "An unexpected error occurred. Please try again later." });
      }
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Deletar(int id)
    {
      // Buscar a tarefa pelo Id no banco
      var tarefaBanco = await _context.Tarefas.FindAsync(id);
      try
      {
        // Verificação se a tarefa existe no banco
        if (tarefaBanco == null)
          return NotFound(new { Erro = "Tarefa não encontrada" });

        // Remover a tarefa do banco e salvar as mudanças
        _context.Tarefas.Remove(tarefaBanco);
        await _context.SaveChangesAsync();

        return NoContent();
      }
      catch (DbUpdateException ex)
      {
        _logger.LogError(ex, "An error occurred while deleting the task.");
        return StatusCode(StatusCodes.Status500InternalServerError, new { Erro = "An error occurred while deleting the task. Please try again later." });
      }
      catch (Exception ex)
      {
        _logger.LogError(ex, "An unexpected error occurred while deleting the task.");
        return StatusCode(StatusCodes.Status500InternalServerError, new { Erro = "An unexpected error occurred. Please try again later." });
      }
    }
  }
}
