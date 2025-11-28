using MediatR;
using Merchants.Application.Responses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Merchants.Application.Queries.Promotions.GetPromotionById
{
    public record GetPromotionByIdQuery(Guid Id) : IRequest<Response>;
}
