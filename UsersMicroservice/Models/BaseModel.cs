using System.ComponentModel.DataAnnotations;

namespace UsersMicroservice.Models {
    public class BaseModel {
        [Key]
        [Required]
        public uint Id { get; set; }
    }
}