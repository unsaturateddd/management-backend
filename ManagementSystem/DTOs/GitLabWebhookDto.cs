namespace ManagementSystem.DTOs;

public class GitLabWebhookDto
{
    public string GitUrl { get; set; } = string.Empty;
    public string TargetBranch { get; set; } = string.Empty;
    public string Version { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
}