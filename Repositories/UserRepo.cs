using System;
using Microsoft.EntityFrameworkCore;
using UsersAPI.Models;

namespace UsersAPI.Repositories
{
    public class UserRepo : DbContext
    {
        public UserRepo(DbContextOptions<UserRepo> options) : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
    }
}
