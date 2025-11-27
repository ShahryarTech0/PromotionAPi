using AutoMapper;
using MediatR;
using Merchants.Application.Commands.Promotions.UpdatePromotion;
using Merchants.Application.Dto;
using Merchants.Application.Responses;
using Merchants.Core.Common;
using Merchants.Core.Entities;
using Merchants.Core.Interfaces;
using Microsoft.Extensions.Logging;

namespace Merchants.Application.Handlers.Promotions.UpdatePromotion
{
    public class UpdatePromotionCommandHandler : IRequestHandler<UpdatePromotionCommand, Response>
    {
        private readonly IPromotionRepository _promotionRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<UpdatePromotionCommandHandler> _logger;

        public UpdatePromotionCommandHandler(IPromotionRepository promotionRepository, IMapper mapper, ILogger<UpdatePromotionCommandHandler> logger)
        {
            _promotionRepository = promotionRepository;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<Response> Handle(UpdatePromotionCommand request, CancellationToken cancellationToken)
        {
            Response response = new Response();
            var promotionList = new List<Promotion>();

            try
            {
                // Load existing promotion from DB
                var existing = await _promotionRepository.GetPromotionByID(request.Id);
                if (existing == null)
                {
                    response.isSuccess = false;
                    response.ResponseCode = 0;
                    response.ResponseDescription = "Invalid promotion Id.";
                    return response;
                }

                // Update basic fields
                _mapper.Map(request, existing);

                // Ensure Images collection exists
                if (existing.Images == null)
                    existing.Images = new List<PromotionImage>();


                var updatedImages = new List<PromotionImage>();
                // -------------------------------
                // REMOVE IMAGES
                // -------------------------------
                // If RemImages is null but new images are provided, replace all images (soft delete all)
                if (request.RemImages == null && request.Images != null && request.Images.Count > 0)
                {
                    // Soft delete all existing images of this promotion
                    foreach (var img in existing.Images.Where(x => !x.isDeleted).ToList())
                    {
                        await _promotionRepository.SoftDeletePromotionImageAsync(img);
                    }
                }
                // If RemImages has specific IDs, soft delete only those
                else if (request.RemImages != null && request.RemImages.Count > 0)
                {
                    foreach (var imageId in request.RemImages)
                    {
                        var img = existing.Images.FirstOrDefault(x => x.Id == imageId && !x.isDeleted);
                        if (img != null)
                        {
                            await _promotionRepository.SoftDeletePromotionImageAsync(img);
                        }
                    }
                }

                // -------------------------------
                // ADD NEW IMAGES
                // -------------------------------
                if (request.Images != null && request.Images.Count > 0)
                {
                    foreach (var file in request.Images)
                    {
                        var fileName = Path.GetFileNameWithoutExtension(file.FileName);
                        var extension = Path.GetExtension(file.FileName);
                        var uniqueFileName = $"{fileName}_{Guid.NewGuid()}{extension}";
                        var uploadPath = Path.Combine(Directory.GetCurrentDirectory(), "UploadedFiles/Promotions/");

                        if (!Directory.Exists(uploadPath))
                            Directory.CreateDirectory(uploadPath);

                        var filePath = Path.Combine(uploadPath, uniqueFileName);

                        using (var stream = new FileStream(filePath, FileMode.Create))
                        {
                            await file.CopyToAsync(stream);
                        }

                        var newImage = new PromotionImage()
                        {
                            Name = fileName,
                            ImageUrl = "/UploadedFiles/Promotions/" + uniqueFileName,
                            Status = "Active"
                            // Don't set Promotion navigation property - EF Core will handle it via the collection
                        };

                        existing.Images.Add(newImage);
                        updatedImages.Add(newImage);
                    }
                }

                // -------------------------------
                // SAVE CHANGES
                // -------------------------------
                await _promotionRepository.UpdatePromotionAsync(existing);

                // Reload from database to get only non-deleted images (query filter will exclude soft-deleted)
                var result = await _promotionRepository.GetPromotionByID(request.Id);

                if (result == null)
                {
                    response.isSuccess = false;
                    response.ResponseCode = 0;
                    response.ResponseDescription = "Promotion not found after update.";
                    return response;
                }

                // Show only newly added images in response
                if (updatedImages.Any() && result.Images != null)
                {
                    // Get ImageUrls of newly added images (match by ImageUrl since IDs are generated after save)
                    var newImageUrls = updatedImages.Select(x => x.ImageUrl).ToList();
                    
                    // Filter result to show only newly added images by matching ImageUrl
                    result.Images = result.Images.Where(x => newImageUrls.Contains(x.ImageUrl) && !x.isDeleted).ToList();
                }
                else if (result.Images != null)
                {
                    // If no new images were added, show empty images list
                    result.Images = new List<PromotionImage>();
                }

                var dto = _mapper.Map<PromotionDto>(result);

                response.isSuccess = true;
                response.ResponseCode = InternalResponseCode.Success;
                response.ResponseDescription = "Promotion updated successfully.";
                response.Data = new List<PromotionDto> { dto };


                return response;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating promotion");
                response.isSuccess = false;
                response.ResponseDescription = ex.Message;
                return response;
            }
        }
    }
}