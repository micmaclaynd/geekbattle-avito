using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AnalyticsMicroservice.Models {
    [Table("categories")]
    public class CategoryModel : BaseModel {
        [Required]
        [MaxLength(128)]
        [Column("name", TypeName = "nvarchar")]
        public string Name { get; set; }

        [Column("parent_id")]
        [ForeignKey(nameof(CategoryModel))]
        public uint? ParentId { get; set; } = null;

        [Required]
        [Column("user_id")]
        public uint UserId { get; set; }
    }
}
