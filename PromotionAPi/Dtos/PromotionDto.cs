using Merchants.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Merchants.Application.Dto
{
    //public class PromotionDto : FileBaseDTO
    //{

    //    public string Description { get; set; }
    //    public DateTime EffectiveFrom { get; set; }
    //    public DateTime EffectiveTo { get; set; }

    //    //public ICollection<PromotionImage> Images { get; set; } this i created in FileBaseDto

    //}

    public class PromotionDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }

        public DateTime EffectiveFrom { get; set; }
        public DateTime EffectiveTo { get; set; }

        public List<PromotionImageDto> Images { get; set; }
    }

}

