using EscolaApi.Data;
using EscolaApi.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EscolaApi.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
public class AlunosController : ControllerBase
{
    private readonly AppDbContext db;

    public AlunosController(AppDbContext db)
    {
        this.db = db;
    }

    
    [HttpGet]
    public async Task<IActionResult> GetAlunos([FromQuery] int page = 1, [FromQuery] int size = 10)
    {
        if (page < 1) page = 1;
        if (size < 1 || size > 50) size = 10;

        var totalItems = await db.Alunos.CountAsync();
        
        
        var alunos = await db.Alunos
            .AsNoTracking()
            .Skip((page - 1) * size)
            .Take(size)
            .Select(a => new
            {
                a.Id,
                a.Nome, 
                MatriculasCount = a.Matriculas.Count
            })
            .ToListAsync();

        var response = new
        {
            Page = page,
            PageSize = size,
            TotalItems = totalItems,
            TotalPages = (int)Math.Ceiling(totalItems / (double)size),
            Data = alunos
        };

        return Ok(response); 
    }

    
    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetAlunoPorId(int id)
    {
        var aluno = await db.Alunos
            .AsNoTracking()
            .Include(a => a.Matriculas)
            .FirstOrDefaultAsync(a => a.Id == id);

        if (aluno is null)
        {

            return NotFound(new ProblemDetails
            {
                Status = 404,
                Title = "Aluno não encontrado",
                Detail = $"Não foi possível encontrar nenhum aluno cadastrado com o ID {id}."
            });
        }

        return Ok(aluno); 
    }

    
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> DeletarAluno(int id)
    {
        var aluno = await db.Alunos.FindAsync(id);

        if (aluno is null)
        {
            return NotFound(new ProblemDetails
            {
                Status = 404,
                Title = "Aluno não encontrado",
                Detail = $"Não foi possível deletar pois o aluno ID {id} não existe."
            });
        }

        db.Alunos.Remove(aluno);
        await db.SaveChangesAsync();

        return NoContent(); 
    }

  
    [HttpGet("{alunoId:int}/matriculas")]
    public async Task<IActionResult> GetMatriculasPorAluno(int alunoId, [FromQuery] int? matriculaId)
    {
        var alunoExiste = await db.Alunos.AnyAsync(a => a.Id == alunoId);

        if (!alunoExiste)
        {
          
            return NotFound(new ProblemDetails
            {
                Status = 404,
                Title = "Aluno não encontrado",
                Detail = $"O aluno com o ID {alunoId} não existe na base de dados."
            });
        }

        var query = db.Matriculas.AsNoTracking().Where(m => m.AlunoId == alunoId);

        if (matriculaId.HasValue)
        {
            query = query.Where(m => m.Id == matriculaId.Value);
        }

        var matriculas = await query.ToListAsync();

        return Ok(matriculas); 
    }
}