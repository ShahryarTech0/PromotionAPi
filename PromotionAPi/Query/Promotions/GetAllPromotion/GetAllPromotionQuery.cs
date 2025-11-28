using MediatR;
using Merchants.Application.Responses;
using Merchants.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Google.Apis.Requests.BatchRequest;

namespace Merchants.Application.Queries.Promotions.GetAllPromotion
{
    public record GetAllPromotionQuery()
          : IRequest<Response>;
}


