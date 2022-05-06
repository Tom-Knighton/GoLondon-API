using System;
using GoLondonAPI.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace GoLondonAPI.Data
{
    public class DataContext : DbContext
    {
        protected readonly IConfiguration _config;

        public DataContext(IConfiguration config)
        {
            _config = config;
        }

        protected override void OnConfiguring(DbContextOptionsBuilder builder)
        {
            var connectString = _config.GetConnectionString("Database");
            builder.UseMySql(connectString, ServerVersion.AutoDetect(connectString));
        }


        public DbSet<User> Users { get; set; }


        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<User>(user =>
            {
                user.HasKey(u => u.UserUUID);

                user
                    .HasMany(u => u.Projects)
                    .WithOne(p => p.User)
                    .HasForeignKey(p => p.UserUUID)
                    .OnDelete(DeleteBehavior.Cascade)
                    .IsRequired();

                user
                    .HasOne(u => u.Role)
                    .WithOne(r => r.User)
                    .HasForeignKey<UserRole>(ur => ur.UserUUID)
                    .OnDelete(DeleteBehavior.Cascade)
                    .IsRequired();
            });

            builder.Entity<Project>(project =>
            {
                project.HasKey(p => p.ProjectId);
            });

            builder.Entity<Role>(role =>
            {
                role.Property(r => r.RoleId).ValueGeneratedOnAdd();
                role.HasKey(r => r.RoleId);

                role
                    .HasMany(r => r.Users)
                    .WithOne(ur => ur.Role)
                    .HasForeignKey(ur => ur.RoleId)
                    .OnDelete(DeleteBehavior.Cascade)
                    .IsRequired();
            });

            builder.Entity<UserRole>(userRole =>
            {
                userRole.HasKey(ur => new { ur.UserUUID, ur.RoleId });
            });
                
        }
    }
}

