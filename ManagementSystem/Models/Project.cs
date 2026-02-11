namespace ManagementSystem.Models;

public class Project
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string ProjectType { get; set; } = string.Empty;

    // Добавьте это поле, чтобы ушла ошибка на скриншоте image_c4c5de
    public string GitUrl { get; set; } = string.Empty;

    // Добавьте это, чтобы заработал Include в контроллере (image_c4be9b)
    public List<ProjectVersion> ProjectVersions { get; set; } = new();
}