using AutoMapper;
using Microsoft.EntityFrameworkCore;
using NZWalks.API.Data;
using NZWalks.API.Models.Domain;

namespace NZWalks.API.Repositories
{
    public class SQLWalkRepository : IWalkRepository
    {
        private readonly NZWalksDbContext _context;
        private readonly IMapper _mapper;
        public SQLWalkRepository(NZWalksDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }
        public async Task<Walk> AddWalkAsync(Walk walk)
        {
            await _context.Walks.AddAsync(walk);
            await _context.SaveChangesAsync();
            return walk;
        }
        public async Task<Walk?> DeleteWalkAsync(Guid id)
        {
            var walkEntity = await _context.Walks.FirstOrDefaultAsync(x => x.Id == id);
            if (walkEntity == null)
            {
                return null;
            }
            _context.Walks.Remove(walkEntity);
            await _context.SaveChangesAsync();
            return walkEntity;
        }
        public async Task<Walk?> GetWalkByIdAsync(Guid id)
        {
            var walkEntity = await _context.Walks
                .Include(w => w.Difficulty)
                .Include(w => w.Region)
                .FirstOrDefaultAsync(w => w.Id == id);

            if (walkEntity == null) return null;

            return walkEntity;
        }
        public async Task<IEnumerable<Walk>> GetWalksAsync(string? filterOn = null, string? filterQuery = null,
            string? sortBy = null, bool isAscending = true,
            int pageNumber = 1, int pageSize = 1000)
        {
            var walks = _context.Walks
                .Include(w => w.Difficulty)
                .Include(w => w.Region)
                .AsQueryable();

            //Apply filtering
            if(string.IsNullOrWhiteSpace(filterOn) == false && string.IsNullOrWhiteSpace(filterQuery) == false)
            {
                if (filterOn.Equals("Name", StringComparison.OrdinalIgnoreCase))
                {
                    walks = walks.Where(x => x.Name.Contains(filterQuery));
                }
                else if(filterOn.Equals("Description", StringComparison.OrdinalIgnoreCase))
                {
                    walks = walks.Where(x => x.Description.Contains(filterQuery));
                }
                else if(filterOn.Equals("LengthInKm", StringComparison.OrdinalIgnoreCase))
                {
                    var isDouble = double.TryParse(filterQuery, out double lengthInKm);
                    if (isDouble)
                        walks = walks.Where(x => x.LengthInKm == lengthInKm);
                }
            }

            //Apply sorting
            if(string.IsNullOrWhiteSpace(sortBy) == false)
            {
                if(sortBy.Equals("name", StringComparison.OrdinalIgnoreCase))
                {
                    walks = isAscending? walks.OrderBy(x => x.Name) : walks.OrderByDescending(x => x.Name);
                }
                else if (sortBy.Equals("lengthinkm", StringComparison.OrdinalIgnoreCase))
                {
                    walks = isAscending ? walks.OrderBy(x => x.LengthInKm) : walks.OrderByDescending(x => x.LengthInKm);
                }
            }

            //Apply pagination
            var skipAmount = pageSize * (pageNumber - 1);

            return await walks.Skip(skipAmount).Take(pageSize).ToListAsync();
        }
        public async Task<IEnumerable<Walk>> GetWalksByDifficultyIdAsync(Guid difficultyId)
        {
            var walkEntities = await _context.Walks
                .Include(w => w.Difficulty)
                .Include(w => w.Region)
                .Where(w => w.DifficultyId == difficultyId)
                .ToListAsync();
            return walkEntities;
        }
        public async Task<IEnumerable<Walk>> GetWalksByRegionIdAsync(Guid regionId)
        {
            var walkEntities = await _context.Walks
                .Include(w => w.Difficulty)
                .Include(w => w.Region)
                .Where(w => w.RegionId == regionId)
                .ToListAsync();
            return walkEntities;
        }

        public async Task<Walk?> UpdateWalkAsync(Guid id, Walk walk)
        {
            var existingWalk = await _context.Walks.FirstOrDefaultAsync(x => x.Id == id);
            if(existingWalk == null) return null;

            existingWalk.Name = walk.Name;
            existingWalk.Description = walk.Description;
            existingWalk.LengthInKm = walk.LengthInKm;
            existingWalk.WalkImageUrl = walk.WalkImageUrl;
            existingWalk.DifficultyId = walk.DifficultyId;
            existingWalk.RegionId = walk.RegionId;

            await _context.SaveChangesAsync();
            return existingWalk;
        }
    }
}