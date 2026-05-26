using CareFlow.Domain.Common;
using CareFlow.Domain.Enums;

namespace CareFlow.Domain.Entities;

public class TaskItem : BaseEntity
{
    public string Title { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public DateTime DueDate { get; set; }

    public TaskPriority Priority { get; set; }

    public TaskItemStatus Status { get; set; }

    // FK Paciente
    public Guid PatientId { get; set; }

    public Patient Patient { get; set; } = null!;

    // FK Usuário
    public Guid AssignedUserId { get; set; }

    public User AssignedUser { get; set; } = null!;
}