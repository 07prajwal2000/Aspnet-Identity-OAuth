using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using TestAuthServer.Models;

namespace TestAuthServer.Data;

public class DataContext : IdentityDbContext
{
    public DataContext(DbContextOptions<DataContext> o) : base(o)
    {
    }

    public DbSet<User> Users { get; set; }
}
