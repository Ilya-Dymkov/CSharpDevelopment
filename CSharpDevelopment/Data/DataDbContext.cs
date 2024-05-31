using CSharpDevelopment.Models;
using Microsoft.EntityFrameworkCore;

namespace CSharpDevelopment.Data;

public class DataDbContext : DbContext
{
    private readonly Func<DbContextOptionsBuilder, DbContextOptionsBuilder> _func;
    
    public DbSet<User> Users { get; set; }

    public DataDbContext() => _func = builder => builder.UseSqlite("Data Source=DataBases/users.db");

    public DataDbContext(Func<DbContextOptionsBuilder, DbContextOptionsBuilder> func) => _func = func;

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) => _func(optionsBuilder);
}