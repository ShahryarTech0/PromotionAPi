using AutoMapper;
using MediatR;
using Merchants.Application.Dto;
using Merchants.Application.Handlers.Promotions.GetPromotionById;
using Merchants.Application.Queries.Promotions.GetAllPromotion;
using Merchants.Application.Responses;
using Merchants.Core.Common;
using Merchants.Core.Entities;
using Merchants.Core.Interfaces;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Google.Apis.Requests.BatchRequest;

namespace Merchants.Application.Handlers.Promotions.GetAllPromotion
{
    public class GetAllPromotionQueryHandler
         : IRequestHandler<GetAllPromotionQuery, Response>
    {
        private readonly IPromotionRepository _promotionRepository;
        private readonly ILogger<GetAllPromotionQueryHandler> _logger;
        private readonly IMapper _mapper;

        public GetAllPromotionQueryHandler(
            IPromotionRepository promotionRepository,
            ILogger<GetAllPromotionQueryHandler> logger,
            IMapper mapper)
        {
            _promotionRepository = promotionRepository;
            _logger = logger;
            _mapper = mapper;
        }

        public async Task<Response> Handle(GetAllPromotionQuery request, CancellationToken cancellationToken)
        {
            var response = new Response();
            var promotionList = new List<Promotion>();

            try
            {
                _logger.LogInformation("Fetching all promotions from repository...");
                var dataFromDb = await _promotionRepository.GetAllAsync();

                if (dataFromDb == null)
                {
                    response.isSuccess = true;
                    response.ResponseCode = InternalResponseCode.Success;
                    response.ResponseDescription = "No promotions found.";
                    response.Data = null;
                    return response;
                }


                var promotionDtoList = _mapper.Map<List<PromotionDto>>(dataFromDb);


                response.isSuccess = true;
                response.ResponseCode = InternalResponseCode.Success;
                response.ResponseDescription = "Promotions retrieved successfully.";
                response.Data = promotionDtoList;

                _logger.LogInformation("Promotions retrieved successfully.");
                return response;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching promotions.");
                response.isSuccess = false;
                response.ResponseCode = 0;
                response.ResponseDescription = $"An error occurred while fetching promotions: {ex.Message}";
                response.Data = null;
                return response;
            }
        }
    }
}
