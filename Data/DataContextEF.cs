using DotNetAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace DotNetAPI.Data
{
    public class DataContextEF: DbContext
    {
        private readonly IConfiguration _config;
        public DataContextEF(IConfiguration config){
            _config = config;
        }

        public virtual DbSet<User> Users {get; set;}
        public virtual DbSet<UserSalary> UserSalary {get; set;}
        public virtual DbSet<UserJobInfo> UserJobInfo {get; set;}

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if(!optionsBuilder.IsConfigured){
                optionsBuilder.UseSqlServer(_config.GetConnectionString("DefaultConnectionString"),
                optionsBuilder => optionsBuilder.EnableRetryOnFailure());
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            /* Tells EF what schema DB has and changes User model to link 
            with users table. Also specifies that userID is the primary key*/
            modelBuilder.HasDefaultSchema("TutorialAppSchema");
            modelBuilder.Entity<User>()
                .ToTable("Users", "TutorialAppSchema")
                .HasKey(u => u.UserId);
            
             modelBuilder.Entity<UserSalary>()
                .HasKey(u => u.UserId);

             modelBuilder.Entity<UserJobInfo>()
                .HasKey(u => u.UserId);
        }
    }
}