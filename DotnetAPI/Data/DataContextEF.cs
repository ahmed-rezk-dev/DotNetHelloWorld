using DotNet.Models;
using DotNetAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace DotNetAPI.Data
{
    public class DataContextEF : DbContext
    {
        private readonly IConfiguration _config;

        public DataContextEF(IConfiguration config)
        {
            _config = config;
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer(
                    _config.GetConnectionString("DefaultConnection"),
                    optionsBuilder => optionsBuilder.EnableRetryOnFailure()
                );
            }
        }

        public virtual DbSet<User> Users { get; set; }
        public virtual DbSet<UsersJobInfo> UsersJobInfo { get; set; }
        public virtual DbSet<UserSalary> UserSalary { get; set; }
        public virtual DbSet<UserAuth> UserAuth { get; set; }
        public virtual DbSet<Post> Post { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder
                .HasDefaultSchema("TutorialAppSchema")
                .Entity<User>()
                .ToTable("Users", "TutorialAppSchema")
                .HasKey(u => u.UserId);

            modelBuilder.Entity<UsersJobInfo>().HasKey(u => u.UserId);
            modelBuilder.Entity<UserSalary>().HasKey(u => u.UserId);
            modelBuilder
                .HasDefaultSchema("TutorialAppSchema")
                .Entity<UserAuth>()
                .ToTable("Auth", "TutorialAppSchema")
                .HasNoKey();

            modelBuilder
                .HasDefaultSchema("TutorialAppSchema")
                .Entity<Post>()
                .ToTable("Posts", "TutorialAppSchema")
                .HasKey(u => u.PostId);
        }
    }
}
