using Microsoft.EntityFrameworkCore;
using MinimalApiRecordTests.Data.Model;

namespace MinimalApiRecordTests.Data;

public class DataContext : DbContext
{
    public DbSet<UserData> Users { get; set; }

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    public DataContext(DbContextOptions<DataContext> dbContextOptions) : base(dbContextOptions)
    {
    }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(DataContext).Assembly);
    }
}
