using MediatR;
using Merchants.Application.Commands.Promotions.DeletePromotionImageGlobally;
using Merchants.Application.Responses;
using Merchants.Core.Common;
using Merchants.Core.Interfaces;
using Microsoft.Extensions.Logging;

namespace Merchants.Application.Handlers.Promotions.DeletePromotionImageGlobally
{
    public class DeletePromotionImageGloballyCommandHandler
        : IRequestHandler<DeletePromotionImageGloballyCommand, Response>
    {
        private readonly IPromotionRepository _promotionRepository;
        private readonly ILogger<DeletePromotionImageGloballyCommandHandler> _logger;

        public DeletePromotionImageGloballyCommandHandler(
            IPromotionRepository promotionRepository,
            ILogger<DeletePromotionImageGloballyCommandHandler> logger)
        {
            _promotionRepository = promotionRepository;
            _logger = logger;
        }

        public async Task<Response> Handle(DeletePromotionImageGloballyCommand request, CancellationToken cancellationToken)
        {
            try
            {
                // 1. Get the image to find its ImageUrl
                var image = await _promotionRepository.GetPromotionImageByIdAsync(request.ImageId);
                if (image == null)
                {
                    return new Response
                    {
                        isSuccess = false,
                        ResponseCode = 0,
                        ResponseDescription = "Image not found",
                        Data = null
                    };
                }

                // 2. Get all instances of this image (same ImageUrl) across all promotions
                var allImageInstances = await _promotionRepository.GetPromotionImagesByImageIdAsync(request.ImageId);

                if (!allImageInstances.Any())
                {
                    return new Response
                    {
                        isSuccess = false,
                        ResponseCode = 0,
                        ResponseDescription = "No image instances found",
                        Data = null
                    };
                }

                // 3. Get count of affected promotions BEFORE deletion
                var affectedPromotions = await _promotionRepository.GetPromotionsByImageIdAsync(request.ImageId);
                var promotionCount = affectedPromotions.Count;

                // 4. Soft delete all instances of this image from all promotions
                foreach (var img in allImageInstances)
                {
                    await _promotionRepository.SoftDeletePromotionImageAsync(img);
                }

                _logger.LogInformation($"Image {request.ImageId} deleted globally from {allImageInstances.Count} instance(s) across {promotionCount} promotion(s)");

                return new Response
                {
                    isSuccess = true,
                    ResponseCode = InternalResponseCode.Success,
                    ResponseDescription = $"Image deleted successfully from all promotions",
                    Data = null
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error deleting image {request.ImageId} globally");
                return new Response
                {
                    isSuccess = false,
                    ResponseCode = 0,
                    ResponseDescription = $"Error: {ex.Message}",
                    Data = null
                };
            }
        }
    }
}
