using CareFlow.Domain.Common;
using CareFlow.Domain.Enums;

namespace CareFlow.Domain.Entities;

public class User : BaseEntity
{
    public string Name { get; set; } = string.Empty;

    public string Email { get; set; } = string.Empty;

    public string PasswordHash { get; set; } = string.Empty;

    public UserRole Role { get; set; }

    // Relacionamento
    public ICollection<TaskItem> Tasks { get; set; }
        = new List<TaskItem>();
}