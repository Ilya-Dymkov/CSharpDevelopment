using CSharpDevelopment.Models;
using Microsoft.EntityFrameworkCore;

namespace CSharpDevelopment.Data;

public class DataDbContext : DbContext
{
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) => 
        optionsBuilder.UseSqlite("Data Source=DataBases/users.db");

    public DbSet<User> Users { get; set; }
}