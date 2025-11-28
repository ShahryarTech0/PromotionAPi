using Merchants.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace Merchants.Core.Interfaces
{
    public interface  IPromotionRepository

    {
        
        //Task<PromotionImage> GetPromotionImageByID(Guid id);
        //Task<PromotionImage> RemovePromotionImageAsync(PromotionImage image);
        //Task<PromotionImage> AddPromotionImageAsync(PromotionImage image);
        Task<Promotion> AddPromotionAsync(Promotion entity);

        Task<Promotion> GetPromotionByID(Guid id);

        Task<Promotion> UpdatePromotionAsync(Promotion entity);

        Task<Promotion> DeletePromotionAsync(Promotion entity);

        Task<IEnumerable<Promotion>> GetAllAsync();

        Task AddPromotionImagesAsync(IEnumerable<PromotionImage> images);
        
        // Image deletion methods
        Task<PromotionImage?> GetPromotionImageByIdAsync(Guid imageId);
        Task<List<PromotionImage>> GetPromotionImagesByImageIdAsync(Guid imageId);
        Task SoftDeletePromotionImageAsync(PromotionImage image);
        Task<List<Promotion>> GetPromotionsByImageIdAsync(Guid imageId);
        //Task<IEnumerable<PromotionImage>> GetAllPromotionImgAsync();
    }
}
