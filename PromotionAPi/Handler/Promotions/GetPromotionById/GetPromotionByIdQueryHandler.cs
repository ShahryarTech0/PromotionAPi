using AutoMapper;
using MediatR;
using Merchants.Application.Dto;
using Merchants.Application.Handlers.Promotions.AddPromotion;
using Merchants.Application.Queries.Promotions.GetPromotionById;
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

namespace Merchants.Application.Handlers.Promotions.GetPromotionById
{
    public class GetPromotionByIdQueryHandler : IRequestHandler<GetPromotionByIdQuery, Response>
    {
        private readonly IPromotionRepository _promotionRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<GetPromotionByIdQueryHandler> _logger;

        public GetPromotionByIdQueryHandler(IPromotionRepository promotionRepository, IMapper mapper, ILogger<GetPromotionByIdQueryHandler> logger)
        {
            _promotionRepository = promotionRepository;
            _mapper = mapper;
            _logger = logger;
        }
        public async Task<Response> Handle(GetPromotionByIdQuery request, CancellationToken cancellationToken)
        {
            Response response = new Response();
           // var promotionList = new List<Promotion>();

            try
            {
                
                var datafromdb = await _promotionRepository.GetPromotionByID(request.Id);

                if (datafromdb == null)
                {
                    response.isSuccess = false;
                    response.ResponseCode = 0;
                    response.ResponseDescription = "Id Not Found.";
                    response.Data = null;
                    return response;
                }

                _logger.LogInformation("Prootion fetched successfully for ID {PromotionId}", request.Id);

                var promotionDto = _mapper.Map<PromotionDto>(datafromdb);


                response.isSuccess = true;
                response.ResponseCode = InternalResponseCode.Success;
                response.ResponseDescription = "Promotion successfull.";
                response.Data = promotionDto;

                _logger.LogInformation("----- End GetPromotionbyIdQueryHandler -----");
                return response;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching Promotion for  ID {PromotionID}",request.Id);
                response.isSuccess = false;
                response.ResponseCode = 0;
                response.ResponseDescription = "Invalid Id.";
                response.Data = null;
                return response;
            }
        }
    }
}
