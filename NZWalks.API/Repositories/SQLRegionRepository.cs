using Microsoft.EntityFrameworkCore;
using NZWalks.API.Data;
using NZWalks.API.Models.Domain;

namespace NZWalks.API.Repositories
{
    public class SQLRegionRepository : IRegionRepository
    {
        private readonly NZWalksDbContext _dbContext;
        public SQLRegionRepository(NZWalksDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<Region> CreateRegionAsync(Region region)
        {
            await _dbContext.Regions.AddAsync(region);
            await _dbContext.SaveChangesAsync();

            return region;
        }

        public async Task<Region?> DeleteRegionAsync(Guid id)
        {
            var region = await _dbContext.Regions.FirstOrDefaultAsync(r => r.Id == id);

            if (region == null) return null;

            _dbContext.Regions.Remove(region);
            await _dbContext.SaveChangesAsync();

            return region;
        }

        public async Task<IEnumerable<Region>> GetAllAsync()
        {
            return await _dbContext.Regions.ToListAsync();
        }

        public async Task<Region?> GetByIdAsync(Guid id)
        {
            return await _dbContext.Regions.FirstOrDefaultAsync(r => r.Id == id);
        }

        public async Task<Region?> UpdateRegionAsync(Guid id, Region region)
        {
            var regionToUpdate = await _dbContext.Regions.FirstOrDefaultAsync(r => r.Id == id);

            if (regionToUpdate != null) 
            {
                regionToUpdate.Code = region.Code;
                regionToUpdate.Name = region.Name;
                regionToUpdate.RegionImageUrl = region.RegionImageUrl;
                await _dbContext.SaveChangesAsync();
            }

            return regionToUpdate;
        }
    }
}
