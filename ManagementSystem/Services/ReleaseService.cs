using ManagementSystem.DTOs;
using ManagementSystem.Models;
using Microsoft.EntityFrameworkCore;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ManagementSystem.Services;

public class ReleaseService
{
    private readonly ApplicationDbContext _db;

    public ReleaseService(ApplicationDbContext db)
    {
        _db = db;
    }

    public async Task ProcessWebhook(GitLabWebhookDto data)
    {
        var project = await _db.Projects
            .FirstOrDefaultAsync(p => p.GitUrl == data.GitUrl);

        if (project == null) return; // Если проекта нет, ничего не делаем

        if (string.IsNullOrWhiteSpace(data.Description)) return;

        var version = await _db.ProjectVersions
            .FirstOrDefaultAsync(v => v.ProjectId == project.Id && v.VersionNumber == data.Version);

        if (version == null)
        {
            version = new ProjectVersion
            {
                Id = Guid.NewGuid(),
                ProjectId = project.Id,
                VersionNumber = data.Version,
                State = "Dev",
                Tasks = ExtractTasks(data.Description),
                CreatedAt = DateTime.UtcNow
            };
            _db.ProjectVersions.Add(version);
            await _db.SaveChangesAsync();
        }
    }

    private List<string> ExtractTasks(string description)
    {
        var tasks = new List<string>();
        var regex = new Regex(@"https?://[^\s]+");
        var matches = regex.Matches(description);

        foreach (Match match in matches)
        {
            tasks.Add(match.Value);
        }
        return tasks;
    }
}