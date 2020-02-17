using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace UsersAPI.Models
{
    [Table("Groups")]
    public class Group
    {
        [Key]
        public int GroupID { get; set; }

        [Required]
        [Column(TypeName = "nvarchar(50)")]
        public string GroupName { get; set; }

        [Column(TypeName = "nvarchar(256)")]
        public string Description { get; set; }

        [IgnoreDataMember]
        public List<UserGroup> UserGroups { get; set; }
    }
}
