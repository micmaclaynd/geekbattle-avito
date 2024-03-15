using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AnalyticsMicroservice.Models {
    [Table("prices")]
    public class PriceModel : BaseModel {
        [Required]
        [Column("category_id")]
        [ForeignKey(nameof(CategoryModel))]
        public uint CategoryId { get; set; }

        [Required]
        [Column("location_id")]
        [ForeignKey(nameof(LocationModel))]
        public uint LocationId { get; set; }

        [Column("price")]
        public double? Price { get; set; } = null;

        [Required]
        [Column("matrix_id")]
        [ForeignKey(nameof(MatrixModel))]
        public uint MatrixId { get; set; }
    }
}
