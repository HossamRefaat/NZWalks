using NZWalks.API.Models.Domain;

namespace NZWalks.API.Repositories
{
    public interface IWalkRepository
    {
        Task<IEnumerable<Walk>> GetWalksAsync();
        Task<Walk?> GetWalkByIdAsync(Guid id);
        Task<IEnumerable<Walk>> GetWalksByRegionIdAsync(Guid regionId);
        Task<IEnumerable<Walk>> GetWalksByDifficultyIdAsync(Guid difficultyId);
        Task<Walk> AddWalkAsync(Walk walk);
        Task<Walk?> UpdateWalkAsync(Guid id, Walk walk);
        Task<Walk?> DeleteWalkAsync(Guid id);
    }
}
