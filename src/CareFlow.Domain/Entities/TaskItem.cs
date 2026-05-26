using CareFlow.Domain.Enums;

namespace CareFlow.Domain.Entities;

public class TaskItem
{
    public Guid Id { get; set; }

    public string Title { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public DateTime DueDate { get; set; }

    public TaskPriority Priority { get; set; }

    public TaskItemStatus Status { get; set; }

    public Guid PatientId { get; set; }

    public Patient Patient { get; set; } = null!;

    public Guid AssignedUserId { get; set; }

    public User AssignedUser { get; set; } = null!;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}