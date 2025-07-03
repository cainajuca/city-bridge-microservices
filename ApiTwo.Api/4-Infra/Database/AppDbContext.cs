using ApiTwo.Api._3_Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace ApiTwo.Api._4_Infra.Database;
public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<Bridge> Bridges { get; set; }
}
