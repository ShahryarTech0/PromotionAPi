//using AutoMapper;
//using MediatR;
//using Merchants.Application.Commands.PromotionImages.DeletePromotionImage;
//using Merchants.Application.Responses;
//using Merchants.Core.Common;
//using Merchants.Core.Entities;
//using Merchants.Core.Interfaces;
//using Microsoft.Extensions.Logging;
//using System.IO;
//using System.Linq;
//using System.Threading;
//using System.Threading.Tasks;

//namespace Merchants.Application.Handlers.PromotionImages.DeletePromotionImage
//{
//    public class DeletePromotionImageCommandHandler
//        : IRequestHandler<DeletePromotionImageCommand, Response>
//    {
//        private readonly IPromotionRepository _promotionRepository;
//        private readonly IMapper _mapper;
//        private readonly ILogger<DeletePromotionImageCommandHandler> _logger;

//        public DeletePromotionImageCommandHandler(
//            IPromotionRepository promotionRepository,
//            IMapper mapper,
//            ILogger<DeletePromotionImageCommandHandler> logger)
//        {
//            _promotionRepository = promotionRepository;
//            _mapper = mapper;
//            _logger = logger;
//        }

//        public async Task<Response> Handle(DeletePromotionImageCommand request, CancellationToken cancellationToken)
//        {

//            var promotion = await _promotionRepository.GetPromotionByID(request.PromotionId);

//            if (promotion == null)
//            {
//                return new Response
//                {
//                    isSuccess = false,
//                    ResponseCode = 0,
//                    ResponseDescription = "Promotion not found"
//                };
//            }

//            var image = promotion.Images.FirstOrDefault(i => i.Id == request.ImageId);

//            if (image == null)
//            {
//                return new Response
//                {
//                    isSuccess = false,
//                    ResponseCode = 0,
//                    ResponseDescription = "Image not found in this promotion"
//                };
//            }

//            // 1️⃣ Remove physical file
//            if (!string.IsNullOrEmpty(image.ImageUrl))
//            {
//                var filePath = Path.Combine(Directory.GetCurrentDirectory(), image.ImageUrl.TrimStart('/').Replace("/", "\\"));
//                if (File.Exists(filePath))
//                {
//                    File.Delete(filePath);
//                }
//            }

//            // 2️⃣ Remove from Promotion.Images collection
//            promotion.Images.Remove(image);

//            // 3️⃣ Remove from database
//            await _promotionRepository.RemovePromotionImageAsync(image);

//            // 4️⃣ Update promotion if needed
//            await _promotionRepository.UpdatePromotionAsync(promotion);

//            return new Response
//            {
//                isSuccess = true,
//                ResponseCode = InternalResponseCode.Success,
//                ResponseDescription = "Promotion image deleted successfully"
//            };
//        }
//    }
//}
