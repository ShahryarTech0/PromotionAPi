//using MediatR;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using Merchants.Application.Responses;
//namespace Merchants.Application.Commands.PromotionImages.DeletePromotionImage
//{
//    public record DeletePromotionImageCommand : IRequest<Response>
//    {
//        public Guid? PromotionId { get; set; } // Optional: Promotion from which image will be removed. If null, deletes from all promotions
//        public Guid ImageId { get; set; }      // Image to delete
//    }

//}
