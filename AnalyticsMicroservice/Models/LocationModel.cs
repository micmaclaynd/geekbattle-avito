using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AnalyticsMicroservice.Models {
    [Table("locations")]
    public class LocationModel : BaseModel {
        [Required]
        [MaxLength(128)]
        [Column("name", TypeName = "nvarchar")]
        public string Name { get; set; }

        [Column("parent_id")]
        [ForeignKey(nameof(LocationModel))]
        public uint? ParentId { get; set; } = null;

        [Required]
        [Column("user_id")]
        public uint UserId { get; set; }
    }
}
