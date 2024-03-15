using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AnalyticsMicroservice.Models {
    public class BaseModel {
        [Key]
        [Required]
        [Column("id")]
        public uint Id { get; set; }
    }
}
