using Microsoft.EntityFrameworkCore;
using ManagementSystem.Models;

namespace ManagementSystem;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<Project> Projects { get; set; } = null!;
    public DbSet<Product> Products { get; set; } = null!;
    public DbSet<ProjectVersion> ProjectVersions { get; set; } = null!;
    public DbSet<WorkRound> WorkRounds { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Product>()
            .HasMany(p => p.Projects)
            .WithMany();

        base.OnModelCreating(modelBuilder);
    }
}