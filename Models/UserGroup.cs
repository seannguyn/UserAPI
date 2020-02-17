using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace UsersAPI.Models
{
    [Table("UserGroups")]
    public class UserGroup
    {
        public static async Task<UserGroup> buildUserGroup(int userId, DbSet<User> Users, int groupId, DbSet<Group> Groups)
        {
            UserGroup ug = new UserGroup();
            //ug.UserId = userId;
            ug.User = await Users.FirstOrDefaultAsync(x => x.UserId == userId);

            //ug.GroupID = groupId;
            ug.Group = await Groups.FirstOrDefaultAsync(x => x.GroupID == groupId);

            return ug;
        }

        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        
        public int UserId { get; set; }
        public User User { get; set; }

        public int GroupID { get; set; }
        public Group Group { get; set; }
    }
}
