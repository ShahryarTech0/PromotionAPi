using AutoMapper;
using MediatR;
using Merchants.Application.Commands.Promotions.AddPromotion;
using Merchants.Application.Dto;
using Merchants.Application.Responses;
using Merchants.Core.Common;
using Merchants.Core.Entities;
using Merchants.Core.Interfaces;
using Microsoft.Extensions.Logging;

namespace Merchants.Application.Handlers.Promotions.AddPromotion
{
    public class AddPromotionCommandHandler : IRequestHandler<AddPromotionCommand, Response>
    {
        private readonly IPromotionRepository _promotionRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<AddPromotionCommandHandler> _logger;

        public AddPromotionCommandHandler(IPromotionRepository promotionRepository, IMapper mapper, ILogger<AddPromotionCommandHandler> logger)
        {
            _promotionRepository = promotionRepository;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<Response> Handle(AddPromotionCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("----- Start AddPromotionCommandHandler -----");

            var response = new Response();
            var promotionList = new List<Promotion>();

            try
            {
                // Validate EffectiveFrom date - must not be before today
                var today = DateTime.Today;
                if (request.EffectiveFrom.Date < today)
                {
                    response.isSuccess = false;
                    response.ResponseCode = 0;
                    response.ResponseDescription = "EffectiveFrom date cannot be before today's date.";
                    response.Data = null;
                    return response;
                }

                // Validate EffectiveTo date - must not be less than EffectiveFrom
                if (request.EffectiveTo.Date < request.EffectiveFrom.Date)
                {
                    response.isSuccess = false;
                    response.ResponseCode = 0;
                    response.ResponseDescription = "EffectiveTo date cannot be less than EffectiveFrom date.";
                    response.Data = null;
                    return response;
                }

                // 1. Map request DTO to Promotion entity
                var entity = _mapper.Map<Promotion>(request);
                if (entity == null)
                {
                    response.isSuccess = false;
                    response.ResponseCode = 0;
                    response.ResponseDescription = "Invalid promotion data.";
                    response.Data = null;
                    return response;
                }

                // Ensure entity has an Id
                if (entity.Id == Guid.Empty)
                {
                    entity.Id = Guid.NewGuid();
                }

                // Ensure Images collection exists
                if (entity.Images == null)
                    entity.Images = new List<PromotionImage>();

                // 2. Handle file uploads
                if (request.Files != null && request.Files.Count > 0)
                {
                    var uploadPath = Path.Combine(Directory.GetCurrentDirectory(), "UploadedFiles/Promotions/");
                    if (!Directory.Exists(uploadPath))
                    {
                        Directory.CreateDirectory(uploadPath);
                    }

                    foreach (var file in request.Files)
                    {
                        var fileName = Path.GetFileNameWithoutExtension(file.FileName);
                        var extension = Path.GetExtension(file.FileName);
                        var uniqueFileName = $"{fileName}_{Guid.NewGuid()}{extension}";
                        var filePath = Path.Combine(uploadPath, uniqueFileName);

                        using (var stream = new FileStream(filePath, FileMode.Create))
                        {
                            await file.CopyToAsync(stream);
                        }

                        // Create and associate PromotionImage with the Promotion entity
                        var promotionImage = new PromotionImage
                        {
                            Name = fileName,
                            ImageUrl = "/UploadedFiles/Promotions/" + uniqueFileName,
                            Status = "Active" // explicitly set, optional if default is set in entity
                            // Don't set Promotion navigation property - EF Core will handle it via the collection
                        };

                        entity.Images.Add(promotionImage);
                    }
                }

                _logger.LogInformation("Adding promotion to repository...");
                var addedPromotion = await _promotionRepository.AddPromotionAsync(entity);

                if (addedPromotion == null)
                {
                    response.isSuccess = false;
                    response.ResponseCode = 0;
                    response.ResponseDescription = "Failed to create promotion.";
                    response.Data = null;
                    return response;
                }

                var promotionDto = _mapper.Map<PromotionDto>(addedPromotion);
                // promotionList.Add(addedPromotion);
                //var promotionDtoList = _mapper.Map<List<PromotionDto>>(new List<Promotion> { addedPromotion });
                //response.Data = promotionDtoList;
                //var promotionDtoList = _mapper.Map<List<PromotionDto>>(new List<Promotion> { addedPromotion });

                response.isSuccess = true;
                response.ResponseCode = InternalResponseCode.Success;
                response.ResponseDescription = "Promotion created successfully.";
                response.Data = new List<PromotionDto> { promotionDto };

                _logger.LogInformation("----- End AddPromotionCommandHandler -----");
                return response;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while adding promotion.");

                // Log inner exception for more details
                var errorMessage = ex.Message;
                if (ex.InnerException != null)
                {
                    errorMessage += $" Inner Exception: {ex.InnerException.Message}";
                    _logger.LogError(ex.InnerException, "Inner exception details");
                }

                return new Response
                {
                    isSuccess = false,
                    ResponseCode = 0,
                    ResponseDescription = $"An error occurred while adding the promotion: {errorMessage}",
                    Data = null
                };
            }
        }
    }
}
