using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace UsersMicroservice.Models {
    [Table("users")]
    [Index(nameof(Username), IsUnique = true)]
    public class UserModel : BaseModel {
        [Required]
        [Column("username", TypeName = "nvarchar(32)")]
        public string Username { get; set; }

        [Required]
        [Column("password", TypeName = "nvarchar(256)")]
        public string Password { get; set; }
    }
}