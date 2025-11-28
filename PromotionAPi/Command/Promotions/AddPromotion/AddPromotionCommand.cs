using MediatR;
using Merchants.Application.Responses;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Merchants.Application.Commands.Promotions.AddPromotion
{
    public record AddPromotionCommand(string Name, string Description, DateTime EffectiveFrom, DateTime EffectiveTo, string Status, List<IFormFile>? Files) : IRequest<Response>;
    // IFormFile? ImageFile
}
