namespace WebApi.Helpers;

using Microsoft.EntityFrameworkCore;
using WebApi.Entities;

public class DataContext : DbContext
{
    public DbSet<User> Users { get; set; }

    private readonly IConfiguration Configuration;

    public DataContext(IConfiguration configuration)
    {
        Configuration = configuration;
    }

    protected override void OnConfiguring(DbContextOptionsBuilder options)
    {
        // connect to sqlite database - works both locally and in production
        options.UseSqlite(Configuration.GetConnectionString("WebApiDatabase"));
        
        // Will only works locally / for developement
        // In memory database used for simplicity, change to a real db for production applications
        //options.UseInMemoryDatabase("TestDb");
    }
}