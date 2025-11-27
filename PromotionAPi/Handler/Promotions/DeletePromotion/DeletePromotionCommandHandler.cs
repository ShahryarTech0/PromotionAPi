using AutoMapper;
using MediatR;
using Merchants.Application.Commands.Promotions.DeletePromotion;
using Merchants.Application.Handlers.Promotions.GetPromotionById;
using Merchants.Application.Responses;
using Merchants.Core.Common;
using Merchants.Core.Interfaces;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Merchants.Application.Handlers.Promotions.DeletePromotion
{
    public class DeletePromotionCommandHandler : IRequestHandler<DeletePromotionCommand, Response>
    {
        private readonly IPromotionRepository _promotionRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<DeletePromotionCommandHandler> _logger;

        public DeletePromotionCommandHandler(IPromotionRepository promotionRepository, IMapper mapper, ILogger<DeletePromotionCommandHandler> logger)
        {
            _promotionRepository = promotionRepository;
            _mapper = mapper;
            _logger = logger;
        }
        public async Task<Response> Handle(DeletePromotionCommand request, CancellationToken cancellationToken)
        {
            Response response = new Response();
            try
            {
                var result = await _promotionRepository.GetPromotionByID(request.id);
                if (result == null)
                {
                    _logger.LogWarning("Promotion not found for deletion. Id={Id}", request.id);
                    response.isSuccess = false;
                    response.ResponseCode = 0;
                    response.ResponseDescription = "Invalid Id.";
                    response.Data = null;
                    return response;
                }

                // Option A: Hard delete
                await _promotionRepository.DeletePromotionAsync(result);

                // Option B: Soft delete
                // await _merchantLocationRepository.SoftDeleteAsync(result);

                _logger.LogInformation("Promotion with ID {Id} deleted successfully.", request.id);
                response.isSuccess = true;
                response.ResponseCode = InternalResponseCode.Success;
                response.ResponseDescription = "Promotion Deleted successfully.";
                response.Data = null;
                return response;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while deleting Promoton with ID {Id}", request.id);
                response.isSuccess = false;
                response.ResponseCode = 0;
                response.ResponseDescription = "Invalid Id.";
                response.Data = null;
                return response;
            }

        }
    }
}
