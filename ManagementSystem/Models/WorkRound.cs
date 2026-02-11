namespace ManagementSystem.Models;

public class WorkRound
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public DateTime StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public Guid ProjectVersionId { get; set; }
    public string Status { get; set; } = "Active";


}