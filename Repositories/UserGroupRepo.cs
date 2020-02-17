using System;
using Microsoft.EntityFrameworkCore;
using UsersAPI.Models;

namespace UsersAPI.Repositories
{
    public class UserGroupRepo : DbContext
    {
        public UserGroupRepo(DbContextOptions<UserGroupRepo> options) : base(options)
        { }

        public DbSet<User> Users { get; set; }
        public DbSet<Group> Groups { get; set; }
        public DbSet<UserGroup> UserGroups { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<UserGroup>()
                .HasKey(t => new { t.UserId, t.GroupID });

            modelBuilder.Entity<UserGroup>()
                .HasOne(pt => pt.User)
                .WithMany(p => p.UserGroups)
                .HasForeignKey(pt => pt.UserId);

            modelBuilder.Entity<UserGroup>()
                .HasOne(pt => pt.Group)
                .WithMany(t => t.UserGroups)
                .HasForeignKey(pt => pt.GroupID);
        }
    }
}
