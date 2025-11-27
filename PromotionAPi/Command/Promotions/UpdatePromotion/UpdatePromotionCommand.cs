using MediatR;
using Merchants.Application.Responses;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace Merchants.Application.Commands.Promotions.UpdatePromotion
{
    public record UpdatePromotionCommand(Guid Id,string Name, string Description, DateTime EffectiveFrom, DateTime EffectiveTo, string Status, List<IFormFile>? Images, List<Guid>? RemImages) : IRequest<Response>;

    //, 
}
