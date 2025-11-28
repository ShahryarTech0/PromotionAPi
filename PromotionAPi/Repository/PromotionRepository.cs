using Merchants.Core.Entities;
using Merchants.Core.Interfaces;
using Merchants.Infrastructure.Data;
using Microsoft.Extensions.Logging;
using Merchants.Infrastructure.Extentions;
using Merchants.Application.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Merchants.Infrastructure.Repositories
{
    public class PromotionRepository : IPromotionRepository
    {
        private readonly MerchantContext _PromotionContext;
        private readonly ILogger<PromotionRepository> _logger;

        public PromotionRepository(MerchantContext merchantContext, ILogger<PromotionRepository> logger)
        {
            _PromotionContext = merchantContext;
            _logger = logger;
        }
        //public async Task<PromotionImage?> GetPromotionImageByID(Guid id)
        //{
        //    return await _PromotionContext.PromotionImages.FirstOrDefaultAsync(l => l.Id == id);
        //}
        //public async Task<PromotionImage> RemovePromotionImageAsync(PromotionImage image)
        //{
        //    // Attach if not tracked
        //    var trackedImage = await _PromotionContext.PromotionImages
        //        .FirstOrDefaultAsync(x => x.Id == image.Id);

        //    if (trackedImage == null)
        //    {
        //        // Row does not exist in DB
        //        throw new InvalidOperationException("PromotionImage not found in database.");
        //    }

        //    trackedImage.isDeleted = true;
        //    trackedImage.DeletedAt = DateTime.UtcNow;
        //    trackedImage.Status = "InActive";

        //    await _PromotionContext.SaveChangesAsync();
        //    return trackedImage;
        //}

        //public async Task<PromotionImage> AddPromotionImageAsync(PromotionImage image)
        //{
        //    await _PromotionContext.PromotionImages.AddAsync(image);
        //    await _PromotionContext.SaveChangesAsync();
        //    return image;
        //}

        public async Task<Promotion?> GetPromotionByID(Guid id)
        {
            return await _PromotionContext.Promotions
                .Include(p => p.Images)
                .FirstOrDefaultAsync(l => l.Id == id);
        }
        public async Task<Promotion> AddPromotionAsync(Promotion entity)
        {
            await _PromotionContext.Promotions.AddAsync(entity);
            await _PromotionContext.SaveChangesAsync();
            return entity;
        }
        public async Task<Promotion?> DeletePromotionAsync(Promotion entity)
        {
            entity.isDeleted = true;
            entity.DeletedAt = DateTime.UtcNow;
            entity.Status = "InActive";
            _PromotionContext.Promotions.Update(entity);
            await _PromotionContext.SaveChangesAsync();
            return entity;
        }
        public async Task<Promotion> UpdatePromotionAsync(Promotion entity)
        {
            _PromotionContext.Promotions.Update(entity);
            await _PromotionContext.SaveChangesAsync();
            return entity;
        }

        public async Task<IEnumerable<Promotion>> GetAllAsync()
        {
            return await _PromotionContext.Promotions.Include(p => p.Images).ToListAsync();
        }

        public async Task AddPromotionImagesAsync(IEnumerable<PromotionImage> images)
        {
            await _PromotionContext.PromotionImages.AddRangeAsync(images);
            await _PromotionContext.SaveChangesAsync();
        }

        public async Task<PromotionImage?> GetPromotionImageByIdAsync(Guid imageId)
        {
            return await _PromotionContext.PromotionImages
                .FirstOrDefaultAsync(i => i.Id == imageId && !i.isDeleted);
        }

        public async Task<List<PromotionImage>> GetPromotionImagesByImageIdAsync(Guid imageId)
        {
            // Get the image first to find its ImageUrl
            var image = await GetPromotionImageByIdAsync(imageId);
            if (image == null)
                return new List<PromotionImage>();

            // Find all instances with the same ImageUrl (same physical file used in multiple promotions)
            return await _PromotionContext.PromotionImages
                .Where(i => i.ImageUrl == image.ImageUrl && !i.isDeleted)
                .ToListAsync();
        }

        public async Task SoftDeletePromotionImageAsync(PromotionImage image)
        {
            image.isDeleted = true;
            image.DeletedAt = DateTime.UtcNow;
            image.Status = "InActive";
            _PromotionContext.PromotionImages.Update(image);
            await _PromotionContext.SaveChangesAsync();
        }

        public async Task<List<Promotion>> GetPromotionsByImageIdAsync(Guid imageId)
        {
            var image = await GetPromotionImageByIdAsync(imageId);
            if (image == null)
                return new List<Promotion>();

            // Get all promotions that have this image (by ImageUrl to handle duplicates)
            return await _PromotionContext.Promotions
                .Include(p => p.Images)
                .Where(p => p.Images.Any(i => i.ImageUrl == image.ImageUrl && !i.isDeleted))
                .ToListAsync();
        }

        //public async Task<IEnumerable<PromotionImage>> GetAllPromotionImgAsync()
        //{
        //    return await _PromotionContext.PromotionImages.ToListAsync();
        //}
    }
}
