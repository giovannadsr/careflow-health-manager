using CareFlow.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace CareFlow.Infrastructure.Persistence;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    public DbSet<User> Users => Set<User>();

    public DbSet<Patient> Patients => Set<Patient>();

    public DbSet<TaskItem> Tasks => Set<TaskItem>();
}