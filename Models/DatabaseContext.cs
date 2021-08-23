using Microsoft.EntityFrameworkCore;
using ProyectoWeb2Api.Models;

namespace ProyectoWeb2.Models
{
    public class DatabaseContext : DbContext
    {
        public DatabaseContext(DbContextOptions<DatabaseContext> options) : base(options)
        {
            
        }
        public DbSet<User> Users { get; set; }
        public DbSet<NewsSource> NewsSources { get; set; }
        public DbSet<News> News { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<Category> Categorys { get; set; }
        public DbSet<Tags> Tags { get; set; }
        
    }
}