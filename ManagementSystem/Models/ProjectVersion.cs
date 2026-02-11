using ManagementSystem.Models;

public class ProjectVersion
{
    public Guid Id { get; set; }
    public string VersionNumber { get; set; } = string.Empty;
    public string State { get; set; } = "Dev";
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public Guid ProjectId { get; set; }

    // Добавь это, чтобы ошибка в ThenInclude(v => v.WorkRounds) исчезла
    public List<WorkRound> WorkRounds { get; set; } = new();

    // Добавь это, чтобы ошибка CS0117 (Tasks) исчезла
    public List<string> Tasks { get; set; } = new();
}