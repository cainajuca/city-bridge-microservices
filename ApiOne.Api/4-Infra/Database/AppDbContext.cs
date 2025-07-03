using ApiOne.Api.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace ApiOne.Api.Infra.Database;
public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<City> Cities { get; set; }
}
