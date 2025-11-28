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

                // Validate EffectiveFrom date - must not be before today
                var today = DateTime.Today;
                if (request.EffectiveFrom.Date < today)
                {
                    response.isSuccess = false;
                    response.ResponseCode = 0;
                    response.ResponseDescription = "EffectiveFrom date cannot be before today's date.";
                    return response;
                }

                // Validate EffectiveTo date - must not be less than EffectiveFrom
                if (request.EffectiveTo.Date < request.EffectiveFrom.Date)
                {
                    response.isSuccess = false;
                    response.ResponseCode = 0;
                    response.ResponseDescription = "EffectiveTo date cannot be less than EffectiveFrom date.";
                    return response;
                }

                // Preserve existing images before mapping (to prevent any potential clearing)
                var existingImages = existing.Images?.ToList() ?? new List<PromotionImage>();

                // Update basic fields
                _mapper.Map(request, existing);

                // Ensure Images collection exists and restore existing images if needed
                if (existing.Images == null)
                    existing.Images = new List<PromotionImage>();

                // Restore existing images if they were cleared by mapping
                foreach (var img in existingImages)
                {
                    if (!existing.Images.Any(x => x.Id == img.Id))
                    {
                        existing.Images.Add(img);
                    }
                }

                // -------------------------------
                // REMOVE IMAGES
                // -------------------------------
                if (request.RemImages != null && request.RemImages.Count > 0)
                {
                    foreach (var imageId in request.RemImages)
                    {
                        var img = existing.Images.FirstOrDefault(x => x.Id == imageId);
                        if (img != null)
                        {
                            existing.Images.Remove(img);
                        }
                    }
                }

                // -------------------------------
                // ADD NEW IMAGES
                // -------------------------------
                // When RemImages is null and new images are provided, add them to existing images
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
                            Promotion = existing
                        };

                        // Add new image to existing collection (preserving all existing images)
                        existing.Images.Add(newImage);
                    }
                }

                // -------------------------------
                // SAVE CHANGES
                // -------------------------------
                var result = await _promotionRepository.UpdatePromotionAsync(existing);

                //promotionList.Add(result);
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