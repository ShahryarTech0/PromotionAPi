using AutoMapper;
using Merchants.Application.Commands.Promotions.AddPromotion;
using Merchants.Application.Commands.Promotions.UpdatePromotion;
using Merchants.Application.Dto;
using Merchants.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Merchants.Application.Mapping
{
    public class PromotionMappingProfile : Profile
    {
        public PromotionMappingProfile()
        {
            //CreateMap<AddPromotionCommand, Promotion>();
            //CreateMap<UpdatePromotionCommand, Promotion>();
            //// Map Promotion -> PromotionDto
            ///

            //  CreateMap<AddPromotionCommand, Promotion>()
            //.ForMember(dest => dest.Images, opt => opt.Ignore()); // We'll handle images manually

            //  CreateMap<UpdatePromotionCommand, Promotion>()
            //      .ForMember(dest => dest.Images, opt => opt.Ignore()); // We'll handle images manually


            //  CreateMap<Promotion, PromotionDto>().ReverseMap();
            //  CreateMap<PromotionImage, PromotionImageDto>().ReverseMap();

            // DTO → Entity
            //CreateMap<PromotionDto, Promotion>();
            //CreateMap<PromotionImageDto, PromotionImage>();



            CreateMap<Promotion, PromotionDto>()
               .ForMember(dest => dest.Images, opt => opt.MapFrom(src => src.Images));

            CreateMap<PromotionImage, PromotionImageDto>();

            // DTO/Command -> Entity
            CreateMap<AddPromotionCommand, Promotion>()
                .ForMember(dest => dest.Images, opt => opt.Ignore()); // we'll handle images separately

            CreateMap<UpdatePromotionCommand, Promotion>()
                .ForMember(dest => dest.Images, opt => opt.Ignore()); // handle add/remove separately

        }
    }
}
