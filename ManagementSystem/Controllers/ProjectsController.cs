using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ManagementSystem.Models;

namespace ManagementSystem.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProjectsController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public ProjectsController(ApplicationDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Project>>> GetProjects()
    {
        return await _context.Projects.ToListAsync();
    }

    [HttpPost]
    public async Task<ActionResult<Project>> CreateProject([FromBody] Project project)
    {
        // Если проект пришел пустой
        if (project == null) return BadRequest("Данные проекта не заполнены");

        // Генерируем новый ID, если фронтенд его не прислал
        if (project.Id == Guid.Empty) project.Id = Guid.NewGuid();

        // На всякий случай заполняем заглушку для GitUrl, если она обязательна в БД
        if (string.IsNullOrEmpty(project.GitUrl)) project.GitUrl = "https://placeholder.com";

        _context.Projects.Add(project);

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Ошибка при сохранении в базу: {ex.Message}");
        }

        return CreatedAtAction(nameof(GetProject), new { id = project.Id }, project);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult> GetProject(Guid id)
    {
        // Заменяем Versions на ProjectVersions, как в твоих моделях
        var project = await _context.Projects
            .Include(p => p.ProjectVersions)
                .ThenInclude(v => v.WorkRounds)
            .FirstOrDefaultAsync(p => p.Id == id);

        if (project == null) return NotFound("Проект не найден");

        // Возвращаем данные в формате, который понимает фронтенд
        return Ok(new
        {
            project = project,
            // Мапим ProjectVersions в список для удобства фронтенда
            versions = project.ProjectVersions.Select(v => new {
                id = v.Id,
                versionNumber = v.VersionNumber,
                createdAt = v.CreatedAt,
                // Подтягиваем раунды для отображения меток DEV/STAGE/PROD
                workRounds = v.WorkRounds.Select(r => new {
                    id = r.Id,
                    name = r.Name
                }).ToList()
            }).OrderByDescending(v => v.createdAt).ToList()
        });
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteProject(Guid id)
    {
        var project = await _context.Projects.FindAsync(id);
        if (project == null) return NotFound();

        _context.Projects.Remove(project);
        await _context.SaveChangesAsync();

        return NoContent();
    }

    [HttpPost("versions/{versionId}/upload")]
    public async Task<IActionResult> UploadFile(Guid versionId, IFormFile file)
    {
        var version = await _context.ProjectVersions.FindAsync(versionId);
        if (version == null) return NotFound("Версия не найдена");

        if (file == null || file.Length == 0) return BadRequest("Файл не выбран");

        var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "uploads", versionId.ToString());
        if (!Directory.Exists(uploadsFolder)) Directory.CreateDirectory(uploadsFolder);

        var filePath = Path.Combine(uploadsFolder, file.FileName);

        using (var stream = new FileStream(filePath, FileMode.Create))
        {
            await file.CopyToAsync(stream);
        }

        return Ok(new { message = "Файл успешно загружен", fileName = file.FileName });
    }

    [HttpPost("{projectId}/versions")]
    public async Task<ActionResult> CreateVersion(Guid projectId, [FromBody] ProjectVersion version)
    {
        var project = await _context.Projects.FindAsync(projectId);
        if (project == null) return NotFound("Проект не найден");

        version.Id = Guid.NewGuid();
        version.ProjectId = projectId;
        version.CreatedAt = DateTime.UtcNow;

        _context.ProjectVersions.Add(version);
        await _context.SaveChangesAsync();

        return Ok(version);
    }

    [HttpGet("versions/{versionId}/files")]
    public IActionResult GetVersionFiles(Guid versionId)
    {
        // Путь к папке конкретной версии
        var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "uploads", versionId.ToString());

        if (!Directory.Exists(uploadsFolder))
        {
            // Если папки нет, возвращаем пустой список, а не ошибку
            return Ok(new List<string>());
        }

        // Получаем список имен файлов
        var files = Directory.GetFiles(uploadsFolder)
                             .Select(Path.GetFileName)
                             .ToList();

        return Ok(files);
    }



    [HttpGet("versions/{versionId}/download/{fileName}")]
    public IActionResult DownloadFile(Guid versionId, string fileName)
    {
        var filePath = Path.Combine(Directory.GetCurrentDirectory(), "uploads", versionId.ToString(), fileName);

        if (!System.IO.File.Exists(filePath)) return NotFound("Файл не найден");

        var bytes = System.IO.File.ReadAllBytes(filePath);
        return File(bytes, "application/octet-stream", fileName);
    }


    [HttpPost("versions/{versionId}/rounds")]
    public async Task<ActionResult> CreateRound(Guid versionId, [FromBody] string roundName)
    {
        var version = await _context.ProjectVersions.FindAsync(versionId);
        if (version == null) return NotFound("Версия не найдена");

        var round = new WorkRound
        {
            Id = Guid.NewGuid(),
            ProjectVersionId = versionId,
            Name = roundName,
            StartDate = DateTime.UtcNow,
            Status = "Active"
        };

        _context.WorkRounds.Add(round);
        version.State = roundName;

        await _context.SaveChangesAsync();

        return Ok(new { message = $"Раунд {roundName} начат", roundId = round.Id });
    }
}

