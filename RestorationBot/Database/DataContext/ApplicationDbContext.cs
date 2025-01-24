namespace RestorationBot.Database.DataContext;

using Microsoft.EntityFrameworkCore;
using Models;

public sealed class ApplicationDbContext : DbContext
{
    public ApplicationDbContext()
    {
        Database.Migrate();
    }

    public DbSet<User> Users { get; set; }


    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);
    }
}