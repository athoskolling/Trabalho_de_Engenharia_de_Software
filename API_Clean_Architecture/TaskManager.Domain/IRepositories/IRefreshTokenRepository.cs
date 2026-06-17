using TaskManager.Domain.Entities;

namespace TaskManager.Domain.IRepositories;

public interface IRefreshTokenRepository
{
    Task<RefreshToken?> GetByTokenAsync(string token);
    Task AddAsync(RefreshToken token);
    Task UpdateAsync(RefreshToken token);
    Task DeleteByUserIdAsync(Guid userId);
}
