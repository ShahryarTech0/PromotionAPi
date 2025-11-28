using Merchants.Core.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks; 
using System.Text.Json.Serialization;
namespace Merchants.Core.Entities
{
    [Table("tblPromotions")]
    public class Promotion : BaseEntity
    {
        [MaxLength(50)]
        public string Name { get; set; }
        public string Description { get; set; }
        [Required]
        public DateTime EffectiveFrom { get; set; }
        [Required]
        public DateTime EffectiveTo { get; set; }
        public string? Status { get; set; } // New property

        public ICollection<PromotionImage> Images { get; set; }
    }

    [Table("tblPromotionImages")]
    public class PromotionImage : FileBase
    {
        public Guid PromotionId { get; set; }
        [JsonIgnore]  // Prevent serialization cycle
        public Promotion? Promotion { get; set; }
        public string? Status { get; set; } = "Active"; // Default value
        public bool isDeleted { get; set; } = false;
    }
}
