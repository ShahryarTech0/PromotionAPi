using MediatR;
using Merchants.Application.Responses;
using System;

namespace Merchants.Application.Commands.Promotions.DeletePromotionImageGlobally
{
    public record DeletePromotionImageGloballyCommand(Guid ImageId) : IRequest<Response>;
}
