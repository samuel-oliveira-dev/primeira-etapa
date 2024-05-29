using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using RisePayTest.Models;
namespace RisePayTest.Data
{
    public class AppDbContext : DbContext
    {
        private readonly IConfiguration _configuration;  
        public AppDbContext(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            var connectionString = _configuration.GetConnectionString("DefaultConnection");
            optionsBuilder.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString));
        }
        public DbSet<Colaborador> Colaboradores { get; set; }
        public DbSet<Cargo> Cargos { get; set; }
    }
}
