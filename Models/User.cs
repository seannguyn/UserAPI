using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.Serialization;
using UsersAPI.Encryption;

namespace UsersAPI.Models
{
    [Table("Users")]
    public class User
    {
        [NotMapped]
        [IgnoreDataMember]
        public static IEncrypt encryptionMethod;

        [Key]
        public int UserId { get; set; }

        [Required]
        [Column(TypeName = "nvarchar(50)")]
        public string Username { get; set; }

        [Required]
        [Column(TypeName = "nvarchar(128)")]
        public string Password { get; set; }

        [Required]
        [Column(TypeName = "nvarchar(100)")]
        public string FirstName { get; set; }

        [Required]
        [Column(TypeName = "nvarchar(100)")]
        public string LastName { get; set; }

        [Column(TypeName = "datetime2")]
        public DateTime? DateOfBirth { get; set; }

        [Required]
        [EmailAddress]
        [Column(TypeName = "nvarchar(256)")]
        public string Email { get; set; }

        [Column(TypeName = "nvarchar(50)")]
        public string Phone { get; set; }

        [Column(TypeName = "nvarchar(50)")]
        public string Mobile { get; set; }

        [IgnoreDataMember]
        public List<UserGroup> UserGroups { get; set; }

        public static void setEncryptionMethod(IEncrypt _encryptionMethod)
        {
            encryptionMethod = _encryptionMethod;
        }

        public static string encryptPassword(string rawPassword)
        {
            return encryptionMethod.encryptPassword(rawPassword);
        }
    }
}
