using System;
using Microsoft.EntityFrameworkCore;
using UsersAPI.Models;

namespace UsersAPI.Repositories
{
    public class GroupRepo : DbContext
    {
        public GroupRepo(DbContextOptions<GroupRepo> options) : base(options)
        {
        }

        public DbSet<Group> Groups { get; set; }
    }
}
