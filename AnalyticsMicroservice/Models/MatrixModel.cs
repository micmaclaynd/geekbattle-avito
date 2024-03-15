using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AnalyticsMicroservice.Models {
    [Table("matrices")]
    public class MatrixModel : BaseModel {
        [Required]
        [MaxLength(128)]
        [Column("name", TypeName = "nvarchar")]
        public string Name { get; set; }

        [Required]
        [Column("user_id")]
        public uint UserId { get; set; }
    }
}
